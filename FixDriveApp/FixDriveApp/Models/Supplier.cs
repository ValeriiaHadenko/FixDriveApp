namespace FixDriveApp.Models
{
    public class Supplier
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public List<Product> Products { get; set; } = new();
    }
}