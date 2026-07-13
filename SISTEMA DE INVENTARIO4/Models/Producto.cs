using System;
using System.Collections.Generic;

namespace SISTEMA_DE_INVENTARIO4.Models;

public partial class Producto
{
    public int IdCodigoP { get; set; }

    public string NombreProducto { get; set; } = null!;

    public string Marca { get; set; } = null!;

    public decimal PrecioCompra { get; set; }

    public decimal PrecioVenta { get; set; }

    public int Stock { get; set; }

    public decimal? Iva { get; set; }
}
