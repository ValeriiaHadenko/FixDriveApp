namespace FixDriveApp;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	private async void OnNavigateButtonClicked(object sender, System.EventArgs e)
    {
        await Navigation.PushAsync(new InventoryPage());
    }
}


