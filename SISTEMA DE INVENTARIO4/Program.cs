using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SISTEMA_DE_INVENTARIO4.Models;
using SISTEMA_DE_INVENTARIO4.Services;


var builder = WebApplication.CreateBuilder(args);

// ===== 1. Agregar servicios =====
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ===== 2. Configurar base de datos =====
builder.Services.AddDbContext<InventarioProyecto4Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConexDB")));

// ===== 3. Registrar nuestro servicio =====
builder.Services.AddScoped<AuthService>();

// ===== 4. Configurar autenticación JWT con cookies =====
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secretKey))
    };

    // ESTO ES LO IMPORTANTE: Leer el token de la cookie
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Buscar el token en la cookie "AuthToken"
            var token = context.Request.Cookies["AuthToken"];
            if (!string.IsNullOrEmpty(token))
            {
                context.Token = token;
            }
            return Task.CompletedTask;
        }
    };
});

// ===== 5. Configurar CORS (para frontend) =====
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5500")
              .AllowCredentials()  // Necesario para cookies
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// ===== 6. Configurar middleware =====
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");

// IMPORTANTE: Autenticación antes de Autorización
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ===== 7. Crear/migrar base de datos automáticamente =====
/*using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<InventarioProyecto4Context>();
    dbContext.Database.EnsureCreated(); // Crea la BD si no existe
}*/

app.Run();