using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using SCME.Types.BaseTestParams;
using SCME.Types.BVT;
using SCME.Types.Clamping;
using SCME.Types.Commutation;
using SCME.Types.dVdt;
using SCME.Types.QrrTq;
using SCME.Types.VTM;
using SCME.WpfControlLibrary.Properties;
using Settings = SCME.UIServiceConfig.Properties.Settings;

// ReSharper disable UnusedMethodReturnValue.Global

namespace SCME.WpfControlLibrary.DataProviders
{
    public static class TestTypeEnumDictionary
    {
        public static Dictionary<string, BVTTestType> GetBVTTestTypes()
        {
            return new Dictionary<string, BVTTestType>()
            {
                {Resources.Both, BVTTestType.Both},
                {Resources.Direct, BVTTestType.Direct},
                {Resources.Reverse, BVTTestType.Reverse},
            };
        }

        public static Dictionary<string, VTMTestType> GetVTMTestTypes()
        {
            return new Dictionary<string, VTMTestType>()
            {
                {Resources.Sinus, VTMTestType.Sinus},
                {Resources.Ramp, VTMTestType.Ramp},
                {Resources.Curve, VTMTestType.Curve},
            };
        }

        public static Dictionary<string, BVTMeasurementMode> GetBVTMeasurementModes()
        {
            return new Dictionary<string, BVTMeasurementMode>()
            {
                {"Подтверждение", BVTMeasurementMode.ModeI},
                {"Определение", BVTMeasurementMode.ModeV},
            };
        }

        public static Dictionary<string, DvdtMode> GetDvdtModes()
        {
            return new Dictionary<string, DvdtMode>()
            {
                {Resources.DvdtConfirmation, DvdtMode.Confirmation},
                {Resources.DvdtDetection, DvdtMode.Detection},
            };
        }

        public static Dictionary<string, VoltageRate> GetVoltageRates()
        {
            return new Dictionary<string, VoltageRate>()
            {
                {"500", VoltageRate.V500},
                {"1000", VoltageRate.V1000},
                {"1600", VoltageRate.V1600},
                {"2000", VoltageRate.V2000},
                {"2500", VoltageRate.V2500},
            };
        }

        public static Dictionary<string, TMode> GetTModes()
        {
            return new Dictionary<string, TMode>()
            {
                {nameof(TMode.Qrr), TMode.Qrr},
                {"Tq", TMode.QrrTq}
            };
        }

        public static Dictionary<string, bool> GetTrrModes()
        {
            return new Dictionary<string, bool>()
            {
                {"ГОСТ", false},
                {"90%-50%", true}
            };
        }

        public static Dictionary<string, TDcFallRate> GetTDcFallRates()
        {
            return new Dictionary<string, TDcFallRate>()
            {
                {"2", TDcFallRate.r2},
                {"5", TDcFallRate.r5},
                {"10", TDcFallRate.r10},
                {"15", TDcFallRate.r15},
                {"20", TDcFallRate.r20},
                {"30", TDcFallRate.r30},
                {"50", TDcFallRate.r50},
                {"60", TDcFallRate.r60},
                {"100", TDcFallRate.r100},
            };
        }


        public static Dictionary<string, TOsvRate> GetTOsvRates()
        {
            return new Dictionary<string, TOsvRate>()
            {
                {"20", TOsvRate.r20},
                {"50", TOsvRate.r50},
                {"100", TOsvRate.r100},
                {"200", TOsvRate.r200},
            };
        }

