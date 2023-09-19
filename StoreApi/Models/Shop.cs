using System;
using System.Collections.Generic;

namespace StoreApi.Models;

public partial class Shop
{
    public int Id { get; set; }

    public string OwnerName { get; set; } = null!;

    public string ShopName { get; set; } = null!;

    public string PhoneNumber { get; set; }

    public string Password { get; set; } = null!;

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<ShopAddress> ShopAddresses { get; set; } = new List<ShopAddress>();
}
