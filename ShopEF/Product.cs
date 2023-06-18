
namespace ShopEF;

public partial class Product
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Category { get; set; }

    public string? Description { get; set; }

    public string? Made { get; set; }

    public long Price { get; set; }

    public virtual Discount? Discount { get; set; }

    public virtual MscStorehouse? MscStorehouse { get; set; }

    public virtual NnStorehouse? NnStorehouse { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
