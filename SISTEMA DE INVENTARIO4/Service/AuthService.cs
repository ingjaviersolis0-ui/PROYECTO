using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SISTEMA_DE_INVENTARIO4.Models;
using Microsoft.EntityFrameworkCore;

namespace SISTEMA_DE_INVENTARIO4.Services
{
    public class AuthService
    {
        private readonly InventarioProyecto4Context _context;
        private readonly IConfiguration _configuration;

        public AuthService(InventarioProyecto4Context context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // Método para validar usuario y contraseña
        public async Task<Usuario?> ValidateUser(Usuario usuario)
        {
            var user = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Username == usuario.Username && u.PasswordHash == usuario.PasswordHash);
            return user;
        }

        // Método para generar el token JWT
        public string GenerateJwtToken(Usuario user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];

            // Crear los "claims" (datos que guarda el token)
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Rol)
            };

            // Crear la llave de seguridad
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Crear el token
            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    int.Parse(jwtSettings["ExpirationMinutes"])),
                signingCredentials: credentials
            );

            // Convertir a string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Configurar la cookie segura
        public CookieOptions GetCookieOptions()
        {
            return new CookieOptions
            {
                HttpOnly = true,       // No accesible desde JavaScript
                Secure = true,         // Solo HTTPS
                SameSite = SameSiteMode.Strict,  // Protección CSRF
                Expires = DateTimeOffset.UtcNow.AddMinutes(10),
                Path = "/"
            };
        }
    }
}
