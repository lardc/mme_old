using SCME.Types.BaseTestParams;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SCME.Types.SL
{
    /// <summary>��������� ������������ ������</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public class TestParameters : BaseTestParametersAndNormatives, ICloneable
    {
        /// <summary>�������������� ����� ��������� ������ TestParameters</summary>
        public TestParameters()
        {
            TestParametersType = TestParametersType.SL;
            IsEnabled = true;
            TestType = SLTestType.Sinus;
            RampCurrent = 500;
            RampTime = 1000;
            RampOpeningCurrent = 100;
            RampOpeningTime = 50;
            IsRampOpeningEnabled = false;
            SinusCurrent = 500;
            SinusTime = 10000;
            CurveCurrent = 500;
            CurveTime = 4000;
            CurveFactor = 50;
            CurveAddTime = 0;
            Count = 1;
            VTM = 2.5f;

            IsSelfTest = false;
        }

        [DataMember]
        public SLTestType TestType
        {
            get; set;
        }

        [DataMember]
        public ushort RampCurrent
        {
            get; set;
        }

        [DataMember]
        public ushort RampTime
        {
            get; set;
        }

        [DataMember]
        public bool IsRampOpeningEnabled
        {
            get; set;
        }

        [DataMember]
        public ushort RampOpeningCurrent
        {
            get; set;
        }

        [DataMember]
        public ushort RampOpeningTime
        {
            get; set;
        }

        [DataMember]
        public ushort SinusCurrent
        {
            get; set;
        }

        [DataMember]
        public ushort SinusTime
        {
            get; set;
        }

        [DataMember]
        public ushort CurveCurrent
        {
            get; set;
        }

        [DataMember]
        public ushort CurveTime
        {
            get; set;
        }

        [DataMember]
        public ushort CurveFactor
        {
            get; set;
        }

        [DataMember]
        public ushort CurveAddTime
        {
            get; set;
        }

        [DataMember]
        public bool UseFullScale
        {
            get; set;
        }

        [DataMember]
        public bool UseLsqMethod
        {
            get; set;
        }

        [DataMember]
        public ushort Count
        {
            get; set;
        }

        [DataMember]
        public float VTM
        {
            get; set;
        }

        [DataMember]
        public bool IsSelfTest { get; set; }

        /// <summary>�������� ��������� � ����������</summary>
        /// <param name="oldParameters">������ ���������</param>
        /// <returns>���������� True, ���� ��������� ���� ��������</returns>
        public override bool HasChanges(BaseTestParametersAndNormatives oldParameters)
        {
            //������ ���������
            TestParameters OldTestParameters = (TestParameters)oldParameters;
            if (oldParameters == null)
                throw new InvalidCastException("OldParameters must be SLOldParameters");
            if (TestType != OldTestParameters.TestType)
                return true;
            if (RampCurrent != OldTestParameters.RampCurrent)
                return true;
            if (RampTime != OldTestParameters.RampTime)
                return true;
            if (IsRampOpeningEnabled != OldTestParameters.IsRampOpeningEnabled)
                return true;
            if (RampOpeningCurrent != OldTestParameters.RampOpeningCurrent)
                return true;
            if (RampOpeningTime != OldTestParameters.RampOpeningTime)
                return true;
            if (SinusCurrent != OldTestParameters.SinusCurrent)
                return true;
            if (SinusTime != OldTestParameters.SinusTime)
                return true;
            if (CurveCurrent != OldTestParameters.CurveCurrent)
                return true;
            if (CurveTime != OldTestParameters.CurveTime)
                return true;
            if (CurveFactor != OldTestParameters.CurveFactor)
                return true;
            if (CurveAddTime != OldTestParameters.CurveAddTime)
                return true;
            if (UseFullScale != OldTestParameters.UseFullScale)
                return true;
            if (UseLsqMethod != OldTestParameters.UseLsqMethod)
                return true;
            if(Count != OldTestParameters.Count)
                return true;
            if (VTM != OldTestParameters.VTM)
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
            VTM = 2.5f;
        }

        [DataMember]
        public float VTM
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
            ITMArray = new List<short>();
            DesiredArray = new List<short>();
            VTMArray = new List<short>();
            CapacitorsArray = new List<float>();

            SelfTestArray = new List<short>();
        }

        [DataMember]
        public float Voltage
        {
            get; set;
        }

        [DataMember]
        public ushort Current
        {
            get; set;
        }

        [DataMember]
        public IList<short> ITMArray
        {
            get; set;
        }

        [DataMember]
        public IList<short> DesiredArray
        {
            get; set;
        }

        [DataMember]
        public IList<short> VTMArray
        {
            get; set;
        }
        
        [DataMember]
        public IList<float> CapacitorsArray
        {
            get; set;
        }

        [DataMember]
        public IList<short> SelfTestArray
        {
            get; set;
        }

        [DataMember]
        public bool IsSelftest
        {
            get; set;
        }
    }

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
        /// <summary>������� �������</summary>
        [EnumMember]
        BatteryCharge = 3,
        [EnumMember]
        /// <summary>����������</summary>
        Ready = 4,
        /// <summary>� ��������</summary>
        [EnumMember]
        InProcess = 5
    };

    /// <summary>������� ������</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public enum HWFaultReason
    {
        [EnumMember]
        None = 0,
        /// <summary>������ ����������</summary>
        [EnumMember]
        Interface = 1,
        /// <summary>����������� ���������</summary>
        [EnumMember]
        PCUnexpectedState = 2,
        /// <summary>�������</summary>
        [EnumMember]
        PCStateTimeout = 3,
        /// <summary>������������ ����</summary>
        [EnumMember]
        PCCurrentConfig = 4
    };

    /// <summary>������� ��������������</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public enum HWWarningReason
    {
        [EnumMember]
        None = 0,
        /// <summary>���������� ��� ���������</summary>
        [EnumMember]
        VoltageOutOfRange = 1,
        /// <summary>��� ��� ���������</summary>
        [EnumMember]
        CurrentOutOfRange = 2
    };

    /// <summary>������� ����������</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public enum HWDisableReason
    {
        [EnumMember]
        None = 0,
        /// <summary>������ ����������</summary>
        [EnumMember]
        Interface = 1,
        /// <summary>����������� ���������</summary>
        [EnumMember]
        PCUnexpectedState = 2,
        /// <summary>�������</summary>
        [EnumMember]
        PCStateTimeout = 3,
        /// <summary>������������ ����</summary>
        [EnumMember]
        PCCurrentConfig = 4
    };

    /// <summary>������� ��������</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public enum HWProblemReason
    {
        [EnumMember]
        None = 0
    };

    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public enum HWOperationResult
    {
        [EnumMember]
        None = 0,
        /// <summary>�������</summary>
        [EnumMember]
        OK = 1,
        /// <summary>��������</summary>
        [EnumMember]
        Fail = 2
    };





    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public class CalibrationParameters
    {
        [DataMember]
        public ushort ItmFineN { get; set; }

        [DataMember]
        public ushort ItmFineD { get; set; }

        [DataMember]
        public ushort VtmFineN { get; set; }

        [DataMember]
        public ushort VtmFineD { get; set; }

        [DataMember]
        public short VtmOffset { get; set; }

        [DataMember]
        public ushort PredictParamK1 { get; set; }

        [DataMember]
        public ushort PredictParamK2 { get; set; }

        [DataMember]
        public ushort VtmFine2N { get; set; }

        [DataMember]
        public ushort VtmFine2D { get; set; }
    }
}