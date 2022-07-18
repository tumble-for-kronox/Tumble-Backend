var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
string? dbConnectionString = null;

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    dbConnectionString = builder.Configuration["DbConnectionString"];
    app.UseSwagger();
    app.UseSwaggerUI();
}
else if (app.Environment.IsProduction())
{
    dbConnectionString = Environment.GetEnvironmentVariable("DbConnectionString");
}



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
