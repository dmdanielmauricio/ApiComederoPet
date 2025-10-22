using Microsoft.EntityFrameworkCore;
using ApiComederoPet.Data;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Conectar DbContext a PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PetFeederDB")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // Habilitar Swagger siempre
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiComederoPet v1");
        c.RoutePrefix = string.Empty; // Hace que Swagger se muestre en la raíz "/"
    });

}

//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
