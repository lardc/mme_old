using System.Runtime.Serialization;

namespace SCME.Types.Clamping
{
    /// <summary>Состояние сжатия пресса</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public enum SqueezingState
    {
        /// <summary>Сжат</summary>
        [EnumMember]
        Down = 0,
        /// <summary>Сжимает</summary>
        [EnumMember]
        Squeezing,
        /// <summary>Расжат</summary>
        [EnumMember]
        Up,
        /// <summary>Расжимает</summary>
        [EnumMember]
        Unsqueezing,
        /// <summary>Неопределенное состояние</summary>
        [EnumMember]
        Undeterminated,
        /// <summary>Нагрев</summary>
        [EnumMember]
        Heating,
        /// <summary>Обновление</summary>
        [EnumMember]
        Updating
    }

    /// <summary>Сила сжатия</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public enum ClampingForce
    {
        /// <summary>Контакт</summary>
        [EnumMember]
        Contact = 0,
        /// <summary>Заданная</summary>
        [EnumMember]
        Custom
    }
}