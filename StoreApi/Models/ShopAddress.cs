using System;
using System.Collections.Generic;

namespace StoreApi.Models;

public partial class ShopAddress
{
    public int Id { get; set; }

    public int ShopId { get; set; }

    public string City { get; set; } = null!;

    public string StreetName { get; set; } = null!;

    public string Building { get; set; } = null!;

    public decimal? CoordinateX { get; set; }

    public decimal? CoordinateY { get; set; }

    public virtual Shop? Shop { get; set; }
}
