using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Bas.EuroSing.ScoreBoard.Extensions
{
    internal static class ByteArrayExtensionMethods
    {
        // Constructs a BitmapImage from a byte array
        public static BitmapImage ToBitmapImage(this byte[] byteArray)
        {
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.CreateOptions = BitmapCreateOptions.None;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.StreamSource = new MemoryStream(byteArray);
            bitmapImage.EndInit();

            return bitmapImage;
        }
    }
}
