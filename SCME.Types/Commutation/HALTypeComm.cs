﻿using System.Runtime.Serialization;

namespace SCME.Types.Commutation
{
    /// <summary>Режим коммутации</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public enum CommutationMode
    {
        [EnumMember]
        None = 0,
        [EnumMember]
        GTU,
        [EnumMember]
        SL,
        [EnumMember]
        BVTD,
        [EnumMember]
        BVTR,
        [EnumMember]
        DVDT,
        [EnumMember]
        ATU,
        [EnumMember]
        TOU
    }

    /// <summary>Тип коммутации</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public enum ModuleCommutationType
    {
        [EnumMember]
        Direct = 0x00,
        [EnumMember]
        MT1 = 0x01,
        [EnumMember]
        MD1 = 0x02,
        [EnumMember]
        MT3 = 0x03,
        [EnumMember]
        MT4 = 0x04,
        [EnumMember]
        MT5 = 0x05,
        [EnumMember]
        MD3 = 0x06,
        [EnumMember]
        MD4 = 0x07,
        [EnumMember]
        MD5 = 0x08,
        [EnumMember]
        MTD3 = 0x09,
        [EnumMember]
        MDT3 = 0x0A,
        [EnumMember]
        MTD4 = 0x0B,
        [EnumMember]
        MDT4 = 0x0C,
        [EnumMember]
        MTD5 = 0x0D,
        [EnumMember]
        MDT5 = 0x0E,
        [EnumMember]
        Reverse = 0x0F
    }

    /// <summary>Тип модуля</summary>
    public enum ModuleType
    {
        A2 = 1001,
        C1 = 1002,
        E0 = 1003,
        F1 = 1004,
        D0 = 1005,
        B1 = 1006
    }

    /// <summary>Позиция модуля</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public enum ModulePosition
    {
        /// <summary>Позиция 1</summary>
        [EnumMember]
        P1 = 0,
        /// <summary>Позиция 2</summary>
        [EnumMember]
        P2
    }
}