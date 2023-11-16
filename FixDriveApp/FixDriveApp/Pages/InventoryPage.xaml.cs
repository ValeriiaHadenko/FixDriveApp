using FixDriveApp.Models;
using FixDriveApp.Services;
using Microsoft.EntityFrameworkCore;

namespace FixDriveApp;

public partial class InventoryPage : ContentPage
{
    private ApplicationContext db = new ApplicationContext();

    private bool isEdit { get; set; } = false;

    private Product GetProductById(int ID) => db.Products.Include(s => s.Supplier).FirstOrDefault(s => s.ProductId == ID );

    public InventoryPage()
    {
        InitializeComponent();
        LoadInventoryData();
    }

    private async void OnNavigateBackButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnNavigateSupplierButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SupplierPage()) ;
    }

    private async void OnNavigateMakeOrderButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new OrderPage());
    }

    private void LoadInventoryData()
    {
        List<Product> products = db.Products.Include(p => p.Supplier).ToList();
        List<Supplier> suppliers = db.Suppliers.Include(s => s.Products).ToList();

        inventoryListView.ItemsSource = products;
        supplierListView.ItemsSource = suppliers;
    }

    private void UpdateEntryValue(Product selectedItem) {
        if (selectedItem != null)
        {
            selectedItemEntryID.Text = $"{selectedItem.ProductId}";
            selectedItemEntryName.Text = $"{selectedItem.ProductName}";
            selectedItemEntryModel.Text = $"{selectedItem.Model}";
            selectedItemEntryDescription.Text = $"{selectedItem.Description}";
            selectedItemEntryPrice.Text = $"{selectedItem.Price}";
            selectedItemEntryQuantity.Text = $"{selectedItem.Quantity}";
            selectedItemEntrySupplierName.Text = $"{selectedItem.Supplier.SupplierName}";
            supplierListView.SelectedItem = selectedItem.Supplier;
        }
    }

    private void ToggleEntryIsReadOnly(bool isReadOnly)
    {
        selectedItemEntryName.IsReadOnly = isReadOnly;
        selectedItemEntryModel.IsReadOnly = isReadOnly;
        selectedItemEntryDescription.IsReadOnly = isReadOnly;
        selectedItemEntryPrice.IsReadOnly = isReadOnly;
        selectedItemEntryQuantity.IsReadOnly = isReadOnly;
        selectedItemEntrySupplierName.IsVisible = isReadOnly;

        supplierListView.IsVisible = selectedItemEntrySupplierName.IsVisible ? false : true;
    }

    private void ToggleButtonsVisibility( bool btnEdit = false, bool btnSave = false, bool btnDelete = false, bool btnAdd = true, bool btnCreateProduct = false)
    {
        this.btnEdit.IsVisible = btnEdit;
        this.btnSave.IsVisible = btnSave;
        this.btnDelete.IsVisible = btnDelete;
        this.btnAdd.IsVisible = btnAdd;
        this.btnCreateProduct.IsVisible = btnCreateProduct;
    }

    private bool ValidateInputs()
    {
        if (!Utils.IsValidString(selectedItemEntryName.Text)
            || !Utils.IsValidString(selectedItemEntryModel.Text)
            || !Utils.IsValidString(selectedItemEntryDescription.Text))
        {
            DisplayAlert("Error", "Empty string", "OK");
            return false;
        }

        if (!Utils.IsValidQuantity(selectedItemEntryQuantity.Text))
        {
            DisplayAlert("Error", "Invalid quantity", "OK");
            return false;

        }

        if (!Utils.IsValidPrice(selectedItemEntryPrice.Text))
        {
            DisplayAlert("Error", "Invalid price", "OK");
            return false;

        }

        if (supplierListView.SelectedItem as Supplier == null)
        {
            DisplayAlert("Error", "Invalid supplier", "OK");
            return false;
        }

        return true;
    }

    void inventoryListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        border.Stroke = Color.FromHex("#6f3799");
        border.IsVisible = true;
        isEdit = false;
        inventoryListViewChange.IsVisible = true;
        StackLayoutIdProduct.IsVisible = true;
        ToggleEntryIsReadOnly(true);
        UpdateEntryValue(e.SelectedItem as Product);
        ToggleButtonsVisibility(btnEdit:true, btnDelete:true);
    }

    void supplierListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        Supplier selectedSupplier = e.SelectedItem as Supplier;

        if (isEdit && selectedSupplier != null)
        {
            selectedItemEntrySupplierName.Text = $"{selectedSupplier.SupplierName}";
        }
    }

    void btnEdit_Clicked(object sender, EventArgs e)
    {
        isEdit = !isEdit;

        if (isEdit)
        {
            border.Stroke = Color.FromHex("#0d6efd");
            ToggleEntryIsReadOnly(false);
            ToggleButtonsVisibility(btnSave: true, btnEdit: true, btnDelete: true);
        }
        else
        {
            border.Stroke = Color.FromHex("#6f3799");
            ToggleEntryIsReadOnly(true);
            UpdateEntryValue(inventoryListView.SelectedItem as Product);
            ToggleButtonsVisibility(btnEdit: true, btnDelete: true);
        };
    }

    void btnSave_Clicked(object sender, EventArgs e)
    {
        Product selectedItem = GetProductById(int.Parse(selectedItemEntryID.Text));
 
        if (selectedItem != null && ValidateInputs())
        {
            selectedItem.ProductName = selectedItemEntryName.Text;
            selectedItem.Model = selectedItemEntryModel.Text;
            selectedItem.Description = selectedItemEntryDescription.Text;
            selectedItem.Price = double.Parse(selectedItemEntryPrice.Text);
            selectedItem.Quantity = int.Parse(selectedItemEntryQuantity.Text);
            selectedItem.Supplier = supplierListView.SelectedItem as Supplier;

            db.SaveChangesAsync();
        }
        
        isEdit = false;

        border.Stroke = Color.FromHex("#6f3799");
        LoadInventoryData();
        ToggleEntryIsReadOnly(true);
        UpdateEntryValue(inventoryListView.SelectedItem as Product);
        ToggleButtonsVisibility(btnEdit: true, btnDelete: true);
    }

    void btnDelete_Clicked(object sender, EventArgs e)
    {
        Product selectedItem = GetProductById(int.Parse(selectedItemEntryID.Text));
        if (selectedItem != null)
        {
            db.Products.Remove(selectedItem);
            db.SaveChangesAsync();
        }

        LoadInventoryData();
        inventoryListViewChange.IsVisible = false;
        ToggleButtonsVisibility();
        border.IsVisible = false;
    }

    void btnAdd_Clicked(object sender, EventArgs e)
    {
        border.IsVisible = true;

        inventoryListViewChange.IsVisible = true;

        inventoryListView.SelectedItem = null;
        supplierListView.SelectedItem = null;

        StackLayoutIdProduct.IsVisible = false;

        selectedItemEntryID.Text = string.Empty;
        selectedItemEntryName.Text = string.Empty;
        selectedItemEntryModel.Text = string.Empty;
        selectedItemEntryDescription.Text = string.Empty;
        selectedItemEntryPrice.Text = string.Empty;
        selectedItemEntryQuantity.Text = string.Empty;
        selectedItemEntrySupplierName.Text = string.Empty;

        ToggleEntryIsReadOnly(false);
        ToggleButtonsVisibility(btnCreateProduct:true);
        border.Stroke = Color.FromHex("#ffffff");

    }

    void btnCreateProduct_Clicked(object sender, EventArgs e)
    {
        if (ValidateInputs())
        {
            db.Products.Add(new Product
            {
                ProductName = selectedItemEntryName.Text,
                Model = selectedItemEntryModel.Text,
                Description = selectedItemEntryDescription.Text,
                Price = double.Parse(selectedItemEntryPrice.Text),
                Quantity = int.Parse(selectedItemEntryQuantity.Text),
                Supplier = supplierListView.SelectedItem as Supplier
            });

            db.SaveChanges();
        }

        LoadInventoryData();
        inventoryListViewChange.IsVisible = false;
        ToggleButtonsVisibility();
        border.IsVisible = false;
    }

    void searchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        string searchText = e.NewTextValue?.ToLower();

        if (string.IsNullOrWhiteSpace(searchText))
        {
            inventoryListView.ItemsSource = db.Products.Include(p => p.Supplier).ToList();
        }

        else
        {
            inventoryListView.ItemsSource = (from product in db.Products.Include(p => p.Supplier)
                where product.ProductName.ToLower().Contains(searchText)
                select product).ToList();
        }
    }

}