        public static Dictionary<string, ModuleCommutationType> GetModuleCommutationTypes()
        {
            return new Dictionary<string, ModuleCommutationType>()
            {
                {nameof(ModuleCommutationType.Direct), ModuleCommutationType.Direct},
                {nameof(ModuleCommutationType.Reverse), ModuleCommutationType.Reverse},
                {nameof(ModuleCommutationType.MD1), ModuleCommutationType.MD1},
                {nameof(ModuleCommutationType.MD3), ModuleCommutationType.MD3},
                {nameof(ModuleCommutationType.MD4), ModuleCommutationType.MD4},
                {nameof(ModuleCommutationType.MD5), ModuleCommutationType.MD5},
                {nameof(ModuleCommutationType.MT1), ModuleCommutationType.MT1},
                {nameof(ModuleCommutationType.MT3), ModuleCommutationType.MT3},
                {nameof(ModuleCommutationType.MT4), ModuleCommutationType.MT4},
                {nameof(ModuleCommutationType.MT5), ModuleCommutationType.MT5},
                {nameof(ModuleCommutationType.MDT3), ModuleCommutationType.MDT3},
                {nameof(ModuleCommutationType.MDT4), ModuleCommutationType.MDT4},
                {nameof(ModuleCommutationType.MDT5), ModuleCommutationType.MDT5},
                {nameof(ModuleCommutationType.MTD3), ModuleCommutationType.MTD3},
                {nameof(ModuleCommutationType.MTD4), ModuleCommutationType.MTD4},
                {nameof(ModuleCommutationType.MTD5), ModuleCommutationType.MTD5},
            };
        }

