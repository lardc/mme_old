using System.Runtime.Serialization;

namespace SCME.Types.Clamping
{
    /// <summary>��������� ������������ ������</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public class TestParameters
    {
        /// <summary>������� ������</summary>
        [DataMember]
        public bool SkipClamping
        {
            get; set;
        }

        /// <summary>����������� ���� ������</summary>
        [DataMember]
        public ClampingForceInternal StandardForce
        {
            get; set;
        }

        /// <summary>�������� ���� ������</summary>
        [DataMember]
        public float CustomForce
        {
            get; set;
        }

        /// <summary>������������� ��������� ������</summary>
        [DataMember]
        public bool IsHeightMeasureEnabled
        {
            get; set;
        }

        /// <summary>������ �������</summary>
        [DataMember]
        public ushort Height
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
        /// <summary>�����</summary>
        [EnumMember]
        Ready = 3,
        /// <summary>�������</summary>
        [EnumMember]
        Halt = 4,
        /// <summary>������</summary>
        [EnumMember]
        Homing = 5,
        /// <summary>�������</summary>
        [EnumMember]
        Position = 6,
        /// <summary>������</summary>
        [EnumMember]
        Clamping = 7,
        /// <summary>������ ���������</summary>
        [EnumMember]
        ClampingDone = 8,
        /// <summary>���������� ������</summary>
        [EnumMember]
        ClampingUpdate = 9,
        /// <summary>���������� ������</summary>
        [EnumMember]
        ClampingRelease = 10,
        /// <summary>����������</summary>
        [EnumMember]
        Sliding = 11
    };

    /// <summary>������� ������</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public enum HWFaultReason
    {
        [EnumMember]
        None = 0,
        /// <summary>������ ������������</summary>
        [EnumMember]
        Thermosystem = 2,
        /// <summary>������ CANOpen �������� ������</summary>
        [EnumMember]
        CANOpen = 3,
        /// <summary>������ ������������ � ���</summary>
        [EnumMember]
        TRM = 4,
        /// <summary>������ ��������</summary>
        [EnumMember]
        Pressure = 5,
        /// <summary>������ ������� ����������</summary>
        [EnumMember]
        Sliding = 6
    };

    /// <summary>������� ��������������</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public enum HWWarningReason
    {
        [EnumMember]
        None = 0,
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
        /// <summary>������ ���������</summary>
        [EnumMember]
        LenzeError = 1
    };

    /// <summary>������� ��������</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public enum HWProblemReason
    {
        [EnumMember]
        None = 0,
        /// <summary>�� ���������� ���� ������</summary>
        [EnumMember]
        NoForce = 1,
        /// <summary>��� ���������� ��������</summary>
        [EnumMember]
        NoAirPressure = 2
    };





    /// <summary>��� ������� ������</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public enum ClampingSystemType
    {
        [EnumMember]
        Presspack = 0,
        [EnumMember]
        Stud = 1,
        [EnumMember]
        Module = 2,
        /// <summary>������� ������������</summary>
        [EnumMember]
        Ignored = 3
    }

    /// <summary>����� ������� ������</summary>
    public enum HeatingChannel
    {
        [EnumMember]
        Top = 0,
        [EnumMember]
        Bottom = 1,
        [EnumMember]
        Setting = 2
    }

    /// <summary>���������� ���� ������</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public enum ClampingForceInternal
    {
        [EnumMember] Contact = 0,
        [EnumMember] Custom
    }


    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public class CalibrationParams
    {
        [DataMember]
        public ushort ForceFineN { get; set; }

        [DataMember]
        public ushort ForceFineD { get; set; }

        [DataMember]
        public short ForceOffset { get; set; }
    }
}