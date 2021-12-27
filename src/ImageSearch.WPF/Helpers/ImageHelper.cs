using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageSearch.WPF.Helpers
{
    internal static class ImageHelper
    {
        internal static BitmapSource ScaleImage(BitmapSource source, double scale)
        {
            TransformedBitmap transformed = new TransformedBitmap();
            transformed.BeginInit();
            transformed.Source = source;
            transformed.Transform = new ScaleTransform(scale, scale);
            transformed.EndInit();

            if (transformed.CanFreeze)
            {
                transformed.Freeze();
            }

            return transformed;
        }

        internal static void SaveImage(BitmapSource source, Stream stream)
        {
            BitmapEncoder encoder = new PngBitmapEncoder();

            encoder.Frames.Add(BitmapFrame.Create(source));
            encoder.Save(stream);
        }
    }
}
