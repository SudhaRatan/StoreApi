using System;
using System.Collections.Generic;

namespace StoreApi.Models;

public partial class Order
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int AddressId { get; set; }

    public int ShopId { get; set; }

    public string Status { get; set; } = null!;

    public virtual UserAddress? Address { get; set; }

    public virtual ICollection<ProductOrder> ProductOrders { get; set; } = new List<ProductOrder>();

    public virtual Shop? Shop { get; set; }

    public virtual User? User { get; set; }
}