        public static Dictionary<string, ModuleCommutationType> GetDataFromCommutationModeEnum()
        {
            return UIServiceConfig.Properties.Settings.Default.ClampingSystemType switch
            {
                ClampingSystemType.Ignored => new Dictionary<string, ModuleCommutationType>()
                {
                    { nameof(ModuleCommutationType.Direct), ModuleCommutationType.Direct },
                    { nameof(ModuleCommutationType.Reverse), ModuleCommutationType.Reverse },
                    {nameof(ModuleCommutationType.MD1), ModuleCommutationType.MD1},
                    {nameof(ModuleCommutationType.MD3), ModuleCommutationType.MD3},
                    {nameof(ModuleCommutationType.MD4), ModuleCommutationType.MD4},
                    {nameof(ModuleCommutationType.MD5), ModuleCommutationType.MD5},
                    {nameof(ModuleCommutationType.MT1), ModuleCommutationType.MT1},
                    {nameof(ModuleCommutationType.MT3), ModuleCommutationType.MT3},
                    {nameof(ModuleCommutationType.MT4), ModuleCommutationType.MT4},
                    {nameof(ModuleCommutationType.MT5), ModuleCommutationType.MT5},
                    {nameof(ModuleCommutationType.MDT3), ModuleCommutationType.MDT3},
                    {nameof(ModuleCommutationType.MDT4), ModuleCommutationType.MDT4},
                    {nameof(ModuleCommutationType.MDT5), ModuleCommutationType.MDT5},
                    {nameof(ModuleCommutationType.MTD3), ModuleCommutationType.MTD3},
                    {nameof(ModuleCommutationType.MTD4), ModuleCommutationType.MTD4},
                    {nameof(ModuleCommutationType.MTD5), ModuleCommutationType.MTD5}
                },
                ClampingSystemType.Presspack => new Dictionary<string, ModuleCommutationType>() {{nameof(ModuleCommutationType.Direct), ModuleCommutationType.Direct}},
                ClampingSystemType.Stud => new Dictionary<string, ModuleCommutationType>() {{nameof(ModuleCommutationType.Direct), ModuleCommutationType.Direct}, {nameof(ModuleCommutationType.Reverse), ModuleCommutationType.Reverse}},
                ClampingSystemType.Module => new Dictionary<string, ModuleCommutationType>()
                {
                    {nameof(ModuleCommutationType.MD1), ModuleCommutationType.MD1},
                    {nameof(ModuleCommutationType.MD3), ModuleCommutationType.MD3},
                    {nameof(ModuleCommutationType.MD4), ModuleCommutationType.MD4},
                    {nameof(ModuleCommutationType.MD5), ModuleCommutationType.MD5},
                    {nameof(ModuleCommutationType.MT1), ModuleCommutationType.MT1},
                    {nameof(ModuleCommutationType.MT3), ModuleCommutationType.MT3},
                    {nameof(ModuleCommutationType.MT4), ModuleCommutationType.MT4},
                    {nameof(ModuleCommutationType.MT5), ModuleCommutationType.MT5},
                    {nameof(ModuleCommutationType.MDT3), ModuleCommutationType.MDT3},
                    {nameof(ModuleCommutationType.MDT4), ModuleCommutationType.MDT4},
                    {nameof(ModuleCommutationType.MDT5), ModuleCommutationType.MDT5},
                    {nameof(ModuleCommutationType.MTD3), ModuleCommutationType.MTD3},
                    {nameof(ModuleCommutationType.MTD4), ModuleCommutationType.MTD4},
                    {nameof(ModuleCommutationType.MTD5), ModuleCommutationType.MTD5}
                },
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public static Visibility GetVisibilityHeightForce()
        {
            return Settings.Default.ClampingSystemType == ClampingSystemType.Presspack || Settings.Default.ClampingSystemType == ClampingSystemType.Ignored ? Visibility.Visible : Visibility.Collapsed;
        }

        public static Visibility GetVisibilityModuleType()
        {
            return Settings.Default.ClampingSystemType == ClampingSystemType.Module ? Visibility.Visible : Visibility.Collapsed;
        }

        public static Dictionary<string, ModuleType> GetModuleTypes()
        {
            return new Dictionary<string, ModuleType>()
            {
                {nameof(ModuleType.A2), ModuleType.A2},
                {nameof(ModuleType.C1), ModuleType.C1},
                {nameof(ModuleType.E0), ModuleType.E0},
                {nameof(ModuleType.F1), ModuleType.F1},
                {nameof(ModuleType.D0), ModuleType.D0},
                {nameof(ModuleType.B1), ModuleType.B1},
                {"Аттестация", ModuleType.Validation}
            };
        }

        public static Visibility GetVisibilityTopTemp()
        {
            return Settings.Default.ClampingSystemType == ClampingSystemType.Module ? Visibility.Collapsed: Visibility.Visible;
        }

        public static Dictionary<string, ClampingForce> GetClampingForceTypes()
        {
            return new Dictionary<string, ClampingForce>()
            {
                {nameof(ClampingForce.Contact), ClampingForce.Contact},
                {nameof(ClampingForce.Custom), ClampingForce.Custom},
            };
        }

        public static List<TestParameter> GetTestParametersTypes()
        {
            return new List<TestParameter>()
            {
                new TestParameter(TestParametersType.GTU, typeof(Types.GTU.TestParameters), "GTU"),
                new TestParameter(TestParametersType.SL, typeof(Types.VTM.TestParameters), "SL"),
                new TestParameter(TestParametersType.BVT, typeof(Types.BVT.TestParameters), "BVT"),
                new TestParameter(TestParametersType.dVdt, typeof(Types.dVdt.TestParameters), "dUdt"),
                new TestParameter(TestParametersType.ATU, typeof(Types.ATU.TestParameters), "ATU"),
                new TestParameter(TestParametersType.QrrTq, typeof(Types.QrrTq.TestParameters), "QrrTq"),
                new TestParameter(TestParametersType.TOU, typeof(Types.TOU.TestParameters), "TOU"),
                new TestParameter(TestParametersType.Clamping, typeof(Types.Clamping.TestParameters), Resources.Height)
            };
        }

        public static double MeasureString(string candidate)
        {
            var fontFamily = ((FontFamily) Application.Current.Resources["SCME.DefaultFont"]).FamilyNames.First().Value;

            var formattedText = new FormattedText(
                candidate,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(fontFamily),
                (double) Application.Current.Resources["SCME.BaseFontSize"],
                Brushes.Black,
                new NumberSubstitution());

            return formattedText.Width;
        }

        public static Dictionary<ushort, ushort> GetFrequencyDivisors()
        {
            return new Dictionary<ushort, ushort>()
            {
                {50, 1},
                {25, 2},
                {10, 5},
                {5, 10},
                {2, 25},
                {1, 50},
            };
        }
        
        public static List<ushort> GetVoltageFrequency()
        {
            return new List<ushort>()
            {
                50,60
            };
        }

        public static string GetTopTempName()
        {
            return Settings.Default.ClampingSystemType == ClampingSystemType.Stud ? Resources.Housing : Resources.TopTempName;
        }
        
        public static string GetBottomTempName()
        {
            return Settings.Default.ClampingSystemType == ClampingSystemType.Stud ? Resources.Output : Resources.BotTempName;
        }
        
    }
}