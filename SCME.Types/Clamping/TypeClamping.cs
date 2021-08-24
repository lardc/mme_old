using System.Runtime.Serialization;

namespace SCME.Types.Clamping
{
    /// <summary>Параметры произведения тестов</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public class TestParameters
    {
        /// <summary>Пропуск сжатия</summary>
        [DataMember]
        public bool SkipClamping
        {
            get; set;
        }

        /// <summary>Стандартная сила сжатия</summary>
        [DataMember]
        public ClampingForceInternal StandardForce
        {
            get; set;
        }

        /// <summary>Заданная сила сжатия</summary>
        [DataMember]
        public float CustomForce
        {
            get; set;
        }

        /// <summary>Необходимость измерения высоты</summary>
        [DataMember]
        public bool IsHeightMeasureEnabled
        {
            get; set;
        }

        /// <summary>Высота прибора</summary>
        [DataMember]
        public ushort Height
        {
            get; set;
        }
    }

    /// <summary>Состояние оборудования</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public enum HWDeviceState
    {
        /// <summary>Неопределенное состояние</summary>
        [EnumMember]
        None = 0,
        /// <summary>Ошибка</summary>
        [EnumMember]
        Fault = 1,
        /// <summary>Выключен</summary>
        [EnumMember]
        Disabled = 2,
        /// <summary>Готов</summary>
        [EnumMember]
        Ready = 3,
        /// <summary>Ожидает</summary>
        [EnumMember]
        Halt = 4,
        /// <summary>Хоминг</summary>
        [EnumMember]
        Homing = 5,
        /// <summary>Позиция</summary>
        [EnumMember]
        Position = 6,
        /// <summary>Сжатие</summary>
        [EnumMember]
        Clamping = 7,
        /// <summary>Сжатие выполнено</summary>
        [EnumMember]
        ClampingDone = 8,
        /// <summary>Обновление сжатия</summary>
        [EnumMember]
        ClampingUpdate = 9,
        /// <summary>Ослабление сжатия</summary>
        [EnumMember]
        ClampingRelease = 10,
        /// <summary>Скольжение</summary>
        [EnumMember]
        Sliding = 11
    };

    /// <summary>Причина ошибки</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public enum HWFaultReason
    {
        [EnumMember]
        None = 0,
        /// <summary>Ошибка термосистемы</summary>
        [EnumMember]
        Thermosystem = 2,
        /// <summary>Ошибка CANOpen высокого уровня</summary>
        [EnumMember]
        CANOpen = 3,
        /// <summary>Ошибка коммуникации с ТРМ</summary>
        [EnumMember]
        TRM = 4,
        /// <summary>Низкое давление</summary>
        [EnumMember]
        Pressure = 5,
        /// <summary>Ошибка системы скольжения</summary>
        [EnumMember]
        Sliding = 6
    };

    /// <summary>Причина предупреждения</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public enum HWWarningReason
    {
        [EnumMember]
        None = 0,
        /// <summary>Система перезагружена watchdog'ом</summary>
        [EnumMember]
        WatchdogReset = 1001
    };

    /// <summary>Причина выключения</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public enum HWDisableReason
    {
        [EnumMember]
        None = 0,
        /// <summary>Ошибка инвертора</summary>
        [EnumMember]
        LenzeError = 1
    };

    /// <summary>Причина проблемы</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public enum HWProblemReason
    {
        [EnumMember]
        None = 0,
        /// <summary>Не достигнута сила сжатия</summary>
        [EnumMember]
        NoForce = 1,
        /// <summary>Нет воздушного давления</summary>
        [EnumMember]
        NoAirPressure = 2
    };





    /// <summary>Тип системы пресса</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public enum ClampingSystemType
    {
        [EnumMember]
        Presspack = 0,
        [EnumMember]
        Stud = 1,
        [EnumMember]
        Module = 2,
        /// <summary>Система игнорируется</summary>
        [EnumMember]
        Ignored = 3
    }

    /// <summary>Канал нагрева пресса</summary>
    public enum HeatingChannel
    {
        [EnumMember]
        Top = 0,
        [EnumMember]
        Bottom = 1,
        [EnumMember]
        Setting = 2
    }

    /// <summary>Внутренняя сила сжатия</summary>
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