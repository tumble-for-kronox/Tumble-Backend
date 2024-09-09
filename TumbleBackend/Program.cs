using System.Diagnostics;
using System.Threading.RateLimiting;
using DatabaseAPI;
using DatabaseAPI.Interfaces;
using Microsoft.AspNetCore.RateLimiting;
using MongoDB.Bson.Serialization.Conventions;
using TumbleBackend.Constants;
using TumbleBackend.Filters.ActionFilters;
using TumbleBackend.Filters.OperationFilters;
using TumbleBackend.Middleware.Exceptions;
using TumbleBackend.Modules;

Main();

void Main()
{
    var builder = WebApplication.CreateBuilder(args);

    ConfigureConfiguration(builder);
    ConfigureRateLimiting(builder);
    ConfigureMongoDb();

    RegisterServices(builder.Services, builder.Configuration, builder.Environment);

    EmailUtil.Init(GetAwsAccessKey(builder.Environment, builder.Configuration)!, GetAwsSecretKey(builder.Environment, builder.Configuration)!);

    var app = builder.Build();

    ConfigureMiddleware(app);

    app.Run();
}

void ConfigureConfiguration(WebApplicationBuilder builder)
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
    ConventionPack conventions =
    [
        new CamelCaseElementNameConvention()
    ];
    ConventionRegistry.Register("Custom Conventions", conventions, t => true);
}

void RegisterServices(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
{
    string? dbConnectionString = GetDbConnectionString(environment, configuration);
    string? dbName = GetDbName(environment, configuration);
    MongoDBSettings dbSettings = new(dbConnectionString!, dbName);

    services.AddSingleton(configuration);
    services.AddSingleton<IDbSettings>(dbSettings);
    services.AddSingleton<IDbNewsService>(sp => new MongoNewsService(sp.GetService<IDbSettings>()!));
    services.AddSingleton<MobileMessagingClient>();
    services.AddScoped<AdminAuthFilter>();

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

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    app.UseMiddleware<GeneralExceptionMiddleware>();
    app.UseMiddleware<TimeoutExceptionMiddleware>();
}

string? GetDbConnectionString(IWebHostEnvironment environment, IConfiguration configuration) =>
    environment.IsDevelopment() ? configuration[UserSecrets.DbConnection] : Environment.GetEnvironmentVariable(EnvVar.DbConnection);

string? GetDbName(IWebHostEnvironment environment, IConfiguration configuration) =>
    environment.IsDevelopment() ? configuration[AppSettings.DevDatabase] : configuration[AppSettings.ProdDatabase];

string? GetAwsAccessKey(IWebHostEnvironment environment, IConfiguration configuration) =>
    environment.IsDevelopment() ? configuration[UserSecrets.AwsAccessKey] : Environment.GetEnvironmentVariable(EnvVar.AwsAccessKey);

string? GetAwsSecretKey(IWebHostEnvironment environment, IConfiguration configuration) =>
    environment.IsDevelopment() ? configuration[UserSecrets.AwsSecretKey] : Environment.GetEnvironmentVariable(EnvVar.AwsSecretKey);
