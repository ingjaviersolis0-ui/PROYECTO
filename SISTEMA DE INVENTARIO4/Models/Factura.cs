using System;
using System.Collections.Generic;

namespace SISTEMA_DE_INVENTARIO4.Models;

public partial class Factura
{
    public int IdCodigoF { get; set; }

    public int IdCliente { get; set; }

    public int IdProducto { get; set; }

    public decimal Precio { get; set; }

    public DateOnly Fecha { get; set; }

    public int CantidadCompra { get; set; }
}
