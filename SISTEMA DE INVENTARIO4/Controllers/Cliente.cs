using Microsoft.AspNetCore.Mvc;
using SISTEMA_DE_INVENTARIO4.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using SISTEMA_DE_INVENTARIO4.Attributes;

namespace SISTEMA_DE_INVENTARIO4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "cajera,cajero")] 
    [SimpleThrottle(MaxRequests = 5, Seconds = 10)] 
    public class Clientes : ControllerBase
    {
        private readonly InventarioProyecto4Context _context;

        public Clientes(InventarioProyecto4Context context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("Clientes Autenticados")]
        public async Task<IActionResult> GetCliente()
        {
            var cliente = await _context.Clientes.ToListAsync();
            return Ok(cliente);
        }
        [HttpPost]
        [Route("Registrar Cliente")]
        public async Task<IActionResult> PostCliente(int id, string nombre, string apellido, int cedula, string telefono, string correoElectronico)
        {
            var clienteExistente = await _context.Clientes.FindAsync(id);
            if (clienteExistente != null)
            {
                return BadRequest("Ya existe un cliente con este ID.");
            }

            var cliente = new Cliente
            {
                Id = id,
                Nombre = nombre,
                Apellido = apellido,
                Telefono = telefono,
                Cedula = cedula.ToString(),
                Correo = correoElectronico
            };

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return Ok(cliente);
        }
        [HttpDelete]
        [Route("Eliminar Cliente")]
        public async Task<IActionResult> DeleteCliente(int cedula)
        {
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Cedula == cedula.ToString());
            if (cliente == null)
            {
                return NotFound("El cliente no existe.");
            }
            //cuando se borra un cliente, se borran todas sus facturas y compras asociadas si tiene la misma cedula
            var facturas = await _context.Facturas.Where(f => f.IdCliente == cliente.Id).ToListAsync();
            _context.Facturas.RemoveRange(facturas);

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
            return Ok(cliente);
        }
    }
}