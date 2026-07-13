using Microsoft.AspNetCore.Mvc;
using SISTEMA_DE_INVENTARIO4.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using SISTEMA_DE_INVENTARIO4.Attributes;

namespace SISTEMA_DE_INVENTARIO4.Controllers
{
    //autorizado para admin y el gerente
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin,gerente")] 
    [SimpleThrottle(MaxRequests = 10, Seconds = 10)] 
   
    public class Productos : ControllerBase
    {
        private readonly InventarioProyecto4Context _context;

        public Productos(InventarioProyecto4Context context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("Productos Autenticados")]
        public async Task<IActionResult> GetProducto()
        {
            var producto = await _context.Productos.ToListAsync();
            return Ok(producto);
        }
        [HttpGet]
        [Route("Invertido por cada producto y total en el almacen")]
        public async Task<IActionResult> GetInvertidoPorProducto()
        {
            var productos = await _context.Productos
                .Select(p => new
                {
                    p.IdCodigoP,
                    p.NombreProducto,
                    Invertido = p.PrecioCompra * p.Stock
                })
                .ToListAsync();

            var totalInvertido = productos.Sum(p => p.Invertido);

            return Ok(new { Productos = productos, TotalInvertido = totalInvertido });
        }
        [HttpPost]
        [Route("Registrar Producto")]
        public async Task<IActionResult> PostProducto(int idCodigoP, string nombreProducto, int stock, decimal precioCompra, decimal precioVenta, decimal? iva = null, string marca="")
        {
            var productoExistente = await _context.Productos.FindAsync(idCodigoP);
            if (productoExistente != null)
            {
                return BadRequest("Ya existe un producto con este ID.");
            }

            var producto = new Producto
            {
                IdCodigoP = idCodigoP,
                NombreProducto = nombreProducto,
                Stock = stock,
                PrecioCompra = precioCompra,
                PrecioVenta = precioVenta,
                Iva = iva,
                Marca = marca
            };

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            return Ok(producto);
        }
        [HttpDelete]
        [Route("EliminarProducto")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();
            return Ok(producto);
        }
    }
}