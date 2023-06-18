
namespace ShopEF;

public partial class Discount
{
    public long Id { get; set; }

    public long ProductId { get; set; }

    public long? Discount1 { get; set; }

    public virtual Product Product { get; set; } = null!;
}
