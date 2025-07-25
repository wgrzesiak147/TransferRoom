using SquadFinder.Application.Interfaces;
using SquadFinder.Infrastructure.Mappers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var apiFootballConfig = builder.Configuration.GetSection("ApiFootball");

builder.Services.AddHttpClient<IFootballApiService, FootballApiService>(client =>
{
    var baseUrl = apiFootballConfig.GetValue<string>("BaseUrl");
    client.BaseAddress = new Uri(baseUrl ?? throw new InvalidOperationException("BaseUrl is not configured"));
});

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

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
