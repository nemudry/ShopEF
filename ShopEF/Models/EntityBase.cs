using System.ComponentModel.DataAnnotations;

namespace ShopEF.Models;

public abstract class EntityBase
{
    public int Id { get; set; }

    [Timestamp]
    internal byte[] Timestamp { get; set; }
}
