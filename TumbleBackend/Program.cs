using TumbleBackend.Utilities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
string? dbConnectionString = null;
string? translationKey = null;

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    dbConnectionString = builder.Configuration["DbConnectionString"];
    translationKey = builder.Configuration["AzureTranslatorKey"];

    app.UseSwagger();
    app.UseSwaggerUI();
}
else if (app.Environment.IsProduction())
{
    dbConnectionString = Environment.GetEnvironmentVariable("DbConnectionString");
    translationKey = Environment.GetEnvironmentVariable("AzureTranslatorKey");
}

DatabaseAPI.Connector.Init(dbConnectionString!);
TranslatorUtil.Init(translationKey!);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
