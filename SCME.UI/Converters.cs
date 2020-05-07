﻿using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using SCME.Types.BVT;
using SCME.Types.Clamping;
using SCME.Types.Commutation;
using SCME.Types.dVdt;
using SCME.Types.Profiles;
using SCME.Types.VTM;
using SCME.Types.QrrTq;
using SCME.UIServiceConfig.Properties;
using TestParameters = SCME.Types.VTM.TestParameters;
using SCME.UI.Properties;

namespace SCME.UI
{
    public class MultiBvtRSMTestTypeToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] Values, Type TargetType, object Parameter, CultureInfo culture)
        {
            BVTTestType testType;
            bool parseResult = Enum.TryParse(Values[0]?.ToString(), out testType);
            if (!parseResult)
                return Visibility.Collapsed;

            bool useUdsmUrsm;
            parseResult = bool.TryParse(Values[1]?.ToString(), out useUdsmUrsm);
            if (!parseResult)
                return Visibility.Collapsed;

            int indexInEnum;
            parseResult = int.TryParse(Values[2]?.ToString(), out indexInEnum);
            if (!parseResult)
                return Visibility.Collapsed;

            bool result = (useUdsmUrsm) &&
                          (
                              testType == BVTTestType.Both ||
                              (testType == BVTTestType.Direct && indexInEnum == (int)BVTTestType.Direct) ||
                              (testType == BVTTestType.Reverse && indexInEnum == (int)BVTTestType.Reverse)
                          );

            return result ? Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object Value, Type[] TargetTypes, object Parameter, CultureInfo Culture)
        {
            throw new InvalidOperationException("ConvertBack method is not implemented in MultiBvtRSMTestTypeToVisibilityConverter");
        }
    }   
    
    public class BvtTestTypeToVisibilityConverter : IValueConverter
    {
        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            var en = (BVTTestType)Value;
            int index = int.Parse((string)Parameter);

            return en == BVTTestType.Both ||
                   en == BVTTestType.Direct && index == (int)BVTTestType.Direct ||
                   en == BVTTestType.Reverse && index == (int)BVTTestType.Reverse
                       ? Visibility.Visible
                       : Visibility.Collapsed;
        }

        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            throw new InvalidOperationException("ConvertBack method is not implemented in BvtTestTypeToVisibilityConverter");
        }
    }

    public class MeasureModeToVisibilityConverter : IValueConverter
    {
        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            var en = (BVTMeasurementMode)Value;
            var index = int.Parse((string)Parameter);

            return en == BVTMeasurementMode.ModeI && index == (int)BVTMeasurementMode.ModeI ||
                   en == BVTMeasurementMode.ModeV && index == (int)BVTMeasurementMode.ModeV
                       ? Visibility.Visible
                       : Visibility.Collapsed;
        }

        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            throw new InvalidOperationException("ConvertBack method is not implemented in MeasureModeToVisibilityConverter");
        }
    }

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

    public class ClampingForceToTextConverter : IValueConverter
    {
        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            var en = (ClampingForce)Value;

            switch (en)
            {
                case ClampingForce.Contact:
                    return en.ToString();
                default:
                    return en.ToString().TrimStart('F');
            }
        }

        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            var en = (string)Value;

            return (en == ClampingForce.Contact.ToString()) ? en : Enum.Parse(typeof(ClampingForce), "F" + en);
        }
    }

    public class VtmTestTypeToCurrentConverter : IValueConverter
    {
        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            var vPro = (TestParameters)Value;

            switch (vPro.TestType)
            {
                case VTMTestType.Ramp:
                    return vPro.RampCurrent;
                case VTMTestType.Sinus:
                    return vPro.SinusCurrent;
                case VTMTestType.Curve:
                    return vPro.CurveCurrent;
            }

            return 0;
        }

        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            throw new InvalidOperationException("ConvertBack method is not implemented in VtmTestTypeToCurrentConverter");
        }
    }

    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            return ((bool)Value ^ ((Parameter != null) && (string)Parameter == "!")) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            return (((Visibility)Value) != Visibility.Collapsed) ^ ((Parameter != null) && (string)Parameter == "!");
        }
    }


    /// <summary>
    /// Конвертор булевого в System.Windows.Visibility
    /// </summary>
    public class TryBooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Конвертирует bool в Visibility
        /// </summary>
        /// <param name="value">Конвертируемое значение</param>
        /// <returns></returns>
        public object ConvertWithoutCheckValue(object value)
        {
            switch ((bool)value)
            {
                case true:
                    return Visibility.Visible;
                case false:
                    return Visibility.Hidden;
                default:
                    throw new NotImplementedException($"{nameof(TryBooleanToVisibilityConverter)} {nameof(NotImplementedException)} error");
            }
        }

        /// <summary>
        ///Конвертирует Visibility в bool
        /// </summary>
        /// <param name="value">Конвертируемое значение</param>
        /// <returns></returns>
        public object ConvertBackWithoutCheckValue(object value)
        {
            switch ((Visibility)value)
            {
                case Visibility.Visible:
                    return true;
                case Visibility.Hidden:
                    return false;
                case Visibility.Collapsed:
                    return false;
                default:
                    throw new NotImplementedException($"{nameof(TryBooleanToVisibilityConverter)} {nameof(NotImplementedException)} error");
            }
        }
    
        /// <summary>
        /// Проверяет значение на null и на принадлежность к типу bool
        /// </summary>
        /// <param name="value">Проверяемое значение</param>
        public void ConvertCheck(object value)
        {
            if (value == null)
                throw new NullReferenceException("Value is null");
            if (typeof(bool).Equals(value.GetType()) == false)
                throw new InvalidCastException($"Value has type {value.GetType()} , need bool ");
        }

        /// <summary>
        /// Проверяет значение на null и на принадлежность к типу Visibility
        /// </summary>
        /// <param name="value">Проверяемое значение</param>
        public void ConvertBackCheck(object value)
        {
            if (value == null)
                throw new NullReferenceException("Value is null");
            if (typeof(Visibility).Equals(value.GetType()) == false)
                throw new InvalidCastException($"Value has type {value.GetType()} , need bool ");
        }

        /// <summary>
        /// Конвертортирует bool в System.Windows.Visibility
        /// </summary>
        /// <param name="value">Значение, произведенное исходной привязкой.</param>
        /// <param name="targetType">Тип целевого свойства привязки.</param>
        /// <param name="parameter">Используемый параметр преобразователя.</param>
        /// <param name="culture">Язык и региональные параметры, используемые в преобразователе.</param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ConvertCheck(value);
            return ConvertWithoutCheckValue(value);
        }

        /// <summary>
        /// Конвертортирует System.Windows.Visibility в bool
        /// </summary>
        /// <param name="value">Значение, произведенное исходной привязкой.</param>
        /// <param name="targetType">Тип целевого свойства привязки.</param>
        /// <param name="parameter">Используемый параметр преобразователя.</param>
        /// <param name="culture">Язык и региональные параметры, используемые в преобразователе.</param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ConvertBackCheck(value);
            return ConvertBackWithoutCheckValue(value);
        }

    }

    public class TryBooleanToVisibilityInverseConverter: IValueConverter
    {
        public TryBooleanToVisibilityConverter _Converter = new TryBooleanToVisibilityConverter();

        /// <summary>
        /// Инверсионно конвертортирует System.Windows.Visibility в bool
        /// </summary>
        /// <param name="value">Значение, произведенное исходной привязкой.</param>
        /// <param name="targetType">Тип целевого свойства привязки.</param>
        /// <param name="parameter">Используемый параметр преобразователя.</param>
        /// <param name="culture">Язык и региональные параметры, используемые в преобразователе.</param>
        /// <returns>Преобразованное значение</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            _Converter.ConvertCheck(value);
            bool inverseValue = !((bool)value);
            return _Converter.ConvertWithoutCheckValue(inverseValue);
        }

        /// <summary>
        /// Инверсионно конвертортирует System.Windows.Visibility в bool
        /// </summary>
        /// <param name="value">Значение, произведенное исходной привязкой.</param>
        /// <param name="targetType">Тип целевого свойства привязки.</param>
        /// <param name="parameter">Используемый параметр преобразователя.</param>
        /// <param name="culture">Язык и региональные параметры, используемые в преобразователе.</param>
        /// <returns>Преобразованное значение</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            _Converter.ConvertBackCheck(value);
            Visibility inverseValue = (Visibility)value == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            return _Converter.ConvertBackWithoutCheckValue(inverseValue);
        }
    }

    public class ListBoxDefaultItemToVisibilityConverter : IValueConverter
    {
        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            var item = Value as Profile;

            return item != null && item.Name == "_Default" ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            throw new InvalidOperationException("ConvertBack method is not implemented in ListBoxDefaultItemToVisibilityConverter");
        }
    }

    public class ListBoxDefaultItemToEnableConverter : IValueConverter
    {
        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            var item = Value as string;

            return item != null && item != "_Default";
        }

        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            throw new InvalidOperationException("ConvertBack method is not implemented in ListBoxDefaultItemToEnableConverter");
        }
    }
    public class ListBoxSelectedItemToVisibleConverter : IValueConverter
    {
        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            var item = Value as int?;

            return item != null && item != -1 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            throw new InvalidOperationException("ConvertBack method is not implemented in ListBoxDefaultItemToEnableConverter");
        }
    }

    public class TestTypeToIntConverter : IValueConverter
    {
        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            return (int)((VTMTestType)Value);
        }

        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            return (VTMTestType)((int)Value);
        }
    }

    public class MultiBooleanToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] Values, Type TargetType, object Parameter, CultureInfo Culture)
        {
            var visible = Values.OfType<bool>().Aggregate(true, (Current, Value) => Current && Value);

            return visible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object Value, Type[] TargetTypes, object Parameter, CultureInfo Culture)
        {
            throw new InvalidOperationException("ConvertBack method is not implemented in MultiBooleanToVisibilityConverter");
        }
    }

    public class StringEmptyToVisibilityConverter : IValueConverter
    {
        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            var str = (string)Value;

            return string.IsNullOrEmpty(str) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            return String.Empty;
        }
    }
    /*
    public class StringEmptyToNotVisibilityConverter : IValueConverter
    {
        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            var str = (string)Value;

            return !string.IsNullOrEmpty(str) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            return String.Empty;
        }
    }
    */

    public class GridLengthConverter : IValueConverter
    {
        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            return ((GridLength)Value).Value;
        }

        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            return new GridLength((double)Value);
        }
    }

    public class EnumBooleanConverter : IValueConverter
    {
        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            var parameterString = Parameter as string;
            if (parameterString == null)
                return DependencyProperty.UnsetValue;

            if (Enum.IsDefined(Value.GetType(), Value) == false)
                return DependencyProperty.UnsetValue;

            var parameterValue = Enum.Parse(Value.GetType(), parameterString);

            return parameterValue.Equals(Value);
        }

        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            var parameterString = Parameter as string;

            return parameterString == null ? Activator.CreateInstance(TargetType) : Enum.Parse(TargetType, parameterString);
        }
    }

    public class EnumVisibilityConverter : IValueConverter
    {
        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            var parameterString = Parameter as string;
            if (parameterString == null)
                return DependencyProperty.UnsetValue;

            if (Enum.IsDefined(Value.GetType(), Value) == false)
                return DependencyProperty.UnsetValue;

            var parameterValue = Enum.Parse(Value.GetType(), parameterString);

            return parameterValue.Equals(Value) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            var parameterString = Parameter as string;

            return parameterString == null ? Activator.CreateInstance(TargetType) : Enum.Parse(TargetType, parameterString);
        }
    }

    public class RoundToEven1000Converter : IValueConverter
    {
        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            return Math.Round(System.Convert.ToDecimal(System.Convert.ToDouble(Value) / 1000.0), 2,
                              MidpointRounding.ToEven);
        }

        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            return System.Convert.ToDouble(Value) * 1000.0;
        }
    }

    public class EnumValueToString : IValueConverter
    {
        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            return Value.ToString();
        }

        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            return Enum.Parse(TargetType, (string)Value);
        }
    }

    public class TDcFallRateValueFromEnum : IValueConverter
    {
        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            ushort value = (ushort)((TDcFallRate)Value);

            return value.ToString();
        }

        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            return Enum.Parse(TargetType, (string)Value);
        }
    }

    public class TOsvRateValueFromEnum : IValueConverter
    {
        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            ushort value = (ushort)((TOsvRate)Value);

            return value.ToString();
        }

        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            return Enum.Parse(TargetType, (string)Value);
        }
    }

    public class FrequencyDivisorToFrequency : IValueConverter
    {
        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            return (50 / (ushort)Value).ToString(CultureInfo.InvariantCulture);
        }

        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            return (ushort)(50 / UInt16.Parse((string)Value));
        }
    }

    public class VtmTestTypeToStringConverter : IValueConverter
    {
        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            var type = (VTMTestType)Value;
            var line = string.Empty;

            switch (type)
            {
                case VTMTestType.Ramp:
                    line = Resources.Ramp;
                    break;
                case VTMTestType.Sinus:
                    line = Resources.Sinus;
                    break;
                case VTMTestType.Curve:
                    line = Resources.Curve;
                    break;
            }

            return line;
        }

        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            var type = VTMTestType.Ramp;
            var line = (string)Value;

            if (line == Resources.Ramp)
                type = VTMTestType.Ramp;
            else if (line == Resources.Sinus)
                type = VTMTestType.Sinus;
            else if (line == Resources.Curve)
                type = VTMTestType.Curve;

            return type;
        }
    }

    public class BvtTypeToStringConverter : IValueConverter
    {
        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            var type = (BVTTestType)Value;
            var line = string.Empty;

            switch (type)
            {
                case BVTTestType.Both:
                    line = Resources.Both;
                    break;
                case BVTTestType.Direct:
                    line = Resources.Direct;
                    break;
                case BVTTestType.Reverse:
                    line = Resources.Reverse;
                    break;
            }

            return line;
        }


        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            var type = BVTTestType.Both;
            var line = (string)Value;

            if (line == Resources.Both)
                type = BVTTestType.Both;
            else if (line == Resources.Direct)
                type = BVTTestType.Direct;
            else if (line == Resources.Reverse)
                type = BVTTestType.Reverse;

            return type;
        }
    }

    public class dVdtTypeToStringConverter : IValueConverter
    {
        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            var type = (DvdtMode)Value;
            var line = string.Empty;

            switch (type)
            {
                case DvdtMode.Confirmation:
                    line = Resources.DvdtConfirmation;
                    break;
                case DvdtMode.Detection:
                    line = Resources.DvdtDetection;
                    break;
            }

            return line;
        }


        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            var type = DvdtMode.Confirmation;
            var line = (string)Value;

            if (line == Resources.DvdtConfirmation)
                type = DvdtMode.Confirmation;
            else if (line == Resources.DvdtDetection)
                type = DvdtMode.Detection;

            return type;
        }
    }

    public class VtmTestTypeToVisibilityConverter : IValueConverter
    {
        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            var type = (VTMTestType)Value;
            int index = int.Parse((string)Parameter);

            if ((type == VTMTestType.Ramp && index == 0) || (type == VTMTestType.Sinus && index == 1) ||
                (type == VTMTestType.Curve && index == 2))
                return Visibility.Visible;

            return Visibility.Collapsed;
        }

        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            throw new InvalidOperationException("ConvertBack method is not implemented in VtmTestTypeToVisibilityConverter");
        }
    }

    public class DvdtTestTypeToVisibilityConverter : IValueConverter
    {
        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            var type = (DvdtMode)Value;

            if (type == DvdtMode.Confirmation)
                return Visibility.Visible;

            return Visibility.Collapsed;
        }

        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            throw new InvalidOperationException("ConvertBack method is not implemented in VtmTestTypeToVisibilityConverter");
        }
    }

    public class DvdtTestTypeToVisibilityUnConverter : IValueConverter
    {
        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            var type = (DvdtMode)Value;

            if (type == DvdtMode.Detection)
                return Visibility.Visible;

            return Visibility.Collapsed;
        }

        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            throw new InvalidOperationException("ConvertBack method is not implemented in VtmTestTypeToVisibilityConverter");
        }
    }

    public class IsGreaterThanConverter : IValueConverter
    {
        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            var val = (int)Value;
            var par = (int)Parameter;

            return val > par;
        }

        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            throw new InvalidOperationException("ConvertBack method is not implemented in IsGreaterThanConverter");
        }
    }

    public static class ConverterUtil
    {
        public static HWModuleCommutationType MapCommutationType(ModuleCommutationType Arg)
        {
            return (HWModuleCommutationType)Arg;
        }

        public static HWModulePosition MapModulePosition(ModulePosition Arg)
        {
            switch (Arg)
            {
                case ModulePosition.P1:
                    return HWModulePosition.Position1;
                case ModulePosition.P2:
                    return HWModulePosition.Position2;
                default:
                    return HWModulePosition.Position1;
            }
        }

        public static Tuple<HWModuleCommutationType, BVTTestType, bool> MapCommutationType(
            ModuleCommutationType CommType, int Position)
        {
            var ct = (HWModuleCommutationType)CommType;

            switch (ct)
            {
                case HWModuleCommutationType.Reverse:
                    return
                        new Tuple<HWModuleCommutationType, BVTTestType, bool>(
                            HWModuleCommutationType.Reverse, BVTTestType.Both, false);
                case HWModuleCommutationType.Direct:
                    return
                        new Tuple<HWModuleCommutationType, BVTTestType, bool>(
                            HWModuleCommutationType.Direct, BVTTestType.Both, true);
                case HWModuleCommutationType.MT1:
                    return
                        new Tuple<HWModuleCommutationType, BVTTestType, bool>(
                            HWModuleCommutationType.MT1, BVTTestType.Both, true);
                case HWModuleCommutationType.MD1:
                    return
                        new Tuple<HWModuleCommutationType, BVTTestType, bool>(
                            HWModuleCommutationType.MD1, BVTTestType.Reverse, false);
                case HWModuleCommutationType.MT3:
                    return
                        new Tuple<HWModuleCommutationType, BVTTestType, bool>(
                            HWModuleCommutationType.MT3, BVTTestType.Both, true);
                case HWModuleCommutationType.MD3:
                    return
                        new Tuple<HWModuleCommutationType, BVTTestType, bool>(
                            HWModuleCommutationType.MD3, BVTTestType.Reverse, false);
                case HWModuleCommutationType.MTD3:
                    return
                        new Tuple<HWModuleCommutationType, BVTTestType, bool>(
                            HWModuleCommutationType.MTD3,
                            (Position == 1) ? BVTTestType.Both : BVTTestType.Reverse, Position == 1);
                case HWModuleCommutationType.MDT3:
                    return
                        new Tuple<HWModuleCommutationType, BVTTestType, bool>(
                            HWModuleCommutationType.MDT3,
                            (Position == 1) ? BVTTestType.Reverse : BVTTestType.Both, Position != 1);
                case HWModuleCommutationType.MT4:
                    return
                        new Tuple<HWModuleCommutationType, BVTTestType, bool>(
                            HWModuleCommutationType.MT4, BVTTestType.Both, true);
                case HWModuleCommutationType.MD4:
                    return
                        new Tuple<HWModuleCommutationType, BVTTestType, bool>(
                            HWModuleCommutationType.MD4, BVTTestType.Reverse, false);
                case HWModuleCommutationType.MTD4:
                    return
                        new Tuple<HWModuleCommutationType, BVTTestType, bool>(
                            HWModuleCommutationType.MTD4,
                            (Position == 1) ? BVTTestType.Both : BVTTestType.Reverse, Position == 1);
                case HWModuleCommutationType.MDT4:
                    return
                        new Tuple<HWModuleCommutationType, BVTTestType, bool>(
                            HWModuleCommutationType.MDT4,
                            (Position == 1) ? BVTTestType.Reverse : BVTTestType.Both, Position != 1);
                case HWModuleCommutationType.MT5:
                    return
                        new Tuple<HWModuleCommutationType, BVTTestType, bool>(
                            HWModuleCommutationType.MT5, BVTTestType.Both, true);
                case HWModuleCommutationType.MD5:
                    return
                        new Tuple<HWModuleCommutationType, BVTTestType, bool>(
                            HWModuleCommutationType.MD5, BVTTestType.Reverse, false);
                case HWModuleCommutationType.MTD5:
                    return
                        new Tuple<HWModuleCommutationType, BVTTestType, bool>(
                            HWModuleCommutationType.MTD5,
                            (Position == 1) ? BVTTestType.Both : BVTTestType.Reverse, Position == 1);
                case HWModuleCommutationType.MDT5:
                    return
                        new Tuple<HWModuleCommutationType, BVTTestType, bool>(
                            HWModuleCommutationType.MDT5,
                            (Position == 1) ? BVTTestType.Reverse : BVTTestType.Both, Position != 1);
                default:
                    throw new ArgumentOutOfRangeException("CommType");
            }
        }
    }
}