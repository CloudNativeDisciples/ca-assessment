using CA.Assessment.Application.Extensions;
using CA.Assessment.Application.Services;
using CA.Assessment.Database.Migrations.Extensions;
using CA.Assessment.Database.Sqlite.Extensions;
using CA.Assessment.Images.Extensions;
using CA.Assessment.Model;
using CA.Assessment.Store.Extensions;
using CA.Assessment.WebAPI.Services;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using Serilog;

namespace CA.Assessment.Tests.Helpers;

public abstract class IntegrationTest
{
    private string? _currentDatabaseName;

    private IServiceScope? _currentServiceProviderScope;

    private CurrentUserKindTestProvider? _currentUserKindProvider;
    private string? _imageStoreFolder;
    private ServiceProvider? _serviceProvider;

    [SetUp]
    public void SetUp()
    {
        var serviceCollection = new ServiceCollection();

        _currentDatabaseName = $"{Guid.NewGuid():N}.db";
        _imageStoreFolder = $"{_imageStoreFolder}.images";

        var databaseConnectionString = $"Data Source = {_currentDatabaseName}";

        serviceCollection.AddAssessmentMigrations(databaseConnectionString)
            .AddAssessmentDatabase(databaseConnectionString)
            .AddAssessmentApplication()
            .AddAssessmentSqliteDatabase()
            .AddAssessmentFileSystemImageStore(opts => opts.Folder = _imageStoreFolder);

        _currentUserKindProvider = new CurrentUserKindTestProvider();

        var loggerMock = new Mock<ILogger>();

        serviceCollection.AddTransient<TxScriptsFacade>();
        serviceCollection.AddSingleton<ICurrentUserKindProvider>(_currentUserKindProvider);
        serviceCollection.AddSingleton(loggerMock.Object);

        _serviceProvider = serviceCollection.BuildServiceProvider();

        _currentServiceProviderScope = _serviceProvider.CreateScope();

        var migrator = Resolve<IMigrationRunner>();

        migrator.MigrateUp();
    }

    [TearDown]
    public void TearDown()
    {
        _currentServiceProviderScope?.Dispose();
        _serviceProvider?.Dispose();

        _currentServiceProviderScope = null;
        _serviceProvider = null;

        try
        {
            if (_currentDatabaseName is not null)
            {
                File.Delete(_currentDatabaseName);
            }

            if (_imageStoreFolder is not null)
            {
                Directory.Delete(_imageStoreFolder, true);
            }
        }
        catch (IOException)
        {
            // It's not a problem if we can't delete the database file or image store
        }
    }

    protected TService Resolve<TService>() where TService : notnull
    {
        if (_currentServiceProviderScope is null)
        {
            throw new InvalidOperationException("You must call SetUp() before resolving services");
        }

        return _currentServiceProviderScope.ServiceProvider.GetRequiredService<TService>();
    }

    protected void SetCurrentUserKind(UserKind userKind)
    {
        if (_currentUserKindProvider is not null)
        {
            _currentUserKindProvider.CurrentUserKind = userKind;
        }
    }
}
