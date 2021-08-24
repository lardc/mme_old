using SCME.Types.SL;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SCME.WpfControlLibrary.IValueConverters
{
    public class VtmTestTypeToVisibilityConverter : IValueConverter
    {
        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            var type = (SLTestType)Value;
            int index = int.Parse((string)Parameter);

            if ((type == SLTestType.Ramp && index == 0) || (type == SLTestType.Sinus && index == 1) ||
                (type == SLTestType.Curve && index == 2))
                return Visibility.Visible;

            return Visibility.Collapsed;
        }

        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            throw new InvalidOperationException("ConvertBack method is not implemented in VtmTestTypeToVisibilityConverter");
        }
    }
}
