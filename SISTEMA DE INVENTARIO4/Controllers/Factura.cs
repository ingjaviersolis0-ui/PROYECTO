using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SISTEMA_DE_INVENTARIO4.Models; 
using Microsoft.AspNetCore.Authorization;
using SISTEMA_DE_INVENTARIO4.Attributes;

namespace SISTEMA_DE_INVENTARIO4.Controllers
{
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "cajera,cajero")]
[SimpleThrottle(MaxRequests = 20, Seconds = 10)] 
public class FacturaController : ControllerBase
{
    private readonly InventarioProyecto4Context _context; 

    public FacturaController(InventarioProyecto4Context context)
    {
        _context = context;
    }
    [HttpGet]
    [Route("Productos disponibles para facturación")]
    public async Task<IActionResult> GetProductosDisponibles()
    {
        //mostrando el nombre del producto nada mas siempre y cuando el stock sea mayor a 0
        var productosDisponibles = await _context.Productos
            .Where(p => p.Stock > 0)
            .Select(p => new { p.IdCodigoP, p.NombreProducto, p.PrecioVenta })
            .ToListAsync();
        return Ok(productosDisponibles);
    }
    [HttpPost]
    [Route("Facturación")]
    public async Task<IActionResult> PostFactura(int idProducto, int cedula, int cantidadCompra,int idFactura)
    {
    var facturaExistente = await _context.Facturas.FindAsync(idFactura);
    if (facturaExistente != null) return BadRequest("Ya existe una factura con este ID.");

    var producto = await _context.Productos.FindAsync(idProducto);
    if (producto == null) return NotFound("El producto no existe.");

    var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Cedula == cedula.ToString());
    if (cliente == null) return NotFound("El cliente no existe.");

    if (producto.Stock < cantidadCompra) return BadRequest("No hay suficiente stock.");

    // Descontamos stock del almacén
    producto.Stock -= cantidadCompra;

    // CREAMOS LA FACTURA CON SUS COLUMNAS PROPIAS
    var factura = new Factura
    {
        IdCodigoF = idFactura,
        IdCliente = cliente.Id,    
        IdProducto = idProducto,   
        CantidadCompra = cantidadCompra,
        Precio = producto.PrecioVenta,
        Fecha = DateOnly.FromDateTime(DateTime.Now)
    };

    _context.Facturas.Add(factura);
    await _context.SaveChangesAsync(); 

    return Ok(factura);
    }

    // Consultar el historial de un cliente específico bien ordenado
    [HttpGet]
    [Route("Factura del cliente con su respectiva compra")]
    public async Task<IActionResult> GetFacturasCliente(int cedula) //busqueda por cedula
    {
        var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Cedula == cedula.ToString());
        if (cliente == null) return NotFound("El cliente no existe.");

        var facturasdelproductodelcliente = await _context.Facturas
            .Where(f => f.IdCliente == cliente.Id)
            .Join(_context.Productos,
                factura => factura.IdProducto,
                producto => producto.IdCodigoP,
                (factura, producto) => new
                {
                    FacturaId = factura.IdCodigoF,
                    Cliente = $"{cliente.Nombre} {cliente.Apellido}",
                    cliente.Cedula,
                    factura.Fecha,
                    ProductoComprado = producto.NombreProducto,
                    PrecioUnitario = factura.Precio, 
                    factura.CantidadCompra,
                    producto.Iva,
                    SubTotal = factura.CantidadCompra * factura.Precio,
                    Total = factura.CantidadCompra * factura.Precio * (1 + (producto.Iva / 100))
                }).ToListAsync();

        return Ok(facturasdelproductodelcliente);
    }
   

    // Registro Completo de todas las facturas en el sistema
    [HttpGet]
    [Route("Facturas registradas en el sistema")]
    public async Task<IActionResult> GetFacturas()
    {
        var facturas = await _context.Facturas.ToListAsync();
        return Ok(facturas);
    }

    // Eliminar una factura por su ID
    [HttpDelete]
    [Route("Eliminar Factura")]
    public async Task<IActionResult> DeleteFactura(int id)
    {
        var factura = await _context.Facturas.FindAsync(id);
        if (factura == null) return NotFound("La factura no existe.");

        _context.Facturas.Remove(factura);
        await _context.SaveChangesAsync();
        
        return Ok(new { Mensaje = "Factura eliminada correctamente.", FacturaDeleted = factura });
    }
}
}