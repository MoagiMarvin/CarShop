using SQLite;


namespace CarShop.Model
{
    public class Car
    {
        [PrimaryKey, AutoIncrement]
        public int CarId { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string Color { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int StockQuantity { get; set; }
        public string ImageUrl { get; set; }
    }
}