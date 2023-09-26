using System;
using System.Collections.Generic;

namespace StoreApi.Models;

public partial class ShopAddress
{
    public int Id { get; set; }

    public int ShopId { get; set; }

    public string Address { get; set; } = null!;

    public string Building { get; set; } = null!;

    public string? Landmark { get; set; }

    public decimal CoordinateX { get; set; }

    public decimal CoordinateY { get; set; }

    public virtual Shop Shop { get; set; } = null!;
}
