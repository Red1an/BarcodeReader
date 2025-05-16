namespace BarcodeReader;

public partial class CameraPage : ContentPage
{
	public CameraPage()
	{
		InitializeComponent();
        NavigationPage MainPage = new NavigationPage(new MainPage());
    }
}