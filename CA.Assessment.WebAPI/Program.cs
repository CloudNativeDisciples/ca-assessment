using CA.Assessment.Application.Extensions;
using CA.Assessment.Application.Providers;
using CA.Assessment.Database.Migrations.Extensions;
using CA.Assessment.Infrastructure.Extensions;
using CA.Assessment.Store.Extensions;
using CA.Assessment.WebAPI.Host;
using CA.Assessment.WebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
    .AddEnvironmentVariables("CA_ASSESSMENT_")
    .Build();

builder.Host.ConfigureAppConfiguration(cfg =>
{
    cfg.Sources.Clear();
    cfg.AddConfiguration(configuration);
});

var databaseConnectionStringValue = configuration.GetSection("Database").Get<string>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddHostedService<MigrationsHostedService>();
builder.Services.AddTransient<ICurrentUserKindProvider, CurrentHttpContextUserKindProvider>();

builder.Services.AddAssessmentDatabase(databaseConnectionStringValue)
    .AddAssessmentMigrations(databaseConnectionStringValue)
    .AddAssessmentApplication()
    .AddAssessmentInfrastructure();

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();
