using System.Runtime.Serialization;

namespace SCME.Types.Commutation
{
    /// <summary>��������� ������������ ������</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public class TestParameters
    {
        [DataMember]
        public HWModulePosition Position
        {
            get; set;
        }

        [DataMember]
        public HWBlockIndex BlockIndex
        {
            get; set;
        }

        [DataMember]
        public HWModuleCommutationType CommutationType
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
        /// <summary>�������</summary>
        [EnumMember]
        Enabled = 3,
        /// <summary>������ ������������ �������</summary>
        [EnumMember]
        SafetyActive = 4,
        /// <summary>������� ������ ������������</summary>
        [EnumMember]
        SafetyTrig = 5
    };

    /// <summary>������� ������</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public enum HWFaultReason
    {
        [EnumMember]
        None = 0,
        /// <summary>������ ��������</summary>
        [EnumMember]
        LowPressure = 302
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
        /// <summary>�������� � ������� ������������</summary>
        [EnumMember]
        BadClock = 1001
    };

    /// <summary>������� ��������</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public enum HWProblemReason
    {
        [EnumMember]
        None = 0
    };

   
    
    /// <summary>������� ������</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public enum HWModulePosition
    {
        /// <summary>������� 1</summary>
        [EnumMember]
        Position1 = 1,
        /// <summary>������� 2</summary>
        [EnumMember]
        Position2 = 2
    };

    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public enum HWBlockIndex
    {
        [EnumMember] Block1 = 1,
        [EnumMember] Block2 = 2
    };

    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public enum HWModuleCommutationType
    {
        [EnumMember]
        [EnumUseRCC(false)]
        Direct = 0x00,
        [EnumMember]
        [EnumUseRCC(false)]
        MT1 = 0x01,
        [EnumMember]
        [EnumUseRCC(false)]
        MD1 = 0x02,
        [EnumMember]
        [EnumUseRCC(false)]
        MT3 = 0x03,
        [EnumMember]
        [EnumUseRCC(false)]
        MT4 = 0x04,
        [EnumMember]
        [EnumUseRCC(true)]
        MT5 = 0x05,
        [EnumMember]
        [EnumUseRCC(false)]
        MD3 = 0x06,
        [EnumMember]
        [EnumUseRCC(false)]
        MD4 = 0x07,
        [EnumMember]
        [EnumUseRCC(false)]
        MD5 = 0x08,
        [EnumMember]
        [EnumUseRCC(false)]
        MTD3 = 0x09,
        [EnumMember]
        [EnumUseRCC(false)]
        MDT3 = 0x0A,
        [EnumMember]
        [EnumUseRCC(false)]
        MTD4 = 0x0B,
        [EnumMember]
        [EnumUseRCC(false)]
        MDT4 = 0x0C,
        [EnumMember]
        [EnumUseRCC(true)]
        MTD5 = 0x0D,
        [EnumMember]
        [EnumUseRCC(true)]
        MDT5 = 0x0E,
        [EnumMember]
        [EnumUseRCC(false)]
        Reverse = 0x0F,
        [EnumMember]
        [EnumUseRCC(false)]
        Undefined = 0xFF
    }
}