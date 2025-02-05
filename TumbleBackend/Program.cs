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
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using System.Diagnostics;
using Prometheus;
using Serilog;
using Serilog.Formatting.Compact;

var builder = WebApplication.CreateBuilder(args);

ConfigureEnvironmentAndSecrets(builder);
ConfigureRateLimiting(builder);
ConfigureLogging(builder);
ConfigureMongoDb();

RegisterServices(builder.Services, builder.Configuration, builder.Environment);

var app = builder.Build();
ConfigureMiddleware(app);

EmailUtil.Init(GetAwsAccessKey(builder.Environment, builder.Configuration), GetAwsSecretKey(builder.Environment, builder.Configuration));
app.UseSerilogRequestLogging();

app.Run();

void ConfigureLogging(WebApplicationBuilder builder)
{
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .CreateLogger();

    builder.Services.AddSerilog(options =>
    {
        options.Enrich.WithProperty("Application", "ProductAPI")
        .Enrich.WithProperty("Environment", GetEnvironment(builder.Environment))
        .WriteTo.Console(new RenderedCompactJsonFormatter());
    });
}

string GetEnvironment(IWebHostEnvironment environment)
{
    return environment.IsDevelopment() ? "Development" : "Production";
}

void ConfigureEnvironmentAndSecrets(WebApplicationBuilder builder)
{
    builder.Configuration.AddJsonFile("secrets/secrets.json", optional: true);
    builder.Configuration.AddEnvironmentVariables();
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
