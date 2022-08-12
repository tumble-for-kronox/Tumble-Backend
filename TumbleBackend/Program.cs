using System.Diagnostics;
using TumbleBackend.StringConstants;
using TumbleBackend.Utilities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var dbglistener = new TextWriterTraceListener(Console.Out);
Trace.Listeners.Add(dbglistener);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
string? dbConnectionString = app.Environment.IsDevelopment() ? builder.Configuration[UserSecrets.DbConnection] : Environment.GetEnvironmentVariable(EnvVar.DbConnection);
string? translationKey = app.Environment.IsDevelopment() ? builder.Configuration[UserSecrets.AzureTranslatorKey] : Environment.GetEnvironmentVariable(EnvVar.AzureTranslatorKey);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

    app.UseSwagger();
    app.UseSwaggerUI();
}

DatabaseAPI.Connector.Init(dbConnectionString!);
TranslatorUtil.Init(translationKey!);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
