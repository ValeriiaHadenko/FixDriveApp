using FixDriveApp.Models;
using FixDriveApp.Services;
using Microsoft.EntityFrameworkCore;

namespace FixDriveApp;

public partial class SupplierPage : ContentPage
{
    private ApplicationContext db = new ApplicationContext();

    private bool isEdit { get; set; } = false;

    private Supplier GetSupplierById(int ID) => db.Suppliers.Include(s => s.Products).FirstOrDefault(s => s.SupplierId == ID);

    public SupplierPage()
	{
		InitializeComponent();
        LoadInventoryData();
    }

    private async void OnNavigateMainButtonClicked(object sender, System.EventArgs e)
    {
        await Navigation.PushAsync(new MainPage());
    }

    private void LoadInventoryData()
    {
        List<Product> products = db.Products.Include(p => p.Supplier).ToList();
        List<Supplier> suppliers = db.Suppliers.Include(s => s.Products).ToList();
        supplierListView.ItemsSource = suppliers;
    }

    private void UpdateEntryValue(Supplier selectedItem)
    {
        if (selectedItem != null)
        {
            selectedItemEntryID.Text = $"{selectedItem.SupplierId}";
            selectedItemEntryName.Text = $"{selectedItem.SupplierName}";
            selectedItemEntryMobile.Text = $"{selectedItem.Mobile}";
            selectedItemEntryEmail.Text = $"{selectedItem.Email}";
            inventoryListView.ItemsSource = selectedItem.Products;
        }
    }

    private void ToggleEntryIsReadOnly(bool isReadOnly)
    {
        selectedItemEntryName.IsReadOnly = isReadOnly;
        selectedItemEntryMobile.IsReadOnly = isReadOnly;
        selectedItemEntryEmail.IsReadOnly = isReadOnly;
        StackLayoutProducts.IsVisible = isReadOnly;
    }

    private void ToggleButtonsVisibility(bool btnEdit = false, bool btnSave = false, bool btnDelete = false, bool btnAdd = true, bool btnCreateSupplier = false)
    {
        this.btnEdit.IsVisible = btnEdit;
        this.btnSave.IsVisible = btnSave;
        this.btnDelete.IsVisible = btnDelete;
        this.btnAdd.IsVisible = btnAdd;
        this.btnCreateSupplier.IsVisible = btnCreateSupplier;
    }

    private bool ValidateInputs()
    {
        if (!Utils.IsValidString(selectedItemEntryName.Text))
        {
            DisplayAlert("Error", "Empty string", "OK");
            return false;
        }

        if (!Utils.IsValidPhoneNumber(selectedItemEntryMobile.Text))
        {
            DisplayAlert("Error", "Invalid phone number", "OK");
            return false;
        }

        if (!Utils.IsValidEmail(selectedItemEntryEmail.Text))
        {
            DisplayAlert("Error", "Invalid email address", "OK");
            return false;
        }

        return true;
    }

    void supplierListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        border.Stroke = Color.FromHex("#6f3799");
        border.IsVisible = true;

        isEdit = false;
        supplierListViewChange.IsVisible = true;
        StackLayoutIdProduct.IsVisible = true;
        ToggleEntryIsReadOnly(true);
        UpdateEntryValue(e.SelectedItem as Supplier);
        ToggleButtonsVisibility(btnEdit: true, btnDelete: true);
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
            UpdateEntryValue(supplierListView.SelectedItem as Supplier);
            ToggleButtonsVisibility(btnEdit: true, btnDelete: true);
        };
    }

    void btnSave_Clicked(object sender, EventArgs e)
    {
        Supplier selectedSupplier = GetSupplierById(int.Parse(selectedItemEntryID.Text));

        if (selectedSupplier != null && ValidateInputs())
        {
            selectedSupplier.SupplierName = selectedItemEntryName.Text;
            selectedSupplier.Mobile = selectedItemEntryMobile.Text;
            selectedSupplier.Email = selectedItemEntryEmail.Text;

            db.SaveChangesAsync();  
        }

        isEdit = false;

        border.Stroke = Color.FromHex("#6f3799");
        LoadInventoryData();
        ToggleEntryIsReadOnly(true);
        UpdateEntryValue(supplierListView.SelectedItem as Supplier);
        ToggleButtonsVisibility(btnEdit: true, btnDelete: true);
    }

    void btnDelete_Clicked(object sender, EventArgs e)
    {
        Supplier selectedSupplier = GetSupplierById(int.Parse(selectedItemEntryID.Text));

        if (selectedSupplier != null)
        {
            db.Suppliers.Remove(selectedSupplier);
            db.SaveChangesAsync();
        }

        LoadInventoryData();
        supplierListViewChange.IsVisible = false;
        ToggleButtonsVisibility();
        border.IsVisible = false;
    }

    void btnAdd_Clicked(object sender, EventArgs e)
    {
        border.IsVisible = true;

        supplierListView.SelectedItem = null;
        supplierListViewChange.IsVisible = true;
        inventoryListView.ItemsSource = null;

        StackLayoutProducts.IsVisible = false;
        StackLayoutIdProduct.IsVisible = false;

        selectedItemEntryID.Text = string.Empty;
        selectedItemEntryName.Text = string.Empty;
        selectedItemEntryMobile.Text = string.Empty;
        selectedItemEntryEmail.Text = string.Empty;

        ToggleEntryIsReadOnly(false);
        ToggleButtonsVisibility(btnCreateSupplier: true);
        border.Stroke = Color.FromHex("#ffffff");
    }

    void btnCreateSupplier_Clicked(object sender, EventArgs e)
    {
        if (ValidateInputs())
        {
            db.Suppliers.Add(new Supplier
            {
                SupplierName = selectedItemEntryName.Text,
                Mobile = selectedItemEntryMobile.Text,
                Email = selectedItemEntryEmail.Text
            });

            db.SaveChanges();
        }

        LoadInventoryData();
        supplierListViewChange.IsVisible = false;
        ToggleButtonsVisibility();
        border.IsVisible = false;
    }

    void searchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        string searchText = e.NewTextValue?.ToLower();

        if (string.IsNullOrWhiteSpace(searchText))
        {
            supplierListView.ItemsSource = db.Suppliers.Include(s => s.Products).ToList();
        }

        else
        {
            supplierListView.ItemsSource = (from supplier in db.Suppliers.Include(s => s.Products)
                where supplier.SupplierName.ToLower().Contains(searchText)
                select supplier).ToList();
        }
    }

    void selectedItemEntryMobile_TextChanged(object sender, TextChangedEventArgs e)
    {
        selectedItemEntryMobile.TextChanged -= selectedItemEntryMobile_TextChanged;

        string cleanedText = new string(e.NewTextValue?.Where(char.IsDigit).ToArray() ?? Array.Empty<char>());

        if (cleanedText.Length > 10)
        {
            cleanedText = cleanedText.Substring(0, 10);
        }

        string formattedText = Utils.FormatPhoneNumber(cleanedText);

        selectedItemEntryMobile.Text = formattedText;

        selectedItemEntryMobile.TextChanged += selectedItemEntryMobile_TextChanged;
    } 
}
