using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using SCME.Types.SL;
using SCME.WpfControlLibrary.Properties;

namespace SCME.WpfControlLibrary.IValueConverters
{
    public class VtmConverters
    {
        public class VtmTestTypeToVisibilityConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                var type = (SLTestType)value;
                int index = int.Parse((string)parameter);

                if ((type == SLTestType.Ramp && index == 0) || (type == SLTestType.Sinus && index == 1) ||
                    (type == SLTestType.Curve && index == 2))
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new InvalidOperationException("ConvertBack method is not implemented in VtmTestTypeToVisibilityConverter");
            }
        }
        
        public class VtmTestTypeToStringConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                var type = (SLTestType)value;
                var line = string.Empty;

                switch (type)
                {
                
                    case SLTestType.Ramp:
                        line = Resources.Ramp;
                        break;
                    case SLTestType.Sinus:
                        line = Resources.Sinus;
                        break;
                    case SLTestType.Curve:
                        line = Resources.Curve;
                        break;
                }

                return line;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                var type = SLTestType.Ramp;
                var line = (string)value;

                if (line == Resources.Ramp)
                    type = SLTestType.Ramp;
                else if (line == Resources.Sinus)
                    type = SLTestType.Sinus;
                else if (line == Resources.Curve)
                    type = SLTestType.Curve;

                return type;
            }
        }
    }
}