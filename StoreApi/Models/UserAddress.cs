using System;
using System.Collections.Generic;

namespace StoreApi.Models;

public partial class UserAddress
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Name { get; set; } = null!;

    public string PhoneNumber { get; set; }

    public string City { get; set; } = null!;

    public int Pincode { get; set; }

    public string StreetName { get; set; } = null!;

    public string Building { get; set; } = null!;

    public decimal? CoordinateX { get; set; }

    public decimal? CoordinateY { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual User? User { get; set; }
}
