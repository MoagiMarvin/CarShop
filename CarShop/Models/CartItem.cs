using SQLite;


namespace CarShop.Model
{
    public class CartItem
{
    [PrimaryKey, AutoIncrement]
    public int CartItemId { get; set; }

    [Indexed]
    public int ProfileId { get; set; }

    [Indexed]
    public int CarId { get; set; }

    public int Quantity { get; set; }
    public decimal PriceAtTime { get; set; }  // Store price at time of adding to cart
    public DateTime AddedDate { get; set; }

    // Navigation properties (not stored in DB, used for data relations)
    [Ignore]
    public Car Car { get; set; }
}
}