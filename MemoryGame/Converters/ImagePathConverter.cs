using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace MemoryGame.Converters
{
    public class ImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string imagePath && !string.IsNullOrEmpty(imagePath))
            {
                try
                {
                    if (!Path.IsPathRooted(imagePath))
                    {
                        imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, imagePath);
                    }

                    if (File.Exists(imagePath))
                    {
                        return new BitmapImage(new Uri(imagePath));
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}