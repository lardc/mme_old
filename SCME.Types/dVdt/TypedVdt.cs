using SCME.Types.BaseTestParams;
using System;
using System.Runtime.Serialization;

namespace SCME.Types.dVdt
{
    /// <summary>Параметры произведения тестов</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public class TestParameters : BaseTestParametersAndNormatives, ICloneable
    {
        /// <summary>Инициализирует новый экземпляр класса TestParameters</summary>
        public TestParameters()
        {
            TestParametersType = TestParametersType.dVdt;
            IsEnabled = true;
            Voltage = 1000;
            VoltageRate = VoltageRate.V500;
            Mode = DvdtMode.Confirmation;
            ConfirmationCount = 1;
            VoltageRateLimit = 1000;
            VoltageRateOffSet = 100;
        }

        [DataMember]
        public ushort Voltage
        {
            get; set;
        }

        [DataMember]
        public DvdtMode Mode
        {
            get; set;
        }

        [DataMember]
        public VoltageRate VoltageRate
        {
            get; set;
        }

        [DataMember]
        public ushort ConfirmationCount
        {
            get; set;
        }

        [DataMember]
        public ushort VoltageRateLimit
        {
            get; set;
        }

        [DataMember]
        public ushort VoltageRateOffSet
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
                throw new InvalidCastException("OldParameters must be dVdtOldParameters");
            if (Mode != OldTestParameters.Mode)
                return true;
            if (VoltageRate != OldTestParameters.VoltageRate)
                return true;
            if (Voltage != OldTestParameters.Voltage)
                return true;
            if (ConfirmationCount != OldTestParameters.ConfirmationCount)
                return true;
            if (VoltageRateLimit != OldTestParameters.VoltageRateLimit)
                return true;
            if (VoltageRateOffSet != OldTestParameters.VoltageRateOffSet)
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
            Passed = false;
            VoltageRate = 0;
        }

        [DataMember]
        public bool Passed
        {
            get; set;
        }

        [DataMember]
        public ushort VoltageRate
        {
            get; set;
        }

        [DataMember]
        public DvdtMode Mode
        {
            get; set;
        }
    }

    /// <summary>Состояние оборудования</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public enum HWDeviceState
    {
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
        Powered = 3
    };

    /// <summary>Причина ошибки</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public enum HWFaultReason
    {
        [EnumMember]
        None = 0
    };

    /// <summary>Причина предупреждения</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public enum HWWarningReason
    {
        [EnumMember]
        None = 0
    };

    /// <summary>Причина выключения</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public enum HWDisableReason
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





    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public class CalibrationParams
    {
        [DataMember]
        public ushort VFineN
        {
            get; set;
        }

        [DataMember]
        public ushort VFineD
        {
            get; set;
        }

        [DataMember]
        public ushort V500
        {
            get; set;
        }

        [DataMember]
        public ushort V1000
        {
            get; set;
        }

        [DataMember]
        public ushort V1500
        {
            get; set;
        }

        [DataMember]
        public ushort V2000
        {
            get; set;
        }

        [DataMember]
        public ushort V2500
        {
            get; set;
        }
    }
}