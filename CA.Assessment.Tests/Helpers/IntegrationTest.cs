using System;
using System.IO;
using CA.Assessment.Application.Extensions;
using CA.Assessment.Application.Providers;
using CA.Assessment.Database.Migrations.Extensions;
using CA.Assessment.Domain.Anemic;
using CA.Assessment.Images.Extensions;
using CA.Assessment.Infrastructure.Extensions;
using CA.Assessment.Store.Extensions;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CA.Assessment.Tests.Helpers;

public abstract class IntegrationTest
{
    private string? _currentDatabaseName;
    private string? _imageStoreFolder;

    private CurrentUserKindTestProvider? _currentUserKindProvider;

    private IServiceScope? _currentServiceProviderScope;
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
            .AddAssessmentInfrastructure()
            .AddFileSystemImageStore(opts => opts.Folder = _imageStoreFolder);

        _currentUserKindProvider = new CurrentUserKindTestProvider();

        serviceCollection.AddSingleton<ICurrentUserKindProvider>(_currentUserKindProvider);

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
        catch
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