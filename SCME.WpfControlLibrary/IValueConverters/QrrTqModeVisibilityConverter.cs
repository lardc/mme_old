using SCME.Types.QrrTq;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SCME.WpfControlLibrary.IValueConverters
{
    public class QrrTqModeVisibilityConverter : IValueConverter
    {
        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            TMode Mode = (TMode)Value;
            string Name = Parameter.ToString();

            if (Mode == TMode.Qrr)
            {
                switch (Name)
                {
                    case "TrrMeasureBy9050Method":
                        return Visibility.Visible;

                    case "OffStateVoltage":
                        return Visibility.Collapsed;

                    case "OsvRate":
                        return Visibility.Collapsed;

                    case "Tq":
                        return Visibility.Collapsed;

                    //видны
                    case "Qrr":
                        return Visibility.Visible;

                    case "Irr":
                        return Visibility.Visible;

                    case "Trr":
                        return Visibility.Visible;

                    default:
                        return Visibility.Visible;
                }
            }
            else
            {
                if (Mode == TMode.QrrTq)
                {
                    switch (Name)
                    {
                        case "TrrMeasureBy9050Method":
                            return Visibility.Collapsed;

                        case "OffStateVoltage":
                            return Visibility.Visible;

                        case "OsvRate":
                            return Visibility.Visible;

                        case "Tq":
                            return Visibility.Visible;

                        //скрыты
                        case "Qrr":
                            return Visibility.Collapsed;

                        case "Irr":
                            return Visibility.Collapsed;

                        case "Trr":
                            return Visibility.Collapsed;

                        default:
                            return Visibility.Visible;
                    }
                }
                else
                    return Visibility.Visible;
            }
        }

        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            throw new InvalidOperationException("ConvertBack method is not implemented in QrrTqModeVisibilityConverter");
        }
    }
}
