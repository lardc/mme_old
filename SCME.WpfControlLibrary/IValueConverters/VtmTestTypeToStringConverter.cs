using SCME.Types.SL;
using SCME.WpfControlLibrary.Properties;
using System;
using System.Globalization;
using System.Windows.Data;

namespace SCME.WpfControlLibrary.IValueConverters
{
    public class VtmTestTypeToStringConverter : IValueConverter
    {
        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            var type = (SLTestType)Value;
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

        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            var type = SLTestType.Ramp;
            var line = (string)Value;

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
