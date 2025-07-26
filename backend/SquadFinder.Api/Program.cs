using SquadFinder.Infrastructure;
using SquadFinder.Infrastructure.Mappers;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole(); // Logs to console, usually we could add here some external tool for collecting logs

var frontEndOrigin = "squadFinderFrontEnd";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: frontEndOrigin,
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddScoped<IFootballApiMapper, FootballApiMapper>();

builder.Services.AddFootballApiClient(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.UseCors(frontEndOrigin);

app.MapControllers();

app.Run();
