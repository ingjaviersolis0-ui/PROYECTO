using Microsoft.AspNetCore.Mvc;
using SISTEMA_DE_INVENTARIO4.Attributes;
using SISTEMA_DE_INVENTARIO4.Models;
using SISTEMA_DE_INVENTARIO4.Services;

namespace SISTEMA_DE_INVENTARIO4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        // POST: api/auth/login
        [HttpPost("Iniciar Sesión")]
        [SimpleThrottle(MaxRequests = 3, Seconds = 10)] 
        public async Task<IActionResult> Login(string username, string password)
        {
            // 1. Validar credenciales
            var user = await _authService.ValidateUser(new Usuario { Username = username, PasswordHash = password });

            if (user == null)
            {
                return Unauthorized(new { message = "Usuario o contraseña incorrectos" });
            }

            // 2. Generar token JWT
            var token = _authService.GenerateJwtToken(user);

            // 3. Guardar token en cookie
            var cookieOptions = _authService.GetCookieOptions();
            Response.Cookies.Append("AuthToken", token, cookieOptions);

            // 4. Retornar respuesta exitosa
            return Ok(new
            {
                success = true,
                message = "Login exitoso",
                user = new
                {
                    user.Id,
                    user.Username,
                    user.Email,
                    user.Rol
                }
            });
        }

        // POST: api/auth/logout
        [HttpPost("Cerrar Sesión")]
        public IActionResult Logout()
        {
            // Eliminar la cookie
            Response.Cookies.Delete("AuthToken");
            return Ok(new { success = true, message = "Sesión cerrada" });
        }

        // GET: api/auth/me
        [HttpGet("me")]
       
        public IActionResult GetCurrentUser()
        {
            // Leer datos del usuario autenticado
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var username = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            return Ok(new
            {
                userId,
                username,
                email,
                role
            });
        }
    }

    // Clase auxiliar para recibir datos del login
    public class LoginRequest
    {
        public string username { get; set; }
        public string password { get; set; }
    }
}