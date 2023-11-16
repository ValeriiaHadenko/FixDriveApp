namespace FixDriveApp.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Model { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }
    }
}
