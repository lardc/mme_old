﻿using SCME.Types;
using SCME.Types.ATU;
using SCME.UIServiceConfig.Properties;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SCME.Service.IO
{
    internal class IOATU
    {
        private const int REQUEST_DELAY_MS = 50;

        private readonly IOAdapter m_IOAdapter;
        private readonly BroadcastCommunication m_Communication;
        private readonly ushort m_Node;
        private readonly bool m_IsATUEmulationHard;
        private bool m_IsATUEmulation;
        private IOCommutation m_IOCommutation;
        private TestParameters m_Parameters;
        private DeviceConnectionState m_ConnectionState;
        private volatile Types.DeviceState m_State;
        private volatile TestResults m_Result;
        private volatile bool m_Stop;

        private int m_Timeout = 30000;

        internal IOATU(IOAdapter Adapter, BroadcastCommunication Communication)
        {
            m_IOAdapter = Adapter;
            m_Communication = Communication;
            m_IsATUEmulationHard = Settings.Default.ATUEmulation;
            m_IsATUEmulation = m_IsATUEmulationHard;
            m_Node = (ushort)Settings.Default.ATUNode;
            m_Result = new TestResults();
            SystemHost.Journal.AppendLog(ComplexParts.ATU, LogMessageType.Milestone, string.Format("ATU created. Emulation mode: {0}", Settings.Default.ATUEmulation));
        }

        internal IOCommutation ActiveCommutation
        {
            get => m_IOCommutation;
            set
            {
                m_IOCommutation = value;
            }
        }

        internal DeviceConnectionState Initialize(bool Enable, int Timeout)
        {
            m_IsATUEmulation = m_IsATUEmulationHard || !Enable;
            m_ConnectionState = DeviceConnectionState.ConnectionInProcess;
            FireConnectionEvent(m_ConnectionState, "ATU initializing");
            if (m_IsATUEmulation)
            {
                m_ConnectionState = DeviceConnectionState.ConnectionSuccess;
                FireConnectionEvent(m_ConnectionState, "ATU initialized", LogMessageType.Milestone);
                return m_ConnectionState;
            }
            try
            {
                //для исполнения процедуры инициализации необходимо, чтобы блок ATU находился в состоянии DS_NONE, поэтому прежде чем мы начнём исполнять цикл конечного автомата, реализующий инициализацию ATU переведём ATU в состояние DS_None
                if (ReadRegister(REG_DEV_STATE) != (ushort)Types.ATU.HWDeviceState.DS_None)
                    CallAction(ACT_DISABLE_POWER);
                //ATU должен быть в состоянии DS_NONE. в принципе можно было бы сразу проверить состояние блока ATU, но мы будем выдерживать таймаут m_Timeout, за время истечения которого блок ATU должен выйти в состояние DS_None. если такового не случится - будем возбуждать исключительную ситуацию
                Types.ATU.HWDeviceState WaitedState = Types.ATU.HWDeviceState.DS_None;
                ushort State;
                bool End = false;
                while (!End)
                {
                    //чтение переменных конечного автомата
                    State = ReadDeviceState(WaitedState, Timeout);
                    if (State == (ushort)WaitedState)
                    {
                        switch (State)
                        {
                            case (ushort)HWDeviceState.DS_None:
                                //блок ATU перешёл в состояние DS_None
                                //очищаем ошибки блока ATU
                                ClearErrors();

                                //включаем заряд внутренних накопителей
                                EnablePower();

                                //из данного состояния ATU должен перейти в состояние DS_BatteryCharge но см. http://elma.pe.local/Projects/ProjectTask/Execute/82923 комментарий (Мороз Е. В. 13.04.2017 13:00:51)
                                WaitedState = Types.ATU.HWDeviceState.DS_Ready;
                                break;

                            case (ushort)HWDeviceState.DS_BatteryCharge:
                                //из данного состояния ATU должен перейти в состояние DS_Ready
                                WaitedState = Types.ATU.HWDeviceState.DS_Ready;
                                break;

                            case (ushort)HWDeviceState.DS_Ready:
                                //блок ATU перешёл в состояние DS_Ready - завершаем процесс инициализации
                                m_ConnectionState = DeviceConnectionState.ConnectionSuccess;
                                End = true;
                                FireConnectionEvent(m_ConnectionState, "ATU initialized.", LogMessageType.Milestone);
                                break;
                        }
                    }
                    else
                    {
                        //состояние блока ATU отличное от ожидаемого: ожидалось состояние WaitedState, а по факту оказалось состояние State
                        switch (State)
                        {
                            case (ushort)HWDeviceState.DS_Fault:
                                //сбрасываем состояние DS_Fault
                                ClearFault();

                                //после сброса состояния DS_Fault оно всегда будет DS_None
                                WaitedState = Types.ATU.HWDeviceState.DS_None;
                                break;

                            case (ushort)HWDeviceState.DS_Disabled:
                                throw new Exception(string.Format("state is 'DS_Disabled', reason: {0}", ReadRegister(REG_DISABLE_REASON)));

                            default:
                                throw new Exception(string.Format("state is '{0}', waited state is '{1}'", ((Types.ATU.HWDeviceState)State).ToString(), ((Types.ATU.HWDeviceState)WaitedState).ToString()));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                m_ConnectionState = DeviceConnectionState.ConnectionFailed;
                FireConnectionEvent(m_ConnectionState, String.Format("ATU initialization error: {0}.", e.Message), LogMessageType.Error);
            }

            return m_ConnectionState;
        }

        internal void Deinitialize()
        {
            var oldState = m_ConnectionState;

            m_ConnectionState = DeviceConnectionState.DisconnectionInProcess;
            FireConnectionEvent(DeviceConnectionState.DisconnectionInProcess, "ATU disconnecting");

            try
            {
                if (!m_IsATUEmulation && oldState == DeviceConnectionState.ConnectionSuccess)
                {
                    CallAction(ACT_DISABLE_POWER);
                }

                m_ConnectionState = DeviceConnectionState.DisconnectionSuccess;
                FireConnectionEvent(DeviceConnectionState.DisconnectionSuccess, "ATU disconnected", LogMessageType.Milestone);
            }
            catch (Exception)
            {
                m_ConnectionState = DeviceConnectionState.DisconnectionError;
                FireConnectionEvent(DeviceConnectionState.DisconnectionError, "ATU disconnection error", LogMessageType.Error);
            }
        }

        internal Types.DeviceState Start(TestParameters Parameters, Types.Commutation.TestParameters commParameters)
        {
            m_Parameters = Parameters;

            if (m_State == Types.DeviceState.InProcess) throw new Exception("ATU test is already started.");

            m_Result = new TestResults();
            m_Result.TestTypeId = m_Parameters.TestTypeId;

            m_Stop = false;

            if (!m_IsATUEmulation)
            {
                ushort State = ReadRegister(REG_DEV_STATE);

                switch (State)
                {
                    case (ushort)HWDeviceState.DS_Fault:
                        ushort faultReason = ReadRegister(REG_FAULT_REASON);

                        FireNotificationEvent((ushort)HWWarningReason.None, faultReason, (ushort)HWDisableReason.None);

                        throw new Exception(string.Format("ATU is in 'DS_Fault' state, reason: {0}", faultReason));

                    case (ushort)HWDeviceState.DS_Disabled:
                        ushort disableReason = ReadRegister(REG_DISABLE_REASON);

                        FireNotificationEvent((ushort)HWWarningReason.None, (ushort)HWFaultReason.None, disableReason);

                        throw new Exception(string.Format("ATU is in 'DS_Disabled' state, reason: {0}", disableReason));
                }
            }

            MeasurementLogicRoutine(commParameters);

            return m_State;
        }

        internal void Stop()
        {
            m_Stop = true;
        }

        internal ushort ReadDeviceState(Types.ATU.HWDeviceState WaitedState, int Timeout)
        //реализация чтения состояния конечного автомата.
        //в WaitedState принимается состояние, в которое должен перейти конечный автомат ATU
        //реализация ожидает перехода конечного автомата в состояние WaitedState в течении времени Timeout
        //реализация возвращает считанный номер состояния конечного автомата
        {
            ushort State = ReadRegister(REG_DEV_STATE);

            if (State == (ushort)WaitedState) return State;
            else
            {
                //пока не истёк таймаут - будем перечитывать состояние блока ATU через каждые 100 мс до тех пор, пока не окажемся в ожидаемом состоянии WaitedState
                var timeStamp = Environment.TickCount + Timeout;

                while (Environment.TickCount < timeStamp)
                {
                    if (m_Stop)
                    {
                        //ATU умеет команду Stop. при этом ATU перейдёт в состояние DS_Ready
                        CallAction(ACT_STOP_TEST);
                    }

                    Thread.Sleep(100);

                    State = ReadRegister(REG_DEV_STATE);

                    //считано состояние State, равное ожидаемому состоянию WaitedState - прерываем цикл ожидания
                    if (State == (ushort)WaitedState) return State;
                }

                //раз мы здесь - значит наступил таймаут, а состояния WaitedState мы так и не дождались
                return State;
            }
        }

        internal bool IsReadyToStart()
        {
            var devState = (Types.ATU.HWDeviceState)ReadRegister(REG_DEV_STATE);

            return !((devState == Types.ATU.HWDeviceState.DS_Fault) || (devState == Types.ATU.HWDeviceState.DS_Disabled) || (m_State == Types.DeviceState.InProcess));
        }

        private float RoundTwoDigits(double value)
        {
            //округление до второго знака после запятой и преобразование результата в тип float
            return (float)Math.Round(value, 2);
        }

        private void MeasurementLogicRoutine(Types.Commutation.TestParameters Commutation)
        {
            try
            {
                m_State = Types.DeviceState.InProcess;

                //перед измерением очищаем только Warning. сброс состояния Fault не делаем, т.к. он переводит ATU в состояние None
                ClearWarning();

                //уведомляем UI о том, что мы находимся в состоянии m_State с результатами измерений m_Result
                FireATUEvent(m_State, m_Result);

                //пишем значение амплитуды предварительного импульса тока в блок ATU
                WriteRegister(REG_PRE_PULSE_VALUE, m_Parameters.PrePulseValue);

                //пишем значение требуемой мощности в блок ATU
                WriteRegister(REG_POWER_VALUE, m_Parameters.PowerValue);

                if (m_IsATUEmulation)
                {
                    //эмулируем успешный результат измерений
                    m_State = Types.DeviceState.Success;
                    m_Result.UBR = 3000;   //В
                    m_Result.UPRSM = 1800; //В
                    m_Result.IPRSM = RoundTwoDigits(10257 / 1000d); //А   уровень отображения реализует вывод до 2 десятых
                    m_Result.PRSM = RoundTwoDigits(316 / 100d);   //в регистре сидит значение Bт/10. переводим его в кВт уровень отображения реализует вывод до 2 десятых

                    //проверяем обработку не удачного окончания измерения
                    FireNotificationEvent((ushort)HWWarningReason.Short, (ushort)HWFaultReason.None, (ushort)HWDisableReason.None);
                    FireNotificationEvent((ushort)HWWarningReason.None, (ushort)HWFaultReason.ChargeError, (ushort)HWDisableReason.None);
                }
                else
                {
                    //перед каждым измерением выполняем включение специальной коммутации для блока ATU для подключения требуемого измерительного блока к испытуемому прибору
                    if (m_IOCommutation.Switch(Types.Commutation.CommutationMode.ATU, Commutation.CommutationType, Commutation.Position) == Types.DeviceState.Fault)
                    {
                        m_State = Types.DeviceState.Fault;
                        //раз коммутация не удалась - выставляем значения всех измеряемых параметров в ноль
                        m_Result.UBR = 0;
                        m_Result.UPRSM = 0;
                        m_Result.IPRSM = 0;
                        m_Result.PRSM = 0;

                        FireATUEvent(m_State, m_Result);

                        return;
                    }

                    //запускаем процесс измерения
                    CallAction(ACT_START_TEST);

                    //ждём окончания процесса измерения
                    m_State = WaitForEndOfTest();

                    //считываем результаты измерения из блока ATU
                    m_Result.UBR = ReadRegisterS(REG_CASCADING_VOLTAGE_VALUE_MEASURE);
                    m_Result.UPRSM = ReadRegisterS(REG_VOLTAGE_VALUE_MEASURE);
                    m_Result.IPRSM = RoundTwoDigits(ReadRegisterS(REG_CURRENT_VALUE_MEASURE) / 1000d); //А уровень отображения реализует вывод до 2 десятых
                    m_Result.PRSM = RoundTwoDigits(ReadRegisterS(REG_POWER_VALUE_MEASURE) / 100d);     //в регистре сидит значение Bт/10. переводим его в кВт уровень отображения реализует вывод до 2 десятых

                    if (Settings.Default.ATUReadGraph)
                    {
                        m_Result.ArrayVDUT = ReadArrayFastS(ARR_SCOPE1_DATA);
                        m_Result.ArrayIDUT = ReadArrayFastS(ARR_SCOPE2_DATA);
                    }

                    //по окончании процесса измерения - отключаем коммутацию
                    if (m_IOCommutation.Switch(Types.Commutation.CommutationMode.None) == Types.DeviceState.Fault)
                    {
                        m_State = Types.DeviceState.Fault;

                        //коммутация не удалась, оставляем содержимое m_Result без изменения
                        FireATUEvent(m_State, m_Result);

                        return;
                    }
                }

                FireATUEvent(m_State, m_Result);
            }
            catch (Exception e)
            {
                m_IOCommutation.Switch(Types.Commutation.CommutationMode.None);
                m_State = Types.DeviceState.Fault;
                FireATUEvent(m_State, m_Result);
                FireExceptionEvent(e.Message);

                throw;
            }
        }

        private Types.DeviceState WaitForEndOfTest()
        {
            var timeStamp = Environment.TickCount + m_Timeout;

            while (Environment.TickCount < timeStamp)
            {
                if (m_Stop)
                {
                    CallAction(ACT_STOP_TEST);
                    return Types.DeviceState.Stopped;
                }

                ushort devState = ReadRegister(REG_DEV_STATE, true);

                //блок ATU перешёл в состояние DS_Fault
                if (devState == (ushort)Types.ATU.HWDeviceState.DS_Fault)
                {
                    ushort faultReason = ReadRegister(REG_FAULT_REASON);

                    FireNotificationEvent((ushort)HWWarningReason.None, faultReason, (ushort)HWDisableReason.None);
                }

                //блок ATU перешёл в состояние DS_Disabled
                if (devState == (ushort)Types.ATU.HWDeviceState.DS_Disabled)
                {
                    ushort disableReason = ReadRegister(REG_DISABLE_REASON);

                    FireNotificationEvent((ushort)HWWarningReason.None, (ushort)HWFaultReason.None, disableReason);
                }

                //блок ATU завершил процесс измерения
                if (devState == (ushort)Types.ATU.HWDeviceState.DS_Ready)
                {
                    //проверим наличие warnig от блока ATU
                    ushort warning = ReadRegister(REG_WARNING);

                    if (warning != (ushort)HWWarningReason.None)
                    {
                        FireNotificationEvent(warning, (ushort)HWFaultReason.None, (ushort)HWDisableReason.None);
                        ClearWarning();
                    }

                    break;
                }

                Thread.Sleep(REQUEST_DELAY_MS);
            }

            //мы вывалились из цикла ожидания окончания процесса измерения либо по break, либо по таймауту. убедимся, что мы уложились в отведённое время
            if (Environment.TickCount > timeStamp)
            {
                //цикл ожидания момента окончания измерений закончился позже отведённого нами времени - уведомляем все клиентские приложения и возбуждаем исключение
                FireExceptionEvent("File 'IOATU.cs'. Method 'WaitForEndOfTest'. Timeout while waiting for ATU test to end");
                throw new Exception("Timeout while waiting for ATU test to end");
            }

            //раз мы сюда добрались - значит ATU находится в адекватном состоянии и таймаут не наступил
            return Types.DeviceState.Success;
        }

        #region Standart API
        private void EnablePower()
        //включение зарядки конденсаторов блока ATU
        {
            SystemHost.Journal.AppendLog(ComplexParts.ATU, LogMessageType.Milestone, "ATU power is set to enable");
            CallAction(ACT_ENABLE_POWER);
        }

        internal void ClearFault()
        {
            //сброс состояния DS_Fault блока ATU
            CallAction(ACT_CLR_FAULT);
        }

        private void ClearWarning()
        {
            //очистка предупреждения блока ATU
            CallAction(ACT_CLR_WARNING);
        }

        private void ClearErrors()
        //очистка ошибок блока ATU
        {
            ClearFault();
            ClearWarning();
        }

        internal ushort ReadRegister(ushort Address, bool SkipJournal = false)
        //чтение ushort значения регистра с номером Address блока ATU
        {
            ushort value = 0;

            if (!m_IsATUEmulation) value = m_IOAdapter.Read16(m_Node, Address);

            if (!SkipJournal) SystemHost.Journal.AppendLog(ComplexParts.ATU, LogMessageType.Info, string.Format("ATU @ReadRegister, address {0}, value {1}", Address, value));

            return value;
        }

        internal short ReadRegisterS(ushort Address, bool SkipJournal = false)
        //чтение short значения регистра с номером Address блока ATU
        {
            short value = 0;

            if (!m_IsATUEmulation) value = m_IOAdapter.Read16S(m_Node, Address);

            if (!SkipJournal) SystemHost.Journal.AppendLog(ComplexParts.ATU, LogMessageType.Info, string.Format("ATU @ReadRegisterS, address {0}, value {1}", Address, value));

            return value;
        }

        internal void WriteRegister(ushort Address, float Value, bool SkipJournal = false)
        //запись в регистр с номером Address ushort значения Value
        //значение мощности в регистр надо писать в Вт/10, а интерфейс пользователя пишет в принимаемое на вход Value в кВт, поэтому данная реализация для случая записи значения мощности переводит принимаемое значение в Вт/10
        //принимаемое Value типа float только для возможности писать значение мощности с дробной частью
        {
            if (!SkipJournal) SystemHost.Journal.AppendLog(ComplexParts.ATU, LogMessageType.Info, string.Format("ATU @WriteRegister, address {0}, value {1}", Address, Value));

            if (m_IsATUEmulation) return;

            ushort value = 0;

            switch (Address)
            {
                case REG_POWER_VALUE:
                    //в пересчёте на Вт/10 это гарантированно не больше, чем позволяет ushort 65535
                    value = (ushort)(Convert.ToUInt16(Value*100));
                    break;

                default:
                    value = (ushort)Value;
                    break;
            }

            m_IOAdapter.Write16(m_Node, Address, value);
        }

        private IList<short> ReadArrayFastS(ushort Address)
        {
            SystemHost.Journal.AppendLog(ComplexParts.Gate, LogMessageType.Info,
                                         string.Format("ATU @ReadArrayFastS, endpoint {0}", Address));

            if (m_IsATUEmulation)
                return new List<short>();

            return m_IOAdapter.ReadArrayFast16S(m_Node, Address);
        }

        internal void CallAction(ushort Action)
        {
            SystemHost.Journal.AppendLog(ComplexParts.ATU, LogMessageType.Info, string.Format("ATU @Call, action {0}", Action));

            if (m_IsATUEmulation) return;

            m_IOAdapter.Call(m_Node, Action);
        }
        #endregion

        #region Events
        private void FireConnectionEvent(DeviceConnectionState State, string Message, LogMessageType type = LogMessageType.Info)
        {
            SystemHost.Journal.AppendLog(ComplexParts.ATU, type, Message);
            m_Communication.PostDeviceConnectionEvent(ComplexParts.ATU, State, Message);
        }

        private void FireNotificationEvent(ushort Warning, ushort Fault, ushort Disable)
        {
            SystemHost.Journal.AppendLog(ComplexParts.ATU, LogMessageType.Error, string.Format("ATU device notification: warning {0}, fault {1}, disable {2}", Warning, Fault, Disable));
            m_Communication.PostATUNotificationEvent(Warning, Fault, Disable);
        }

        private void FireATUEvent(Types.DeviceState State, TestResults Result)
        {
            string message = string.Format("ATU test state {0}", State);

            SystemHost.Journal.AppendLog(ComplexParts.ATU, LogMessageType.Info, message);
            m_Communication.PostATUEvent(State, Result);
        }

        private void FireExceptionEvent(string Message)
        {
            SystemHost.Journal.AppendLog(ComplexParts.ATU, LogMessageType.Error, Message);
            m_Communication.PostExceptionEvent(ComplexParts.ATU, Message);
        }
        #endregion

        #region Registers
        //описание регистров блока ATU
        private const ushort
        //регистры, задающие рабочий режим: 
        //значение амплитуды предварительного импульса тока, мА
        REG_PRE_PULSE_VALUE = 64,

        //значение требуемой мощности, Вт 
        REG_POWER_VALUE = 65,

        //регистры результата:
        //Измеренное значение напряжения лавинообразования(в трапеции тока) [В]
        REG_CASCADING_VOLTAGE_VALUE_MEASURE = 109,

        //измеренное значение напряжения в пике тока, В
        REG_VOLTAGE_VALUE_MEASURE = 110,

        //измеренное значение амплитуды тока, А 
        REG_CURRENT_VALUE_MEASURE = 111,

        //измеренное значение мощности, Вт 
        REG_POWER_VALUE_MEASURE = 112,

        //регистры статуса: 
        REG_DEV_STATE = 96,

        //причина аварии, если DeviceState -> FAULT
        REG_FAULT_REASON = 97,

        //причина аварии, если DeviceState -> DISABLED
        REG_DISABLE_REASON = 98,

        //предупреждение
        REG_WARNING = 99,

        ARR_SCOPE1_DATA = 1,
        ARR_SCOPE2_DATA = 2;
        #endregion

        #region Actions
        //описание команд блока ATU
        private const ushort
        //запуск процесса измерения
        ACT_START_TEST = 100,

        //останов процесса измерения
        ACT_STOP_TEST = 101,

        //включить питание блока
        ACT_ENABLE_POWER = 1,

        //выключить питание блока
        ACT_DISABLE_POWER = 2,

        //очистка ошибки
        ACT_CLR_FAULT = 3,

        //очистка предупреждения
        ACT_CLR_WARNING = 4;
        #endregion
    }
}