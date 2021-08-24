using SCME.Types.BaseTestParams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace SCME.Types.QrrTq
{
    /// <summary>Параметры произведения тестов</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public class TestParameters : BaseTestParametersAndNormatives, ICloneable
    {
        /// <summary>Инициализирует новый экземпляр класса TestParameters</summary>
        public TestParameters()
        {
            TestParametersType = TestParametersType.QrrTq;
            IsEnabled = true;
            Mode = TMode.Qrr;
            DirectCurrent = 50;
            DCPulseWidth = 500;
            DCRiseRate = 0.2f;
            DCFallRate = TDcFallRate.r2;
            OffStateVoltage = 400;
            OsvRate = TOsvRate.r20;
            TrrMeasureBy9050Method = false;
        }

        /// <summary>Режим измерения</summary>
        [DataMember]
        public TMode Mode
        {
            get; set;
        }

        /// <summary>Амплитуда прямого тока</summary>
        [DataMember]
        public ushort DirectCurrent
        {
            get; set;
        }

        /// <summary>Длительность импульса прямого тока</summary>
        [DataMember]
        public ushort DCPulseWidth
        {
            get; set;
        }

        /// <summary>Скорость нарастания прямого тока</summary>
        [DataMember]
        public float DCRiseRate
        {
            get; set;
        }

        /// <summary>Скорость спада тока</summary>
        [DataMember]
        public TDcFallRate DCFallRate
        {
            get; set;
        }

        /// <summary>Прямое повторное напряжение</summary>
        [DataMember]
        public ushort OffStateVoltage
        {
            get; set;
        }

        /// <summary>Скорость нарастания прямого повторного напряжения</summary>
        [DataMember]
        public TOsvRate OsvRate
        {
            get; set;
        }

        /// <summary>Измерение trr по методу 90%-50%</summary>
        [DataMember]
        public bool TrrMeasureBy9050Method
        {
            get; set;
        }

        /// <summary>Фактическое значение прямого тока</summary>
        [DataMember]
        public short Idc
        {
            get; set;
        }

        /// <summary>Заряд обратного восстановления</summary>
        [DataMember]
        public short Qrr
        {
            get; set;
        }

        /// <summary>Ток обратного восстановления</summary>
        [DataMember]
        public short Irr
        {
            get; set;
        }

        /// <summary>Время обратного восстановления</summary>
        [DataMember]
        public short Trr
        {
            get; set;
        }

        [DataMember]
        /// <summary>Фактическая скорость спада тока</summary>
        public float DCFactFallRate
        {
            get; set;
        }

        /// <summary>Время выключения</summary>
        [DataMember]
        public short Tq
        {
            get; set;
        }

        /// <summary>Проверка изменений в параметрах</summary>
        /// <param name="oldParameters">Старые параметры</param>
        /// <returns>Возвращает True, если параметры были изменены</returns>
        public override bool HasChanges(BaseTestParametersAndNormatives oldParameters)
        {
            //Старые параметры
            TestParameters OldTestParameters = (TestParameters)oldParameters;
            if (oldParameters == null)
                throw new InvalidCastException("OldParameters must be QrrTqOldParameters");
            if (Mode != OldTestParameters.Mode)
                return true;
            if (DirectCurrent != OldTestParameters.DirectCurrent)
                return true;
            if (DCPulseWidth != OldTestParameters.DCPulseWidth)
                return true;
            if (DCRiseRate != OldTestParameters.DCRiseRate)
                return true;
            if (DCFallRate != OldTestParameters.DCFallRate)
                return true;
            if (OffStateVoltage != OldTestParameters.OffStateVoltage)
                return true;
            if (OsvRate != OldTestParameters.OsvRate)
                return true;
            if (TrrMeasureBy9050Method != OldTestParameters.TrrMeasureBy9050Method)
                return true;
            if (Idc != OldTestParameters.Idc)
                return true;
            if (Qrr != OldTestParameters.Qrr)
                return true;
            if (Irr != OldTestParameters.Irr)
                return true;
            if (Trr != OldTestParameters.Trr)
                return true;
            if (DCFactFallRate != OldTestParameters.DCFactFallRate)
                return true;
            if (Tq != OldTestParameters.Tq)
                return true;
            return false;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    /// <summary>Результаты тестирования</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public class TestResults : BaseTestResults
    {
        /// <summary>Инициализирует новый экземпляр класса TestResults</summary>
        public TestResults()
        {
            CurrentData = new List<short>();
            VoltageData = new List<short>();
        }

        /// <summary>Режим измерения</summary>
        [DataMember]
        public TMode Mode
        {
            get; set;
        }

        /// <summary>Прямое повторное напряжение</summary>
        [DataMember]
        public ushort OffStateVoltage
        {
            get; set;
        }

        /// <summary>Скорость нарастания прямого повторного напряжения</summary>
        [DataMember]
        public ushort OsvRate
        {
            get; set;
        }

        /// <summary>Фактическое значение прямого тока</summary>
        [DataMember]
        public short Idc
        {
            get; set;
        }

        /// <summary>Заряд обратного восстановления</summary>
        [DataMember]
        public float Qrr
        {
            get; set;
        }

        /// <summary>Ток обратного восстановления</summary>
        [DataMember]
        public short Irr
        {
            get; set;
        }

        /// <summary>Время обратного восстановления</summary>
        [DataMember]
        public float Trr { get; set; }

        [DataMember]
        //Фактическая скорость спада тока, А/мкс
        public float DCFactFallRate { get; set; }

        //Turn-off time (in us) – время выключения(в мкс)
        [DataMember]
        public float Tq { get; set; }

        //Данные для построения графика тока
        [DataMember]
        public List<short> CurrentData { get; set; }

        //Данные для построения графика напряжения
        [DataMember]
        public List<short> VoltageData { get; set; }
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
        /// <summary>Заряжен</summary>
        [EnumMember]
        PowerOn = 3,
        /// <summary>Готов</summary>
        [EnumMember]
        Ready = 4,
        /// <summary>В процессе</summary>
        [EnumMember]
        InProcess = 5
    };

    /// <summary>Причина ошибки</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public enum HWFaultReason
    {
        [EnumMember]
        None = 0,
        /// <summary>Ошибка в протоколе коммуникации</summary>
        [EnumMember]
        Protocol = 1,
        /// <summary>Ошибка в логике на высоком уровне</summary>
        [EnumMember]
        LogicGeneral = 2,
        /// <summary>Ошибка узла CROVU</summary>
        [EnumMember]
        LogicCROVU = 3,
        /// <summary>Ошибка узла FCROVU</summary>
        [EnumMember]
        LogicFCROVU = 4,
        /// <summary>Ошибка узла DCU1</summary>
        [EnumMember]
        LogicDCU1 = 5,
        /// <summary>Ошибка узла DCU2</summary>
        [EnumMember]
        LogicDCU2 = 6,
        /// <summary>Ошибка узла DCU3</summary>
        [EnumMember]
        LogicDCU3 = 7,
        /// <summary>Ошибка узла RCU1</summary>
        [EnumMember]
        LogicRCU1 = 8,
        /// <summary>Ошибка узла RCU2</summary>
        [EnumMember]
        LogicRCU2 = 9,
        /// <summary>Ошибка узла RCU3</summary>
        [EnumMember]
        LogicRCU3 = 10,
        /// <summary>Ошибка узла SCOPE</summary>
        [EnumMember]
        LogicSCOPE = 11,
        /// <summary>Нет давления</summary>
        [EnumMember]
        Pressure = 12
    }

    /// <summary>Причина предупреждения</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public enum HWWarningReason
    {
        [EnumMember]
        None = 0,
        /// <summary>Ручная остановка</summary>
        [EnumMember]
        ManualStop = 1,
        /// <summary>Нет прямого тока</summary>
        [EnumMember]
        NoDirectCurrent = 2,
        /// <summary>Ошибка вычислений узла SCOPE</summary>
        [EnumMember]
        SCOPECalcFailed = 3,
        /// <summary>Высокое значение обратного тока</summary>
        [EnumMember]
        IRRTooHigh = 4,
        /// <summary>Устройство не сменило состояние</summary>
        [EnumMember]
        DeviceTriggered = 5,
        /// <summary>Система перезагружена watchdog'ом</summary>
        [EnumMember]
        WatchdogReset = 1001
    }

    /// <summary>Причина выключения</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public enum HWDisableReason
    {
        [EnumMember]
        None = 0,
        /// <summary>Проблема с главным осциллятором</summary>
        [EnumMember]
        BadClock = 1001
    }

    /// <summary>Причина проблемы</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public enum HWProblemReason
    {
        [EnumMember]
        None = 0
    }

    /// <summary>Результат выполнения</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public enum HWOperationResult
    {
        [EnumMember]
        None = 0,
        /// <summary>Успешно</summary>
        [EnumMember]
        OK = 1,
        /// <summary>Неудачно</summary>
        [EnumMember]
        Fail = 2
    };





    /// <summary>Режим измерений</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public enum TMode
    {
        /// <summary>Обратное восстановление</summary>
        [EnumMember]
        Qrr = 0,
        /// <summary>Обратное восстановление и время выключения</summary>
        [EnumMember]
        QrrTq = 1
    };

    /// <summary>Скорость спада тока</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public enum TDcFallRate
    {
        [EnumMember]
        r2 = 2,
        [EnumMember]
        r5 = 5,
        [EnumMember]
        r10 = 10,
        [EnumMember]
        r15 = 15,
        [EnumMember]
        r20 = 20,
        [EnumMember]
        r30 = 30,
        [EnumMember]
        r50 = 50,
        [EnumMember]
        r60 = 60,
        [EnumMember]
        r100 = 100
    };

    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public class TDcFallRateHelper : object
    {
        public static Array EnumValues()
        {
            return Enum.GetValues(typeof(TDcFallRate)).Cast<uint>().Select(x => x.ToString()).ToArray();
        }
    }

    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public enum TOsvRate
    {
        [EnumMember]
        r20 = 20,
        [EnumMember]
        r50 = 50,
        [EnumMember]
        r100 = 100,
        [EnumMember]
        r200 = 200
    }

    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public class TOsvRateHelper : object
    {
        public static Array EnumValues()
        {
            return Enum.GetValues(typeof(TOsvRate)).Cast<uint>().Select(x => x.ToString()).ToArray();
        }
    }

}