using PropertyChanged;
using SCME.Types.BaseTestParams;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SCME.Types.BVT
{
    /// <summary>��������� ������������</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public enum HWDeviceState
    {
        /// <summary>�������������� ���������</summary>
        [EnumMember]
        DS_None = 0,
        /// <summary>������</summary>
        [EnumMember]
        DS_Fault = 1,
        /// <summary>��������</summary>
        [EnumMember]
        DS_Disabled = 2,
        [EnumMember]
        DS_dummy = 3,
        /// <summary>�������</summary>
        [EnumMember]
        DS_Powered = 4,
        /// <summary>� �������� ������</summary>
        [EnumMember]
        DS_InProcess = 5
    };

    /// <summary>������� ������</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public enum HWFaultReason
    {
        [EnumMember]
        None = 0,
        /// <summary>���������� ���� �����</summary>
        [EnumMember]
        BridgeShort = 200,
        /// <summary>�������� �����</summary>
        [EnumMember]
        TempMon = 201,
        /// <summary>������ ���������� �����</summary>
        [EnumMember]
        OptoConError = 202,
        /// <summary>������ ������ �������</summary>
        [EnumMember]
        LowSidePS = 203,
        /// <summary>������ ���������� ����������</summary>
        [EnumMember]
        FollowingError = 204,
        /// <summary>�������� ������������ PWM</summary>
        [EnumMember]
        PWMSaturation = 205,
        [EnumMember]
        BridgeOverload = 400,
        [EnumMember]
        TemperatureOverload = 401
    };

    /// <summary>������� ��������������</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public enum HWWarningReason
    {
        [EnumMember]
        None = 0,
        [EnumMember]
        CurrentNotReached = 401,
        /// <summary>�� ���������� ������� � ������ V-Mode</summary>
        [EnumMember]
        VoltageNotReached = 402,
        /// <summary>������������� ��� ���������</summary>
        [EnumMember]
        ResOutOfRange = 403,
        /// <summary>���������� ������</summary>
        [EnumMember]
        OutputOverload = 404,
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
        [EnumMember]
        NoShSignal = 400,
        [EnumMember]
        NoStSignal = 401,
        [EnumMember]
        NoTempSignal = 402
    };

    /// <summary>������� ��������</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public enum HWProblemReason
    {
        [EnumMember]
        None = 0,
        [EnumMember]
        Stopped = 401
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
    [DataContract(Name = "BVT.TestParameters", Namespace = "http://proton-electrotex.com/SCME")]
    [AddINotifyPropertyChangedInterface]
    public class TestParameters : BaseTestParametersAndNormatives, ICloneable
    {
        /// <summary>�������������� ����� ��������� ������ TestParameters</summary>
        public TestParameters()
        {
            TestParametersType = TestParametersType.BVT;
            IsEnabled = false;
            UdsmUrsmTestType = TestType = BVTTestType.Reverse;
            MeasurementMode = BVTMeasurementMode.ModeV;
            UdsmUrsmVoltageLimitD = VoltageLimitD = 1000;
            UdsmUrsmVoltageLimitR = VoltageLimitR = 1000;
            UdsmUrsmCurrentLimit = CurrentLimit = 5;
            UdsmUrsmPlateTime = PlateTime = 800;
            UdsmUrsmRampUpVoltage = RampUpVoltage = 1;
            UdsmUrsmStartVoltage = StartVoltage = 500;
            UdsmUrsmVoltageFrequency = VoltageFrequency = 50;
            UdsmUrsmFrequencyDivisor = FrequencyDivisor = 1;
            VDRM = 1400;
            VRRM = 1400;
            IDSM = IDRM = 5;
            IRSM = IRRM = 5;
        }

        [DataMember]
        public BVTTestType UdsmUrsmTestType
        {
            get; set;
        }

        [DataMember]
        public ushort UdsmUrsmPulseFrequency
        {
            get; set;
        }

        [DataMember]
        public float UdsmUrsmCurrentLimit
        {
            get; set;
        }

        [DataMember]
        public float UdsmUrsmRampUpVoltage
        {
            get; set;
        }

        [DataMember]
        public ushort UdsmUrsmStartVoltage
        {
            get; set;
        }

        [DataMember]
        public ushort UdsmUrsmVoltageFrequency
        {
            get; set;
        }

        [DataMember]
        public ushort UdsmUrsmFrequencyDivisor
        {
            get; set;
        }

        [DataMember]
        public ushort UdsmUrsmVoltageLimitR
        {
            get; set;
        }

        [DataMember]
        public ushort UdsmUrsmVoltageLimitD
        {
            get; set;
        }

        [DataMember]
        public ushort UdsmUrsmPlateTime
        {
            get; set;
        }

        [DataMember]
        public ushort VDSM
        {
            get; set;
        }

        [DataMember]
        public ushort VRSM
        {
            get; set;
        }

        [DataMember]
        public float IDSM
        {
            get; set;
        }

        [DataMember]
        public float IRSM
        {
            get; set;
        }

        [DataMember]
        public bool UseUdsmUrsm
        {
            get; set;
        }

        [DataMember]
        [OnChangedMethod(nameof(OnAutoMeasureUdsmUrsmChanged))]
        public bool AutoMeasureUdsmUrsm
        {
            get; set;
        }

        [DataMember]
        public int? ClassByProfileName
        {
            get; set;
        }

        [DataMember]
        public ushort PulseFrequency
        {
            get; set;
        }

        [DataMember]
        public BVTMeasurementMode MeasurementMode
        {
            get; set;
        }

        [DataMember]
        public ushort VoltageLimitD
        {
            get; set;
        }

        [DataMember]
        public ushort VoltageLimitR
        {
            get; set;
        }

        [DataMember]
        public float CurrentLimit
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

        [DataMember]
        public ushort VoltageFrequency
        {
            get; set;
        }

        [DataMember]
        public ushort FrequencyDivisor
        {
            get; set;
        }

        [DataMember]
        public BVTTestType TestType
        {
            get; set;
        }

        [DataMember]
        public ushort VDRM
        {
            get; set;
        }

        [DataMember]
        public ushort VRRM
        {
            get; set;
        }

        [DataMember]
        public float IDRM
        {
            get; set;
        }

        [DataMember]
        public float IRRM
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
                throw new InvalidCastException("OldParameters must be BVTOldParameters");
            if (UseUdsmUrsm != OldTestParameters.UseUdsmUrsm)
                return true;
            if (AutoMeasureUdsmUrsm != OldTestParameters.AutoMeasureUdsmUrsm)
                return true;
            if (PulseFrequency != OldTestParameters.PulseFrequency)
                return true;
            if (MeasurementMode != OldTestParameters.MeasurementMode)
                return true;
            if (VoltageLimitD != OldTestParameters.VoltageLimitD)
                return true;
            if (VoltageLimitR != OldTestParameters.VoltageLimitR)
                return true;
            if (CurrentLimit != OldTestParameters.CurrentLimit)
                return true;
            if (PlateTime != OldTestParameters.PlateTime)
                return true;
            if (RampUpVoltage != OldTestParameters.RampUpVoltage)
                return true;
            if (StartVoltage != OldTestParameters.StartVoltage)
                return true;
            if (VoltageFrequency != OldTestParameters.VoltageFrequency)
                return true;
            if (FrequencyDivisor != OldTestParameters.FrequencyDivisor)
                return true;
            if (TestType != OldTestParameters.TestType)
                return true;
            if (VDRM != OldTestParameters.VDRM)
                return true;
            if (VRRM != OldTestParameters.VRRM)
                return true;
            if (IDRM != OldTestParameters.IDRM)
                return true;
            if (IRRM != OldTestParameters.IRRM)
                return true;
            if (UdsmUrsmPulseFrequency != OldTestParameters.UdsmUrsmPulseFrequency)
                return true;
            if (UdsmUrsmVoltageLimitD != OldTestParameters.VoltageLimitD)
                return true;
            if (UdsmUrsmVoltageLimitR != OldTestParameters.VoltageLimitR)
                return true;
            if (UdsmUrsmCurrentLimit != OldTestParameters.CurrentLimit)
                return true;
            if (UdsmUrsmPlateTime != OldTestParameters.PlateTime)
                return true;
            if (UdsmUrsmRampUpVoltage != OldTestParameters.RampUpVoltage)
                return true;
            if (UdsmUrsmStartVoltage != OldTestParameters.StartVoltage)
                return true;
            if (UdsmUrsmVoltageFrequency != OldTestParameters.VoltageFrequency)
                return true;
            if (UdsmUrsmFrequencyDivisor != OldTestParameters.FrequencyDivisor)
                return true;
            if (UdsmUrsmTestType != OldTestParameters.TestType)
                return true;
            if (VDSM != OldTestParameters.VDSM)
                return true;
            if (VRSM != OldTestParameters.VRSM)
                return true;
            if (IDSM != OldTestParameters.IDSM)
                return true;
            if (IRSM != OldTestParameters.IRSM)
                return true;
            return false;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        private void OnAutoMeasureUdsmUrsmChanged()
        {
            UdsmUrsmTestType = TestType;
            UdsmUrsmCurrentLimit = CurrentLimit;
            UdsmUrsmVoltageLimitD = VoltageLimitD;
            UdsmUrsmVoltageLimitR = VoltageLimitR;
            UdsmUrsmPlateTime = PlateTime;
            UdsmUrsmRampUpVoltage = RampUpVoltage;
            UdsmUrsmStartVoltage = StartVoltage;
            UdsmUrsmVoltageFrequency = VoltageFrequency;
            UdsmUrsmFrequencyDivisor = FrequencyDivisor;
        }
    }

    /// <summary>��������� ������������</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public class ResultNormatives
    {
        /// <summary>�������������� ����� ��������� ������ ResultNormatives</summary>
        public ResultNormatives()
        {
            VDRM = 1400;
            VRRM = 1400;
            UdsmUrsmIDRM = IDRM = 5;
            UdsmUrsmIRRM = IRRM = 5;
        }

        [DataMember]
        public ushort VDRM
        {
            get; set;
        }

        [DataMember]
        public ushort VRRM
        {
            get; set;
        }

        [DataMember]
        public float IDRM
        {
            get; set;
        }

        [DataMember]
        public float IRRM
        {
            get; set;
        }

        [DataMember]
        public float UdsmUrsmIDRM
        {
            get; set;
        }

        [DataMember]
        public float UdsmUrsmIRRM
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
            VoltageData = new List<short>();
            CurrentData = new List<short>();
        }

        [DataMember]
        public ushort VDSM
        {
            get; set;
        }

        [DataMember]
        public ushort VRSM
        {
            get; set;
        }

        [DataMember]
        public float IDSM
        {
            get; set;
        }


        [DataMember]
        public float IRSM
        {
            get; set;
        }

        [DataMember]
        public ushort VDRM
        {
            get; set;
        }

        [DataMember]
        public ushort VRRM
        {
            get; set;
        }

        [DataMember]
        public float IDRM
        {
            get; set;
        }

        [DataMember]
        public float IRRM
        {
            get; set;
        }

        [DataMember]
        public List<short> VoltageData
        {
            get; set;
        }

        [DataMember]
        public List<short> CurrentData
        {
            get; set;
        }
    }

    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public class CalibrationParameters
    {
        [DataMember]
        public ushort S1Current1FineN
        {
            get; set;
        }

        [DataMember]
        public ushort S1Current1FineD
        {
            get; set;
        }

        [DataMember]
        public ushort S1Current2FineN
        {
            get; set;
        }

        [DataMember]
        public ushort S1Current2FineD
        {
            get; set;
        }

        [DataMember]
        public ushort S1Voltage1FineN
        {
            get; set;
        }

        [DataMember]
        public ushort S1Voltage1FineD
        {
            get; set;
        }

        [DataMember]
        public ushort S1Voltage2FineN
        {
            get; set;
        }

        [DataMember]
        public ushort S1Voltage2FineD
        {
            get; set;
        }
    }
}