using SCME.Types.BaseTestParams;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SCME.Types.GTU
{
    /// <summary>��������� ������������</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public enum HWDeviceState
    {
        /// <summary>�������������� ���������</summary>
        [EnumMember]
        None = 0,
        /// <summary>������</summary>
        [EnumMember]
        Fault = 1,
        /// <summary>��������</summary>
        [EnumMember]
        Disabled = 2,
        /// <summary>���������</summary>
        [EnumMember]
        Kelvin = 3,
        /// <summary>Gate</summary>
        [EnumMember]
        Gate = 4,
        /// <summary>IH</summary>
        [EnumMember]
        IH = 5,
        /// <summary>IL</summary>
        [EnumMember]
        IL = 6,
        /// <summary>�������������</summary>
        [EnumMember]
        RG = 7,
        /// <summary>����������</summary>
        [EnumMember]
        Calibrate = 8,
        /// <summary>VGNT</summary>
        [EnumMember]
        VGNT = 9
    };

    /// <summary>������� ������</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public enum HWFaultReason
    {
        [EnumMember]
        None = 0,
        [EnumMember]
        HoldProcessError = 101,
        [EnumMember]
        GateProcessError = 112,
        [EnumMember]
        LatchProcessError = 121
    };

    /// <summary>������� ��������������</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public enum HWWarningReason
    {
        [EnumMember]
        None = 0,
        [EnumMember]
        HoldingCurrentSmall = 101,
        /// <summary>������� ������������� watchdog'��</summary>
        [EnumMember]
        WatchdogReset = 1001
    };

    /// <summary>������� ����������</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public enum HWDisableReason
    {
        [EnumMember]
        None = 0,
        /// <summary>�������� � ������� ������������</summary>
        [EnumMember]
        BadClock = 1001
    };

    /// <summary>������� ��������</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public enum HWProblemReason
    {
        [EnumMember]
        None = 0,
        /// <summary>������� �������� �������������</summary>
        [EnumMember]
        OperationStopped = 1,
        /// <summary>������ ������������ VD</summary>
        [EnumMember]
        VDSetErr = 2,
        /// <summary>������ ������������ ID</summary>
        [EnumMember]
        IDSetErr = 3,
        /// <summary>������ ������������ VG</summary>
        [EnumMember]
        VGSetErr = 4,
        /// <summary>������ ������������ IG</summary>
        [EnumMember]
        IGSetErr = 5,
        /// <summary>����������� ������ ���</summary>
        [EnumMember]
        IDLeak = 6,
        /// <summary>�� ������� DUT</summary>
        [EnumMember]
        DUTNoTrig = 7,
        /// <summary>�� �������� DUT</summary>
        [EnumMember]
        DUTNoClose = 8,
        /// <summary>�� ������ DUT</summary>
        [EnumMember]
        DUTNoLatching = 9,
        /// <summary>��� �������������� ������� DUT VG</summary>
        [EnumMember]
        DUTNoVGSensing = 10,
        /// <summary>DUT ������� �� ����� ������������� VGNT</summary>
        [EnumMember]
        VGNTConfTrig = 11,
        [EnumMember]
        HoldReachTimeout = 101,
        [EnumMember]
        GateCurrentHigh = 111,
        [EnumMember]
        GateFollowingError = 112,
        [EnumMember]
        GateIgtOverload = 113,
        [EnumMember]
        LatchCurrentHigh = 121,
        [EnumMember]
        LatchFollowingError = 122,
        [EnumMember]
        RGateFollowingError = 141,
        [EnumMember]
        RGateOverload = 142
    };

    /// <summary>��������� ����������</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public enum HWOperationResult
    {
        /// <summary>� ��������</summary>
        [EnumMember]
        InProcess = 0,
        /// <summary>�������</summary>
        [EnumMember]
        Success = 1,
        /// <summary>��������</summary>
        [EnumMember]
        Fail = 2
    };

    /// <summary>��������� ������������ ������</summary>
    [DataContract(Name = "GTU.TestParameters", Namespace = "http://proton-electrotex.com/SCME")]
    public class TestParameters : BaseTestParametersAndNormatives, ICloneable
    {
        //������������� IH ����
        private int useIhGost = 1;

        /// <summary>�������������� ����� ��������� ������ TestParameters</summary>
        public TestParameters()
        {
            TestParametersType = TestParametersType.GTU;
            IsEnabled = true;
            Resistance = 100;
            MinIGT = 60;
            IGT = 500;
            VGT = 2.5f;
            IH = 150;
            IL = 1000;
            Itm = 0;
            CurrentLimit = 5;
            VoltageLimitD = 1000;
            PlateTime = 1000;
            RampUpVoltage = 2;
            StartVoltage = 500;
            IGNT = 200;
            VGNT = 2000;
        }

        [DataMember]
        public bool IsCurrentEnabled
        {
            get; set;
        }

        [DataMember]
        public ushort Itm
        {
            get; set;
        }

        [DataMember]
        public bool IsIhEnabled
        {
            get; set;
        }

        [DataMember]
        public bool IsIhStrikeCurrentEnabled
        {
            get; set;
        }

        [DataMember]
        public bool IsIlEnabled
        {
            get; set;
        }

        [DataMember]
        public float Resistance
        {
            get; set;
        }

        [DataMember]
        public float IGT
        {
            get; set;
        }

        [DataMember]
        public float MinIGT
        {
            get; set;
        }

        [DataMember]
        public float VGT
        {
            get; set;
        }

        [DataMember]
        public bool UseIhGost
        {
            get => Convert.ToBoolean(useIhGost);
            set => useIhGost = value ? 1 : 0;
        }

        [DataMember]
        public float IH
        {
            get; set;
        }

        [DataMember]
        public float IL
        {
            get; set;
        }

        [DataMember]
        public bool UseVgnt
        {
            get; set;
        }

        [DataMember]
        public ushort IGNT
        {
            get; set;
        }

        [DataMember]
        public ushort VGNT
        {
            get; set;
        }

        [DataMember]
        public float CurrentLimit
        {
            get; set;
        }

        [DataMember]
        public ushort VoltageLimitD
        {
            get; set;
        }

        [DataMember]
        public ushort PlateTime
        {
            get; set;
        }

        [DataMember]
        public float RampUpVoltage
        {
            get; set;
        }

        [DataMember]
        public ushort StartVoltage
        {
            get; set;
        }

        /// <summary>�������� ��������� � ����������</summary>
        /// <param name="oldParameters">������ ���������</param>
        /// <returns>���������� True, ���� ��������� ���� ��������</returns>
        public override bool HasChanges(BaseTestParametersAndNormatives oldParameters)
        {
            //������ ���������
            TestParameters OldTestParameters = (TestParameters)oldParameters;
            if (oldParameters == null)
                throw new InvalidCastException("OldParameters must be GTUOldParameters");
            if (IsCurrentEnabled != OldTestParameters.IsCurrentEnabled)
                return true;
            if (IsIhEnabled != OldTestParameters.IsIhEnabled)
                return true;
            if (IsIhStrikeCurrentEnabled != OldTestParameters.IsIhStrikeCurrentEnabled)
                return true;
            if (IsIlEnabled != OldTestParameters.IsIlEnabled)
                return true;
            if (Resistance != OldTestParameters.Resistance)
                return true;
            if (IGT != OldTestParameters.IGT)
                return true;
            if (VGT != OldTestParameters.VGT)
                return true;
            if (IH != OldTestParameters.IH)
                return true;
            if (IL != OldTestParameters.IL)
                return true;
            if (Itm != OldTestParameters.Itm)
                return true;
            if (UseVgnt != OldTestParameters.UseVgnt)
                return true;
            if (CurrentLimit != OldTestParameters.CurrentLimit)
                return true;
            if (VoltageLimitD != OldTestParameters.VoltageLimitD)
                return true;
            if (PlateTime != OldTestParameters.PlateTime)
                return true;
            if (RampUpVoltage != OldTestParameters.RampUpVoltage)
                return true;
            if (StartVoltage != OldTestParameters.StartVoltage)
                return true;
            return false;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    /// <summary>��������� ������������</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public class ResultNormatives
    {
        /// <summary>�������������� ����� ��������� ������ ResultNormatives</summary>
        public ResultNormatives()
        {
            Resistance = 100;
            IGT = 500;
            VGT = 2.5f;
            IH = 150;
            IL = 1000;
            IGNT = 200;
            VGNT = 2000;
        }

        [DataMember]
        public float Resistance
        {
            get; set;
        }

        [DataMember]
        public float IGT
        {
            get; set;
        }

        [DataMember]
        public float VGT
        {
            get; set;
        }

        [DataMember]
        public float IH
        {
            get; set;
        }

        [DataMember]
        public float IL
        {
            get; set;
        }

        [DataMember]
        public ushort IGNT
        {
            get; set;
        }

        [DataMember]
        public ushort VGNT
        {
            get; set;
        }
    }

    /// <summary>���������� ������������</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public class TestResults : BaseTestResults
    {
        /// <summary>�������������� ����� ��������� ������ TestResults</summary>
        public TestResults()
        {
            ArrayKelvin = new List<short>();
            ArrayIGT = new List<short>();
            ArrayVGT = new List<short>();
            ArrayIH = new List<short>();
        }

        [DataMember]
        public bool IsKelvinOk
        {
            get; set;
        }

        [DataMember]
        public float Resistance
        {
            get; set;
        }

        [DataMember]
        public float IGT
        {
            get; set;
        }

        [DataMember]
        public float VGT
        {
            get; set;
        }

        [DataMember]
        public float IH
        {
            get; set;
        }

        [DataMember]
        public float IL
        {
            get; set;
        }

        [DataMember]
        public ushort IGNT
        {
            get; set;
        }

        [DataMember]
        public float VGNT
        {
            get; set;
        }

        [DataMember]
        public IList<short> ArrayKelvin
        {
            get; set;
        }

        [DataMember]
        public IList<short> ArrayIGT
        {
            get; set;
        }

        [DataMember]
        public IList<short> ArrayVGT
        {
            get; set;
        }

        [DataMember]
        public IList<short> ArrayIH
        {
            get; set;
        }
    }

    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public class CalibrationResultGate
    {
        [DataMember]
        public ushort Current
        {
            get; set;
        }

        [DataMember]
        public ushort Voltage
        {
            get; set;
        }
    }

    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public class CalibrationParameters
    {
        [DataMember]
        public short GateIGTOffset
        {
            get; set;
        }

        [DataMember]
        public short GateVGTOffset
        {
            get; set;
        }

        [DataMember]
        public short GateIHLOffset
        {
            get; set;
        }

        [DataMember]
        public ushort RgCurrent
        {
            get; set;
        }

        [DataMember]
        public ushort GateFineIGT_N
        {
            get; set;
        }

        [DataMember]
        public ushort GateFineIGT_D
        {
            get; set;
        }

        [DataMember]
        public ushort GateFineVGT_N
        {
            get; set;
        }

        [DataMember]
        public ushort GateFineVGT_D
        {
            get; set;
        }

        [DataMember]
        public ushort GateFineIHL_N
        {
            get; set;
        }

        [DataMember]
        public ushort GateFineIHL_D
        {
            get; set;
        }
    }
}