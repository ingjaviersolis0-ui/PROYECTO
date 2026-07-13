using Microsoft.AspNetCore.Mvc;
using SISTEMA_DE_INVENTARIO4.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using SISTEMA_DE_INVENTARIO4.Attributes;

namespace SISTEMA_DE_INVENTARIO4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [SimpleThrottle(MaxRequests = 10, Seconds = 10)] 
    public class Usuarios : ControllerBase
    {
        private readonly InventarioProyecto4Context _context;

        public Usuarios(InventarioProyecto4Context context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("Usuarios Autenticados")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetUsuario()
        {
            var usuario = await _context.Usuarios.ToListAsync();
            return Ok(usuario);
        }
        [HttpPost]
        [Route("Registrar Usuario")]
        public async Task<IActionResult> PostUsuario(int Id, string Nombre, string Email, string Username, string PasswordHash, string Rol)
        {
            var usuarioExistente = await _context.Usuarios.FindAsync(Id);
            if (usuarioExistente != null)
            {
                return BadRequest("Ya existe un usuario con este ID.");
            }

            var usuario = new Usuario
            {
                Id = Id,
                Nombre = Nombre,
                Email = Email,
                Username = Username,
                PasswordHash = PasswordHash,
                Rol = Rol
            };
            {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return Ok(usuario);
            }  
        }
        [HttpDelete]
        [Route("Eliminar Usuario")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return Ok(usuario);
        }
    }
}