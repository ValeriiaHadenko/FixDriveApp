namespace FixDriveApp.Models
{
	public class Order
	{
        public int OrderId { get; set; }
        public string OrderDate { get; set; }
        public int Quantity { get; set; }
        public int ProductId { get; set; }
        public int SupplierId { get; set; }
        public Product Product { get; set; }
    }
}

