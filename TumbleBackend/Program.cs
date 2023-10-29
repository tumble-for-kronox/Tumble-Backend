using MongoDB.Bson.Serialization;
using System.Diagnostics;
using System.Net;
using TumbleBackend.ActionFilters;
using TumbleBackend.Library;
using TumbleBackend.OperationFilters;
using TumbleBackend.StringConstants;
using TumbleBackend.Utilities;
using TumbleHttpClient;
using WebAPIModels.ResponseModels;

var builder = WebApplication.CreateBuilder(args);
KronoxUrlFilter kronoxUrlFilter = new KronoxUrlFilter();
builder.WebHost.UseIISIntegration();

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

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(config => {
    config.OperationFilter<AuthHeaderFilter>();
});
builder.Services.AddHttpClient("KronoxClient", client =>
{
    client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36");
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(
      "CorsPolicy",
      builder => builder.AllowAnyOrigin()
      .AllowAnyMethod()
      .AllowAnyHeader());
});

builder.Services.AddSingleton(builder.Configuration);
builder.Services.AddSingleton<MobileMessagingClient>();
builder.Services.AddScoped<AuthActionFilter>();
builder.Services.AddScoped<KronoxUrlFilter>();
builder.Services.AddTransient<KronoxRequestClient>();

builder.Services.AddSpaStaticFiles(config =>
{
    config.RootPath = "wwwroot";
});

//builder.Services.AddMvc(opts =>
//{
//    opts.Filters.Add(new KronoxUrlFilter());
//});

var app = builder.Build();

app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == 404)
    {
        context.Request.Path = "/";
        await next();
    }
});

app.UseCors("CorsPolicy");
string? dbConnectionString = app.Environment.IsDevelopment() ? builder.Configuration[UserSecrets.DbConnection] : Environment.GetEnvironmentVariable(EnvVar.DbConnection);
string? awsAccessKey = app.Environment.IsDevelopment() ? builder.Configuration[UserSecrets.AwsAccessKey] : Environment.GetEnvironmentVariable(EnvVar.AwsAccessKey);
string? awsSecretKey = app.Environment.IsDevelopment() ? builder.Configuration[UserSecrets.AwsSecretKey] : Environment.GetEnvironmentVariable(EnvVar.AwsSecretKey);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

DatabaseAPI.Connector.Init(dbConnectionString!, app.Environment.IsDevelopment());
EmailUtil.Init(awsAccessKey!, awsSecretKey!);

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseDefaultFiles();

app.UseSpaStaticFiles();

app.MapControllers();

app.Run();
