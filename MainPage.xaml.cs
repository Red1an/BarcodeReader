using System.Diagnostics;

namespace BarcodeReader
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnFileButtonClicked(object sender, EventArgs e)
        {
            try
            {

                FileResult? photo = await FilePicker.PickAsync();

                await LoadPhotoAsync(photo);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"CapturePhotoAsync THREW: {ex.Message}");
            }
        }

        private async void OnCameraButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CameraPage());
        }

        async Task LoadPhotoAsync(FileResult? photo)
        {
            if (photo == null)
            {
                return;
            }

            await Navigation.PushAsync(new PicturePage(photo.FullPath));
        }

    }
}