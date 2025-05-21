using Dynamsoft.DBR;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using Dynamsoft.CVR;
using BarcodeReader.Controls;
using System.Collections.ObjectModel;
namespace BarcodeReader;

public partial class CameraPage : ContentPage
{
    public ObservableCollection<string> CamLinks { get; set; } = new();
    private static object _lockObject = new object();
    private int imageWidth;
    private int imageHeight;
    private bool isCameraStoped = false;
    private CapturedResult? result;

    public CameraPage()
    {
        InitializeComponent();
    }

    private async void OnLinkTapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is string url && Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            await Launcher.OpenAsync(uri);
        }
       ((ListView)sender).SelectedItem = null;
    }

    private void OnStopButtonClicked(object sender, EventArgs e)
    {
        isCameraStoped = true;
        CameraView.StopPreview();
    }

    private void OnStartButtonClicked(object sender, EventArgs e)
    {
        isCameraStoped= false;
        CamLinks.Clear();
        CameraView.StartPreview();
    }

    void OnOverlayPaint(object sender, SKPaintSurfaceEventArgs args)
    {
        SKImageInfo info = args.Info;
        SKSurface surface = args.Surface;
        SKCanvas canvas = surface.Canvas;
        canvas.Clear();
        CamLinks.Clear();
        
        float textSize = 18;
        float StrokeWidth = 2;
        SKPaint skPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Blue,
            StrokeWidth = StrokeWidth,
        };

        SKPaint textPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Red,
            StrokeWidth = StrokeWidth,
        };

        SKFont font = new SKFont() { Size = textSize };

        canvas.DrawRect(0, 0, info.Width, info.Height, skPaint);

        lock (_lockObject)
        {
            if (result == null) { return; }

            DecodedBarcodesResult? barcodesResult = result.GetDecodedBarcodesResult();
            if (barcodesResult != null)
            {
                BarcodeResultItem[] items = barcodesResult.GetItems();
                foreach (BarcodeResultItem barcodeItem in items)
                {
                    Dynamsoft.Core.Point[] points = barcodeItem.GetLocation().points;
                    canvas.DrawText(barcodeItem.GetText(), points[0][0], points[0][1], SKTextAlign.Left, font, textPaint);
                    canvas.DrawLine(points[0][0], points[0][1], points[1][0], points[1][1], skPaint);
                    canvas.DrawLine(points[1][0], points[1][1], points[2][0], points[2][1], skPaint);
                    canvas.DrawLine(points[2][0], points[2][1], points[3][0], points[3][1], skPaint);
                    canvas.DrawLine(points[3][0], points[3][1], points[0][0], points[0][1], skPaint);
                    if (isCameraStoped == true)
                        CamLinks.Add(barcodeItem.GetText());
                }
            }
        }
    }

    private void OnResultReady(object sender, ResultReadyEventArgs e)
    {
        lock (_lockObject)
        {
            imageWidth = e.PreviewWidth;
            imageHeight = e.PreviewHeight;
            result = e.Result;
            Overlay.InvalidateSurface();
        }
    }
}