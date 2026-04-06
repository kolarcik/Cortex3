using Microsoft.EntityFrameworkCore;
using ScoreApi.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var conn = builder.Configuration.GetConnectionString("DefaultConnection") ?? Environment.GetEnvironmentVariable("SCORES_DB") ?? "Data Source=scores.db";
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(conn));

builder.Services.AddCors(options => options.AddPolicy("AllowFrontend", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.UseCors("AllowFrontend");
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.MapControllers();
app.Run();
