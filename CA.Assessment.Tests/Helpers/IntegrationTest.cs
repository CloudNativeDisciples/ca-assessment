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
    private CurrentUserKindTestProvider currentUserKindProvider;
    private ServiceProvider? serviceProvider;

    [SetUp]
    public void SetUp()
    {
        var serviceCollection = new ServiceCollection();

        currentDatabaseName = $"{Guid.NewGuid():N}.db";

        var databaseConnectionString = $"Data Source = {currentDatabaseName}";

        serviceCollection.AddAssessmentMigrations(databaseConnectionString)
            .AddAssessmentDatabase(databaseConnectionString)
            .AddAssessmentApplication()
            .AddAssessmentInfrastructure();

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
        }
        catch
        {
            // It's not a problem if we can't delete the database file 
        }
    }

    public TService Resolve<TService>()
    {
        if (currentServiceProviderScope is null)
            throw new InvalidOperationException("You must call SetUp() before resolving services");

        return currentServiceProviderScope.ServiceProvider.GetRequiredService<TService>();
    }

    public void SetCurrentUserKind(UserKind userKind)
    {
        currentUserKindProvider.CurrentUserKind = userKind;
    }
}
