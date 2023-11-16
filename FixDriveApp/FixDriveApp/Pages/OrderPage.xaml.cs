using System.Collections.ObjectModel;
using FixDriveApp.Models;
using FixDriveApp.Services;
using Microsoft.EntityFrameworkCore;


namespace FixDriveApp;

public partial class OrderPage : ContentPage
{
    private ApplicationContext db = new ApplicationContext();

    public static DateTime Today { get; } = DateTime.Now.Date;

    private Product GetProductById(int ID) =>
       db.Products.Include(s => s.Supplier).FirstOrDefault(p => p.ProductId == ID);

    public ObservableCollection<Grouping<string, Product>> ProductGroups { get; set; }


    public OrderPage()
	{
        InitializeComponent();
        LoadInventoryData();
    }
    private void LoadInventoryData()
    {
        List<Product> products = db.Products.Include(p => p.Supplier).ToList();
        List<Supplier> suppliers = db.Suppliers.Include(s => s.Products).ToList();
        List<Order> orders = db.Orders.Include(o => o.Product).ToList();

        var groups = products.GroupBy(p => p.Supplier.SupplierName).Select(g => new Grouping<string, Product>(g.Key, g));
        ProductGroups = new ObservableCollection<Grouping<string, Product>>(groups);
        collectionViewProductGroups.ItemsSource = ProductGroups;
        collectionViewOrder.ItemsSource = orders;        
    }

    void collectionViewProductGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Product currentProduct = e.CurrentSelection.FirstOrDefault() as Product;

        if (currentProduct != null)
        {
            selectedItemLabelProduct.Text = currentProduct.ProductName;
        }
    }

    void Button_Clicked_Submit(object sender, EventArgs e)
    {

        Product selectedItem = GetProductById((collectionViewProductGroups.SelectedItem as Product).ProductId) ;

        if (ValidateInputs())
        {
            db.Orders.Add(new Order
            {
                OrderDate = Today.ToShortDateString(),
                Quantity = int.Parse(selectedItemEntryQuantity.Text),
                ProductId = selectedItem.ProductId,
                SupplierId = selectedItem.SupplierId,
                Product = selectedItem
            }) ;

            selectedItem.Quantity += int.Parse(selectedItemEntryQuantity.Text);

            db.SaveChanges();
        }

        LoadInventoryData();

        selectedItemEntryQuantity.Text = string.Empty;
        selectedItemLabelProduct.Text = string.Empty;
        collectionViewProductGroups.SelectedItem = null;
    }

    private bool ValidateInputs()
    {
        if (!Utils.IsValidQuantity(selectedItemEntryQuantity.Text))
        {
            DisplayAlert("Error", "Invalid quantity", "OK");
            return false;

        }

        if (!Utils.IsValidString(selectedItemLabelProduct.Text))
        {
            DisplayAlert("Error", "Invalid product", "OK");
            return false;
        }

        return true;
    }

        private async void OnNavigateMainButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MainPage());
    }
}
