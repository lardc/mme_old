using SCME.Types.BaseTestParams;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SCME.Types.ATU
{
    /// <summary>Параметры произведения тестов</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public class TestParameters : BaseTestParametersAndNormatives, ICloneable
    {
        /// <summary>Инициализирует новый экземпляр класса TestParameters</summary>
        public TestParameters()
        {
            TestParametersType = TestParametersType.ATU;
            IsEnabled = true;
            PrePulseValue = 100;
            PowerValue = 16;
        }

        [DataMember]
        public ushort PrePulseValue
        {
            get; set;
        }

        [DataMember]
        public float PowerValue
        {
            get; set;
        }

        [DataMember]
        public short UBR
        {
            get; set;
        }

        [DataMember]
        public short UPRSM
        {
            get; set;
        }

        [DataMember]
        public float IPRSM
        {
            get; set;
        }

        [DataMember] 
        public float PRSM_Min
        {
            get; set;
        } = 0;

        [DataMember]
        public float PRSM_Max
        {
            get; set;
        } = 70;

        /// <summary>Проверка изменений в параметрах</summary>
        /// <param name="oldParameters">Старые параметры</param>
        /// <returns>Возвращает True, если параметры были изменены</returns>
        public override bool HasChanges(BaseTestParametersAndNormatives oldParameters)
        {
            //Старые параметры
            TestParameters OldTestParameters = (TestParameters)oldParameters;
            if (oldParameters == null)
                throw new InvalidCastException("OldParameters must be ATUOldParameters");
            if (PrePulseValue != OldTestParameters.PrePulseValue)
                return true;
            if (PowerValue != OldTestParameters.PowerValue)
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
            ArrayIDUT = new List<short>();
            ArrayVDUT = new List<short>();
        }

        [DataMember]
        public short UBR
        {
            get; set;
        }

        [DataMember]
        public short UPRSM
        {
            get; set;
        }

        [DataMember]
        public float IPRSM
        {
            get; set;
        }

        [DataMember]
        public float PRSM
        {
            get; set;
        }

        [DataMember]
        public IList<short> ArrayIDUT
        {
            get; set;
        }

        [DataMember]
        public IList<short> ArrayVDUT
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
        /// <summary>Ожидание заряда батареи</summary>
        [EnumMember]
        BatteryCharge = 3,
        /// <summary>Состояние готовности</summary>
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
        /// <summary>Ошибка заряда батареи</summary>
        [EnumMember]
        Battery = 1,
        /// <summary>Ошибка заряда батареи мужду импульсами</summary>
        [EnumMember]
        BatteryP2P = 2,
        /// <summary>Ошибка регулирования мощности</summary>
        [EnumMember]
        FollowingError = 3
    };

    /// <summary>Причина предупреждения</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public enum HWWarningReason
    {
        [EnumMember]
        None = 0,
        /// <summary>ХХ на выходе</summary>
        [EnumMember]
        Idle = 1,
        /// <summary>КP на выходе</summary>
        [EnumMember]
        Short = 2,
        /// <summary>Погрешность полученной мощности велика</summary>
        [EnumMember]
        Accuracy = 3,
        /// <summary>Пробой прибора</summary>
        [EnumMember]
        Break = 4,
        /// <summary>Краевой пробой прибора</summary>
        [EnumMember]
        FacetBreak = 5
    };

    /// <summary>Причина выключения</summary>
    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public enum HWDisableReason
    {
        [EnumMember]
        None = 0,
        /// <summary>Ошибка заряда батареи</summary>
        [EnumMember]
        Battery = 1,
        /// <summary>Ошибка заряда батареи мужду импульсами</summary>
        [EnumMember]
        BatteryP2P = 2,
        /// <summary>Ошибка регулирования мощности</summary>
        [EnumMember]
        FollowingError = 3
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
    public class CalibrationParameters
    {

    }
}