using System;
using System.Collections.Generic;

namespace StoreApi.Models;

public partial class Cart
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int ShopId { get; set; }

    public virtual ICollection<ProductCart> ProductCarts { get; set; } = new List<ProductCart>();

    public virtual Shop? Shop { get; set; }

    public virtual User? User { get; set; }
}
