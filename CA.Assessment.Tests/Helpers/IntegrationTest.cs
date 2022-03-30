using System;
using System.IO;
using CA.Assessment.Application.Extensions;
using CA.Assessment.Application.Providers;
using CA.Assessment.Database.Migrations.Extensions;
using CA.Assessment.Domain.Anemic;
using CA.Assessment.Infrastructure.Extensions;
using CA.Assessment.Store.Extensions;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CA.Assessment.Tests.Helpers;

public abstract class IntegrationTest
{
    private string? currentDatabaseName;
    private IServiceScope? currentServiceProviderScope;
    private CurrentUserKindTestProvider? currentUserKindProvider;
    private string? imageStoreFolder;
    private ServiceProvider? serviceProvider;

    [SetUp]
    public void SetUp()
    {
        var serviceCollection = new ServiceCollection();

        currentDatabaseName = $"{Guid.NewGuid():N}.db";
        imageStoreFolder = $"{imageStoreFolder}.images";

        var databaseConnectionString = $"Data Source = {currentDatabaseName}";

        serviceCollection.AddAssessmentMigrations(databaseConnectionString)
            .AddAssessmentDatabase(databaseConnectionString)
            .AddAssessmentApplication()
            .AddAssessmentInfrastructure(imageStoreFolder);

        currentUserKindProvider = new CurrentUserKindTestProvider();

        serviceCollection.AddSingleton<ICurrentUserKindProvider>(currentUserKindProvider);

        serviceProvider = serviceCollection.BuildServiceProvider();

        currentServiceProviderScope = serviceProvider.CreateScope();

        var migrator = Resolve<IMigrationRunner>();

        migrator.MigrateUp();
    }

    [TearDown]
    public void TearDown()
    {
        currentServiceProviderScope?.Dispose();
        serviceProvider?.Dispose();

        currentServiceProviderScope = null;
        serviceProvider = null;

        try
        {
            if (currentDatabaseName is not null) File.Delete(currentDatabaseName);
            if (imageStoreFolder is not null) Directory.Delete(imageStoreFolder, true);
        }
        catch
        {
            // It's not a problem if we can't delete the database file or image store
        }
    }

    protected TService Resolve<TService>() where TService : notnull
    {
        if (currentServiceProviderScope is null)
            throw new InvalidOperationException("You must call SetUp() before resolving services");

        return currentServiceProviderScope.ServiceProvider.GetRequiredService<TService>();
    }

    protected void SetCurrentUserKind(UserKind userKind)
    {
        if (currentUserKindProvider is not null) currentUserKindProvider.CurrentUserKind = userKind;
    }
}
