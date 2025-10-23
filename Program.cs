using Microsoft.EntityFrameworkCore;
using ApiComederoPet.Data;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ----------------------------------------------
// 🔹 CORS: permitir peticiones desde tu frontend
// ----------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(
                "https://localhost:7157",      // Frontend local
                "https://apicomederopet.onrender.com" // (opcional) permitir desde Render si hospedas el panel allí
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ----------------------------------------------
// 🔹 Controladores y Swagger
// ----------------------------------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ----------------------------------------------
// 🔹 Base de datos PostgreSQL
// ----------------------------------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PetFeederDB")));

var app = builder.Build();

// ----------------------------------------------
// 🔹 Swagger
// ----------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiComederoPet v1");
        c.RoutePrefix = string.Empty; // Swagger en raíz "/"
    });
}

// ----------------------------------------------
// 🔹 Middleware de CORS antes de redirección HTTPS
// ----------------------------------------------
app.UseCors("AllowFrontend");

app.UseHttpsRedirection();
app.MapControllers();
app.Run();

