using Microsoft.EntityFrameworkCore;
using Scoreboard.Api.Data;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// configuration
var configuration = builder.Configuration;
var conn = configuration.GetConnectionString("DefaultConnection") ?? Environment.GetEnvironmentVariable("SCORES_CONN") ?? "Data Source=scores.db";

// services
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(conn));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "Scoreboard API", Version = "v1" }));

// CORS
var allowed = configuration.GetSection("AllowedOrigins").Get<string[]>() ?? new[]{ "http://localhost:8000" };
builder.Services.AddCors(o => o.AddPolicy("default", p => p.WithOrigins(allowed).AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

// ensure DB
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("default");
app.MapControllers();
app.Run();
