using SkiaSharp;
using SkiaSharp.Views.Maui;
using Dynamsoft.CVR;
using Dynamsoft.DBR;
using SkiaSharp.Views.Maui.Controls;
using Dynamsoft.Core;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Collections.ObjectModel;

namespace BarcodeReader;

public partial class PicturePage : ContentPage
{
    public ObservableCollection<string> AllLinks { get; set; } = new();
    SKBitmap? bitmap;
    private CaptureVisionRouter cvr = new CaptureVisionRouter();
    CapturedResult? result;
    bool isDataReady = false;

    public PicturePage(string imagepath)
    {
        Debug.WriteLine("�������� PicturePage...");
        InitializeComponent();
        BindingContext = this;
        try
        {
            bitmap = LoadAndCorrectOrientation(imagepath);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        DecodeFile(imagepath);
    }
    private async void OnLinkTapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is string url && Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            await Launcher.OpenAsync(uri);
        }
        ((ListView)sender).SelectedItem = null;
    }

    SKBitmap? LoadAndCorrectOrientation(string imagePath)
    {
        Debug.WriteLine("������ LoadAndCorrectOrientation.....");

        if (!File.Exists(imagePath))
        {
            Debug.WriteLine("���� �� ������: " + imagePath);
            return null;
        }

        using var stream = new SKFileStream(imagePath);
        using var codec = SKCodec.Create(stream);

        SKBitmap bitmap = SKBitmap.Decode(codec);

        var origin = codec.EncodedOrigin;
        if (origin == SKEncodedOrigin.TopLeft)
        {
            return bitmap;
        }

        SKMatrix matrix = SKMatrix.CreateIdentity();
        int rotatedWidth = bitmap.Width;
        int rotatedHeight = bitmap.Height;

        switch (origin)
        {
            case SKEncodedOrigin.RightTop:
                matrix = SKMatrix.CreateRotationDegrees(90, 0, 0);
                rotatedWidth = bitmap.Height;
                rotatedHeight = bitmap.Width;
                break;
            case SKEncodedOrigin.BottomRight:
                matrix = SKMatrix.CreateRotationDegrees(180, 0, 0);
                break;
            case SKEncodedOrigin.LeftBottom:
                matrix = SKMatrix.CreateRotationDegrees(270, 0, 0);
                rotatedWidth = bitmap.Height;
                rotatedHeight = bitmap.Width;
                break;
            default:
                break;
        }

        SKBitmap rotatedBitmap = new SKBitmap(rotatedWidth, rotatedHeight);
        using (var surface = new SKCanvas(rotatedBitmap))
        {
            switch (origin)
            {
                case SKEncodedOrigin.RightTop:
                    surface.Translate(rotatedWidth, 0);
                    break;
                case SKEncodedOrigin.BottomRight:
                    surface.Translate(rotatedWidth, rotatedHeight);
                    break;
                case SKEncodedOrigin.LeftBottom:
                    surface.Translate(0, rotatedHeight);
                    break;
            }
            surface.Concat(matrix);
            surface.DrawBitmap(bitmap, 0, 0);
        }

        return rotatedBitmap;
    }

    async void DecodeFile(string imagepath)
    {
        try
        {
            result = await Task.Run(() =>
                cvr.Capture(imagepath, PresetTemplate.PT_READ_BARCODES)
            );
            isDataReady = true;

            MainThread.BeginInvokeOnMainThread(() =>
            {
                canvasView.InvalidateSurface();
            });
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"DecodeFile error: {ex}");
        }
    }

    void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
    {
        Debug.WriteLine("������ OnCanvasViewPaintSurface...");
        if (!isDataReady)
        {
            return;
        }
        SKImageInfo info = args.Info;
        SKSurface surface = args.Surface;
        SKCanvas canvas = surface.Canvas;
        canvas.Clear();
        AllLinks.Clear();

        if (bitmap != null)
        {
            var imageCanvas = new SKCanvas(bitmap);

            float textSize = 18;
            float StrokeWidth = 2;

            if (DeviceInfo.Current.Platform == DevicePlatform.Android || DeviceInfo.Current.Platform == DevicePlatform.iOS)
            {
                textSize = (float)(18 * DeviceDisplay.MainDisplayInfo.Density);
                StrokeWidth = 4;
            }

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
            if (isDataReady)
            {
                if (result != null)
                {
                    DecodedBarcodesResult? barcodesResult = result.GetDecodedBarcodesResult();
                    if (barcodesResult != null)
                    {
                        BarcodeResultItem[] items = barcodesResult.GetItems();
                        foreach (BarcodeResultItem barcodeItem in items)
                        {

                            Dynamsoft.Core.Point[] points = barcodeItem.GetLocation().points;
                            imageCanvas.DrawText(barcodeItem.GetText(), points[0][0], points[0][1], SKTextAlign.Left, font, textPaint);

                            imageCanvas.DrawLine(points[0][0], points[0][1], points[1][0], points[1][1], skPaint);
                            imageCanvas.DrawLine(points[1][0], points[1][1], points[2][0], points[2][1], skPaint);
                            imageCanvas.DrawLine(points[2][0], points[2][1], points[3][0], points[3][1], skPaint);
                            imageCanvas.DrawLine(points[3][0], points[3][1], points[0][0], points[0][1], skPaint);
                            AllLinks.Add(barcodeItem.GetText());
                        }
                    }
                }
                else
                {
                    AllLinks.Add("No 1D/2D barcode found");
                }
            }

            float scale = Math.Min((float)info.Width / bitmap.Width,
                               (float)info.Height / bitmap.Height);
            float x = (info.Width - scale * bitmap.Width) / 2;
            float y = (info.Height - scale * bitmap.Height) / 2;
            SKRect destRect = new SKRect(x, y, x + scale * bitmap.Width,
                                               y + scale * bitmap.Height);

            canvas.DrawBitmap(bitmap, destRect);
        }
    }
}