﻿namespace ShopEF.Models;
public class Discount : EntityBase
{
    public int ProductId { get; set; }
    public double? Disc { get; set; }

    public Product Product { get; set; } = null!;

    public Discount(int productId, double? disc)
    {
        ProductId = productId;
        Disc = disc;
    }
}
