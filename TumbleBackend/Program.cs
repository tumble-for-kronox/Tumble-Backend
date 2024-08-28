using DatabaseAPI;
using DatabaseAPI.Interfaces;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using System.Diagnostics;
using TumbleBackend.ActionFilters;
using TumbleBackend.ExceptionMiddleware;
using TumbleBackend.Library;
using TumbleBackend.OperationFilters;
using TumbleBackend.StringConstants;
using TumbleBackend.Utilities;
using TumbleHttpClient;
using WebAPIModels.ResponseModels;
using Prometheus;
using Microsoft.Extensions.Configuration.Json;

var builder = WebApplication.CreateBuilder(args);

// Kubernetes specific secrets config
builder.Configuration.AddJsonFile("secrets/secrets.json", optional: true);
builder.Configuration.AddEnvironmentVariables();

// Configuration and service registration
string? dbConnectionString = builder.Configuration[UserSecrets.DbConnection];
string? dbName = builder.Environment.IsDevelopment() ? builder.Configuration[AppSettings.DevDatabase] : builder.Configuration[AppSettings.ProdDatabase];
MongoDBSettings dbSettings = new(dbConnectionString!, dbName);

string? awsAccessKey = builder.Environment.IsDevelopment() ? builder.Configuration[UserSecrets.AwsAccessKey] : Environment.GetEnvironmentVariable(EnvVar.AwsAccessKey);
string? awsSecretKey = builder.Environment.IsDevelopment() ? builder.Configuration[UserSecrets.AwsSecretKey] : Environment.GetEnvironmentVariable(EnvVar.AwsSecretKey);

var dbglistener = new TextWriterTraceListener(Console.Out);
Trace.Listeners.Add(dbglistener);

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

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(config =>
{
    config.OperationFilter<AuthHeaderFilter>();
});
builder.Services.AddHttpClient("KronoxClient", client =>
{
    client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36");
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddSingleton(builder.Configuration);
builder.Services.AddSingleton<MobileMessagingClient>();
builder.Services.AddSingleton<IDbSettings>(dbSettings);
builder.Services.AddSingleton<IDbSchedulesService>(sp => new MongoSchedulesService(sp.GetService<IDbSettings>()!));
builder.Services.AddSingleton<IDbProgrammeFiltersService>(sp => new MongoProgrammeFiltersService(sp.GetService<IDbSettings>()!));
builder.Services.AddSingleton<IDbNewsService>(sp => new MongoNewsService(sp.GetService<IDbSettings>()!));
builder.Services.AddSingleton<IDbKronoxCacheService>(sp => new MongoKronoxCacheService(sp.GetService<IDbSettings>()!));
builder.Services.AddTransient<JwtUtil>();

builder.Services.AddScoped<AuthActionFilter>();
builder.Services.AddScoped<KronoxUrlFilter>();
builder.Services.AddScoped<KronoxRequestClient>();

builder.Services.AddSpaStaticFiles(config =>
{
    config.RootPath = "wwwroot";
});

var app = builder.Build();

// Middleware configuration
app.UseRouting();
app.UseCors("CorsPolicy");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.UseDefaultFiles();
app.UseSpaStaticFiles();

app.MapControllers();

app.UseEndpoints(endpoints =>
{
    endpoints.MapMetrics();
    endpoints.MapControllers();
});

app.UseMiddleware<GeneralExceptionMiddleware>();
app.UseMiddleware<TimeoutExceptionMiddleware>();

EmailUtil.Init(awsAccessKey!, awsSecretKey!);

app.Run();
