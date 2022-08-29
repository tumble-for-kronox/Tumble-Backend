using MongoDB.Bson.Serialization;
using System.Diagnostics;
using TumbleBackend.StringConstants;
using TumbleBackend.Utilities;
using WebAPIModels.ResponseModels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
string? dbConnectionString = app.Environment.IsDevelopment() ? builder.Configuration[UserSecrets.DbConnection] : Environment.GetEnvironmentVariable(EnvVar.DbConnection);
string? awsAccessKey = app.Environment.IsDevelopment() ? builder.Configuration[UserSecrets.AwsAccessKey] : Environment.GetEnvironmentVariable(EnvVar.AwsAccessKey);
string? awsSecretKey = app.Environment.IsDevelopment() ? builder.Configuration[UserSecrets.AwsSecretKey] : Environment.GetEnvironmentVariable(EnvVar.AwsSecretKey);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

    app.UseSwagger();
    app.UseSwaggerUI();
}

DatabaseAPI.Connector.Init(dbConnectionString!);
EmailUtil.Init(awsAccessKey!, awsSecretKey!);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
