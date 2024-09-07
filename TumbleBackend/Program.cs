using DatabaseAPI;
using DatabaseAPI.Interfaces;
using TumbleBackend.ActionFilters;
using TumbleBackend.ExceptionMiddleware;
using TumbleBackend.Middleware;
using TumbleBackend.Library;
using TumbleBackend.OperationFilters;
using TumbleBackend.StringConstants;
using TumbleBackend.Utilities;
using TumbleHttpClient;
using WebAPIModels.ResponseModels;
using OpenTelemetry.Trace;
using Grafana.OpenTelemetry;
using OpenTelemetry.Logs;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using System.Diagnostics;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

ConfigureEnvironmentAndSecrets(builder);
//ConfigureTracing(builder);
ConfigureRateLimiting(builder);
ConfigureMongoDb();

RegisterServices(builder.Services, builder.Configuration, builder.Environment);

var app = builder.Build();
ConfigureMiddleware(app);

EmailUtil.Init(GetAwsAccessKey(builder.Environment, builder.Configuration), GetAwsSecretKey(builder.Environment, builder.Configuration));

app.Run();

void ConfigureEnvironmentAndSecrets(WebApplicationBuilder builder)
{
    builder.Configuration.AddJsonFile("secrets/secrets.json", optional: true);
    builder.Configuration.AddEnvironmentVariables();
}

void ConfigureTracing(WebApplicationBuilder builder)
{
    builder.Services.AddOpenTelemetry()
        .WithTracing(tracerProviderBuilder =>
        {
            tracerProviderBuilder
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .UseGrafana()
                .AddConsoleExporter();
        });

    builder.Logging.AddOpenTelemetry(options =>
    {
        options.UseGrafana().AddConsoleExporter();
    });
}

void ConfigureRateLimiting(WebApplicationBuilder builder)
{
    builder.Services.AddRateLimiter(_ => _
        .AddFixedWindowLimiter(policyName: "fixed", options =>
        {
            options.PermitLimit = 4;
            options.Window = TimeSpan.FromSeconds(12);
            options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            options.QueueLimit = 2;
        }));
}

void ConfigureMongoDb()
{
    BsonClassMap.RegisterClassMap<EventWebModel>(cm =>
    {
        cm.AutoMap();
        cm.UnmapProperty(c => c.Id);
        cm.MapMember(c => c.Id)
            .SetElementName("id")
            .SetOrder(0)
            .SetIsRequired(true);
    });

    ConventionPack conventions = new()
    {
        new CamelCaseElementNameConvention()
    };
    ConventionRegistry.Register("Custom Conventions", conventions, t => true);
}

void RegisterServices(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
{
    string? dbConnectionString = GetDbConnectionString(environment, configuration);
    string? dbName = GetDbName(environment, configuration);
    MongoDBSettings dbSettings = new(dbConnectionString!, dbName!);

    services.AddSingleton(configuration);
    services.AddSingleton(dbSettings);
    services.AddSingleton<IDbSettings>(dbSettings);
    services.AddSingleton<IDbSchedulesService>(sp => new MongoSchedulesService(sp.GetService<IDbSettings>()!));
    services.AddSingleton<IDbProgrammeFiltersService>(sp => new MongoProgrammeFiltersService(sp.GetService<IDbSettings>()!));
    services.AddSingleton<IDbNewsService>(sp => new MongoNewsService(sp.GetService<IDbSettings>()!));
    services.AddSingleton<IDbKronoxCacheService>(sp => new MongoKronoxCacheService(sp.GetService<IDbSettings>()!));
    services.AddSingleton<MobileMessagingClient>();
    services.AddTransient<JwtUtil>();
    services.AddTransient<TestUserUtil>();

    services.AddScoped<AuthActionFilter>();
    services.AddScoped<KronoxUrlFilter>();
    services.AddScoped<KronoxRequestClient>();

    services.AddControllers();
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(config =>
    {
        config.OperationFilter<AuthHeaderFilter>();
    });


    services.AddCors(options =>
    {
        options.AddPolicy("CorsPolicy", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
    });

    services.AddSpaStaticFiles(config => { config.RootPath = "wwwroot"; });

    var dbglistener = new TextWriterTraceListener(Console.Out);
    Trace.Listeners.Add(dbglistener);
}

void ConfigureMiddleware(WebApplication app)
{
    app.UseRouting();
    app.UseCors("CorsPolicy");
    app.UseRateLimiter();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseAuthorization();
    app.UseDefaultFiles();
    app.UseSpaStaticFiles();

    app.UseMiddleware<GeneralExceptionMiddleware>();
    app.UseMiddleware<TimeoutExceptionMiddleware>();
    app.UseMiddleware<TestUserMiddleware>();

    app.UseMetricServer();
    app.UseHttpMetrics();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapMetrics();
        endpoints.MapControllers();
    });
}

string? GetDbConnectionString(IWebHostEnvironment environment, IConfiguration configuration) =>
    environment.IsDevelopment() ? configuration[UserSecrets.DbConnection] : Environment.GetEnvironmentVariable(EnvVar.DbConnection);

string? GetDbName(IWebHostEnvironment environment, IConfiguration configuration) =>
    environment.IsDevelopment() ? configuration[AppSettings.DevDatabase] : configuration[AppSettings.ProdDatabase];

string? GetAwsAccessKey(IWebHostEnvironment environment, IConfiguration configuration) =>
    environment.IsDevelopment() ? configuration[UserSecrets.AwsAccessKey] : Environment.GetEnvironmentVariable(EnvVar.AwsAccessKey);

string? GetAwsSecretKey(IWebHostEnvironment environment, IConfiguration configuration) =>
    environment.IsDevelopment() ? configuration[UserSecrets.AwsSecretKey] : Environment.GetEnvironmentVariable(EnvVar.AwsSecretKey);
