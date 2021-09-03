﻿using SCME.Types;
using SCME.UIServiceConfig.Properties;
using System;
using System.Threading;

namespace SCME.UI.IO
{
    internal class IOCommutation
    {
        private readonly IOAdapter m_IOAdapter;
        private readonly BroadcastCommunication m_Communication;
        private readonly ThreadService m_CheckSafetyThread = null;
        private readonly ushort m_Node;
        private readonly bool m_IsCommutationEmulation, m_Type6;
        private readonly ComplexParts m_ID;
        private readonly ComplexSafety m_SafetyType = ComplexSafety.None;

        private bool m_SafetyAlarm = false;
        private bool m_FireSafetyEventCalled = false;

        private DeviceConnectionState m_ConnectionState;
        private const int REQUEST_DELAY_MS = 100;

        private bool m_SafetyOn = false;
        private SafetyMode _SafetyMode;

        internal IOCommutation(IOAdapter Adapter, BroadcastCommunication Communication, bool CommutationEmulation, int CommutationNode, bool Type6, ComplexParts ID)
        {
            m_ID = ID;
            m_IOAdapter = Adapter;
            m_Communication = Communication;
            m_IsCommutationEmulation = CommutationEmulation;

            //читаем из файла конфигурации тип установленной системы безопасности
            if (m_ID == ComplexParts.Commutation)
            {
                m_SafetyType = ReadSafetyTypeFromConfig();

                if (m_SafetyType == ComplexSafety.Optical)
                {
                    //если это оптическая шторка - создаём поток, который будет опрашивать её состояние
                    m_CheckSafetyThread = new ThreadService();
                    m_CheckSafetyThread.FinishedHandler += CheckSafetyThread_FinishedHandler;
                }
            }

            m_Node = (ushort)CommutationNode;
            m_Type6 = Type6;

            SystemHost.AppendLog(m_ID, LogMessageType.Milestone, String.Format("Commutation created. Emulation mode: {0}", m_IsCommutationEmulation));
        }

        internal DeviceConnectionState Initialize()
        {
            m_ConnectionState = DeviceConnectionState.ConnectionInProcess;
            FireConnectionEvent(m_ConnectionState, "Commutation initializing");

            if (m_IsCommutationEmulation)
            {
                m_ConnectionState = DeviceConnectionState.ConnectionSuccess;
                FireConnectionEvent(m_ConnectionState, "Commutation initialized", LogMessageType.Milestone);

                return m_ConnectionState;
            }

            try
            {
                ClearFault();
                ClearWarning();

                var devState = (Types.Commutation.HWDeviceState)GetDeviceState();

                if (devState != Types.Commutation.HWDeviceState.Enabled)
                {
                    if (devState == Types.Commutation.HWDeviceState.Fault)
                    {
                        ClearFault();
                        Thread.Sleep(1000);

                        devState = (Types.Commutation.HWDeviceState)GetDeviceState();

                        if (devState == Types.Commutation.HWDeviceState.Fault)
                            throw new Exception(string.Format("Device is in fault state, reason: {0}",
                                (Types.Commutation.HWFaultReason)ReadRegister(REG_FAULT_REASON)));
                    }

                    if (devState == Types.Commutation.HWDeviceState.Disabled)
                        throw new Exception(string.Format("Device is in disabled state, reason: {0}",
                                     (Types.Commutation.HWDisableReason)ReadRegister(REG_DISABLE_REASON)));
                }

                Thread.Sleep(1000);

                devState = (Types.Commutation.HWDeviceState)GetDeviceState();

                if (devState == Types.Commutation.HWDeviceState.Fault)
                    throw new Exception(string.Format("Device is in fault state, reason: {0}",
                                                        (Types.Commutation.HWFaultReason)ReadRegister(REG_FAULT_REASON)));
                if (devState == Types.Commutation.HWDeviceState.Disabled)
                    throw new Exception(string.Format("Device is in disabled state, reason: {0}",
                                                        (Types.Commutation.HWDisableReason)ReadRegister(REG_DISABLE_REASON)));

                if (devState == Types.Commutation.HWDeviceState.SafetyTrig || devState == Types.Commutation.HWDeviceState.SafetyActive)
                    CallAction(ACT_DISABLE_POWER);

                CallAction(ACT_ENABLE_POWER);

                if (m_Type6)
                {
                    WriteRegister(REG_MODULE_TYPE, (ushort)Types.Commutation.HWModuleCommutationType.MT3);
                    WriteRegister(REG_MODULE_POSITION,
                                        (ushort)Types.Commutation.HWModulePosition.Position1);
                    CallAction(ACT_COMM6_SL);
                    CallAction(ACT_COMM6_NONE);
                    WriteRegister(REG_MODULE_POSITION,
                                        (ushort)Types.Commutation.HWModulePosition.Position2);
                    CallAction(ACT_COMM6_SL);
                    CallAction(ACT_COMM6_NONE);
                }
                else
                {
                    CallAction(ACT_COMM2_SL);
                    CallAction(ACT_COMM2_NONE);
                }

                //если поток, контролирующий состояние оптической шторки создан, значит стенд оборудован оптической шторкой безопасности
                if (m_CheckSafetyThread != null)
                {
                    //запускаем поток контроля состояния системы коммутации если он ещё не работает
                    if (!m_CheckSafetyThread.IsRunning)
                    {
                        //поток не работает. в переменной осталось значение от предыдущего теста, сбрасываем его
                        m_SafetyAlarm = false;

                        //уведомляем UI о состоянии оптической шторки
                        FireSafetyEvent(m_SafetyAlarm);

                        m_CheckSafetyThread.StartCycle(SafetyThreadWorker, REQUEST_DELAY_MS);
                        SystemHost.AppendLog(m_ID, LogMessageType.Milestone, "Safety thread cycle started");
                    }
                }


                WriteRegister(REG_EN_SFTY_IN4, 0);
                SystemHost.AppendLog(m_ID, LogMessageType.Info, "REG_EN_SFTY_IN4 write state 0");

                m_ConnectionState = DeviceConnectionState.ConnectionSuccess;

                FireConnectionEvent(m_ConnectionState, "Commutation initialized", LogMessageType.Milestone);
            }
            catch (Exception ex)
            {
                m_ConnectionState = DeviceConnectionState.ConnectionFailed;
                FireConnectionEvent(m_ConnectionState,
                                    String.Format("Commutation initialization error: {0}", ex.Message));
            }

            return m_ConnectionState;
        }

        internal void Deinitialize()
        {
            var oldState = m_ConnectionState;

            m_ConnectionState = DeviceConnectionState.DisconnectionInProcess;
            FireConnectionEvent(DeviceConnectionState.DisconnectionInProcess, "Commutation disconnecting");

            try
            {
                if (!m_IsCommutationEmulation && oldState == DeviceConnectionState.ConnectionSuccess)
                    CallAction(ACT_DISABLE_POWER);

                if (m_CheckSafetyThread != null)
                {
                    //завершаем работу потока, контролирующего срабатывание оптической системы безопасности
                    m_SafetyOn = false;
                    m_CheckSafetyThread.StopCycle(true);
                }

                m_ConnectionState = DeviceConnectionState.DisconnectionSuccess;
                FireConnectionEvent(DeviceConnectionState.DisconnectionSuccess, "Commutation disconnected", LogMessageType.Milestone);
            }
            catch (Exception)
            {
                m_ConnectionState = DeviceConnectionState.DisconnectionError;
                FireConnectionEvent(DeviceConnectionState.DisconnectionError, "Commutation disconnection error", LogMessageType.Error);
            }
        }

        private ComplexSafety ReadSafetyTypeFromConfig()
        {
            ComplexSafety Result = ComplexSafety.None;

            //читаем из файла конфигурации тип установленной системы безопасности
            if (!Enum.TryParse(Settings.Default.SafetyType, out Result))
            {
                Result = ComplexSafety.None;
                SystemHost.AppendLog(ComplexParts.Commutation, LogMessageType.Error, "Unrecognised value on config parameter SafetyType");
            }

            return Result;
        }

        private void CheckSafetyThread_FinishedHandler(object Sender, ThreadFinishedEventArgs E)
        {
            //поток, контролирующий состояние оптической системы безопасности завершается выполнением данной реализации
            //выключаем аппаратный контроль за оптической системой безопасности
            SetSafetyOff();

            if (E.Error != null)
                m_Communication.PostExceptionEvent(ComplexParts.Service, E.Error.Message);
        }

        internal bool IsSafetyAlarm()
        {
            //если тип системы безопасности не оптическая шторка - то никакого Alarm и быть не может
            if ((m_SafetyType != ComplexSafety.Optical) || (m_ID != ComplexParts.Commutation))
                return false;

            //Если безопасность отключенна
            if (_SafetyMode == SafetyMode.Disabled)
                return false;

            //опрашиваем состояние блока коммутации и если оно SafetyTrig (сработала оптическая шторка) - возвращаем true, иначе false
            Types.Commutation.HWDeviceState devState = (Types.Commutation.HWDeviceState)GetDeviceState(true);

            return (devState == Types.Commutation.HWDeviceState.SafetyTrig);
        }

        private void SafetyThreadWorker()
        {
            //поток, который запускает данную потоковую функцию работает всегда
            if (m_SafetyOn)
            {
                //опрашиваем состояние блока коммутации и если оно SafetyTrig - значит было зафиксировано срабатывание шторки
                m_SafetyAlarm = IsSafetyAlarm();

                if (m_SafetyAlarm)
                {
                    if (!m_FireSafetyEventCalled)
                    {
                        //уведомляем UI о нарушении периметра безопасности
                        FireSafetyEvent(m_SafetyAlarm);

                        //мы уведомили UI о том, что оптическая шторка сработала. одного раза будет достаточно
                        m_FireSafetyEventCalled = true;
                    }
                }
            }
            else m_SafetyAlarm = false;
        }

        private void CheckSafetyOn()
        {
            //если в потоковой процедуре возникло исключение - то поток завершит свою работу, поэтому будем проверять состояние потока и при необходимости запускать его заново
            if (m_CheckSafetyThread != null)
            {
                //выставляем флаг о том, что мы ещё не уведомили UI о наступившем событии срабатывания оптической шторки
                m_FireSafetyEventCalled = false;

                m_SafetyOn = true;

                if (!m_CheckSafetyThread.IsRunning)
                    m_CheckSafetyThread.StartCycle(SafetyThreadWorker, REQUEST_DELAY_MS);
            }
        }

        private void CheckSafetyOff()
        {
            m_SafetyOn = false;
        }

        private ushort ReadDeviceStateWithAlarm(Types.Commutation.HWDeviceState WaitedState, int Timeout)
        //реализация чтения состояния конечного автомата
        //в WaitedState принимается состояние, в которое должен перейти конечный автомат 
        //реализация ожидает перехода конечного автомата в состояние WaitedState в течении времени Timeout
        //реализация возвращает считанный номер состояния конечного автомата
        //В случаи аларма прекращаем ожидание
        {
            if (m_IsCommutationEmulation)
                return (ushort)WaitedState;

            ushort State = GetDeviceState();

            if (State == (ushort)WaitedState) return State;
            else
            {
                //пока не истёк таймаут - будем перечитывать состояние блока коммутации через каждые 100 мс до тех пор, пока не окажемся в ожидаемом состоянии WaitedState
                var timeStamp = Environment.TickCount + Timeout;

                while (Environment.TickCount < timeStamp)
                {
                    Thread.Sleep(100);

                    State = GetDeviceState();

                    if((Types.Commutation.HWDeviceState)State == Types.Commutation.HWDeviceState.SafetyTrig)
                    {
                        m_SafetyAlarm = true;
                        FireSafetyEvent(m_SafetyAlarm);
                        return State;
                    }

                    //считано состояние State, равное ожидаемому состоянию WaitedState - прерываем цикл ожидания
                    if (State == (ushort)WaitedState) return State;
                }

                //раз мы здесь - значит наступил таймаут, а состояния WaitedState мы так и не дождались
                return State;
            }
        }

        public void SetSafetyMode(SafetyMode safetyMode) => _SafetyMode = safetyMode;

        private void WriteSafetyRegister()
        {
            if (SCME.UIServiceConfig.Properties.Settings.Default.IsWriteSafetyRegister == true)
                switch (_SafetyMode)
                {
                    case SafetyMode.Internal:
                        WriteRegister(REG_EN_SFTY_IN1, 1);
                        WriteRegister(REG_EN_SFTY_IN2, 1);
                        WriteRegister(REG_EN_SFTY_IN3, 0);
                        break;
                    case SafetyMode.External:
                        WriteRegister(REG_EN_SFTY_IN1, 0);
                        WriteRegister(REG_EN_SFTY_IN2, 1);
                        WriteRegister(REG_EN_SFTY_IN3, 1);
                        break;
                    case SafetyMode.Disabled:
                        WriteRegister(REG_EN_SFTY_IN1, 0);
                        WriteRegister(REG_EN_SFTY_IN2, 1);
                        WriteRegister(REG_EN_SFTY_IN3, 0);
                        break;
                    default:
                        throw new NotImplementedException("WriteSafetyRegister _SafetyMode != Internal && _SafetyMode != Internal");
                }
        }

        internal void SetSafetyOn()
        {
            if ((m_SafetyType == ComplexSafety.Optical) && (m_ID == ComplexParts.Commutation) && _SafetyMode != SafetyMode.Disabled)
            {
                //активация оптического датчика безопасности
                SystemHost.AppendLog(m_ID, LogMessageType.Info, "Try to commutation optical safety set to on");

                if (m_IsCommutationEmulation)
                {
                    if(SCME.UIServiceConfig.Properties.Settings.Default.AlarmEmulation)
                    {
                        m_SafetyAlarm = true;
                        FireSafetyEvent(m_SafetyAlarm);
                        //throw new Exception("Safety alarm");
                    }
                    return;
                }

                WriteSafetyRegister();

                CallAction(ACT_SAFETY_ON);

                //блок коммутации должен перейти в состояние SafetyActive. подождём его
                var WaitedState = Types.Commutation.HWDeviceState.SafetyActive;

                ushort FactState = ReadDeviceStateWithAlarm(WaitedState, 3000);

                if (FactState == (ushort)Types.Commutation.HWDeviceState.SafetyTrig)
                    return;
                if (FactState != (ushort)WaitedState)
                    throw new Exception(string.Format("Device is in a bad state. WaitedState={0}, factstate={1}.", (((ushort)WaitedState)).ToString(), FactState.ToString()));

                CheckSafetyOn();

                SystemHost.AppendLog(m_ID, LogMessageType.Milestone, "Commutation optical safety is on");
            }
        }

        internal void SetSafetyOff()
        {
            if ((m_SafetyType == ComplexSafety.Optical) && (m_ID == ComplexParts.Commutation))
            {
                //деактивация датчика безопасности
                CheckSafetyOff();

                SystemHost.AppendLog(m_ID, LogMessageType.Milestone, "Try to commutation optical safety set to off");



                if (m_IsCommutationEmulation)
                {
                    if (SCME.UIServiceConfig.Properties.Settings.Default.AlarmEmulation)
                        m_SafetyAlarm = false;
                    return;
                }

                WriteSafetyRegister();

                CallAction(ACT_SAFETY_OFF);

                //блок должен перейти в состояние PowerReady. подождём его
                var WaitedState = Types.Commutation.HWDeviceState.Enabled;

                ushort FactState = ReadDeviceStateWithAlarm(WaitedState, 3000);

                if (FactState == (ushort)Types.Commutation.HWDeviceState.SafetyTrig)
                    return;

                if (FactState != (ushort)WaitedState)
                     throw new Exception(string.Format("Device is in a bad state. WaitedState={0}, factstate={1}.", (((ushort)WaitedState)).ToString(), FactState.ToString()));
                    

                SystemHost.AppendLog(m_ID, LogMessageType.Milestone, "Commutation optical safety is off");
            }
        }

        internal bool GetButtonState()
        {
            //возвращает: false - датчик оптической шторки не сработал
            //            true - датчик оптической шторки сработал 
            return !m_IsCommutationEmulation || !m_SafetyAlarm;
        }

        #region Standart API


        internal void ClearFault()
        {
            SystemHost.AppendLog(m_ID, LogMessageType.Info, "Commutation fault cleared");

            if (m_IsCommutationEmulation)
                return;

            CallAction(ACT_CLEAR_FAULT);
        }

        private void ClearWarning()
        {
            SystemHost.AppendLog(m_ID, LogMessageType.Info, "Commutation warning cleared");

            if (m_IsCommutationEmulation)
                return;

            CallAction(ACT_CLEAR_WARNING);
        }

        private ushort GetDeviceState(bool SkipJournal = false)
        {
            return ReadRegister(REG_DEVICE_STATE, SkipJournal);
        }

        internal ushort ReadRegister(ushort Address, bool SkipJournal = false)
        {
            ushort value = 0;

            if (!m_IsCommutationEmulation)
                value = m_IOAdapter.Read16(m_Node, Address);

            if (!SkipJournal)
                SystemHost.AppendLog(m_ID, LogMessageType.Info,
                                         string.Format("Commutation @ReadRegister, address {0}, value {1}", Address,
                                                       value));

            return value;
        }

        internal void WriteRegister(ushort Address, ushort Value, bool SkipJournal = false)
        {
            if (!SkipJournal)
                SystemHost.AppendLog(m_ID, LogMessageType.Info,
                                         string.Format("Commutation @WriteRegister, address {0}, value {1}", Address,
                                                       Value));

            if (m_IsCommutationEmulation)
                return;

            m_IOAdapter.Write16(m_Node, Address, Value);
        }

        internal void CallAction(ushort Action)
        {
            SystemHost.AppendLog(m_ID, LogMessageType.Info,
                                         string.Format("Commutation @Call, action {0}", Action));

            if (m_IsCommutationEmulation)
                return;

            m_IOAdapter.Call(m_Node, Action);
        }

        #endregion

        internal DeviceState Switch(Types.Commutation.CommutationMode Mode,
                                  Types.Commutation.HWModuleCommutationType CommutationType =
                                      Types.Commutation.HWModuleCommutationType.Undefined,
                                  Types.Commutation.HWModulePosition Position =
                                      Types.Commutation.HWModulePosition.Position1)
        {
            if (m_IsCommutationEmulation)
                return DeviceState.Success;

            if (CommutationType == Types.Commutation.HWModuleCommutationType.Undefined)
                CommutationType = m_Type6 ? Types.Commutation.HWModuleCommutationType.MT3 : Types.Commutation.HWModuleCommutationType.Direct;

            try
            {
                var warning = (Types.Commutation.HWWarningReason)
                              ReadRegister(REG_WARNING);

                if (warning != Types.Commutation.HWWarningReason.None)
                {
                    FireNotificationEvent(warning, Types.Commutation.HWFaultReason.None);
                    ClearWarning();
                }

                var devState = (Types.Commutation.HWDeviceState)GetDeviceState();

                if (devState == Types.Commutation.HWDeviceState.Fault)
                {
                    FireNotificationEvent(Types.Commutation.HWWarningReason.None,
                                          (Types.Commutation.HWFaultReason)ReadRegister(REG_FAULT_REASON));
                    return DeviceState.Fault;
                }

                WriteRegister(REG_MODULE_TYPE, (ushort)CommutationType);
                WriteRegister(REG_MODULE_POSITION, (ushort)Position);

                switch (Mode)
                {
                    case Types.Commutation.CommutationMode.None:
                        CallAction(m_Type6 ? ACT_COMM6_NONE : ACT_COMM2_NONE);
                        break;
                    case Types.Commutation.CommutationMode.GTU:
                        CallAction(m_Type6 ? ACT_COMM6_GATE : ACT_COMM2_GATE);
                        break;
                    case Types.Commutation.CommutationMode.SL:
                        CallAction(m_Type6 ? ACT_COMM6_SL : ACT_COMM2_SL);
                        break;
                    case Types.Commutation.CommutationMode.BVTD:
                        CallAction(m_Type6 ? ACT_COMM6_BVT_D : ACT_COMM2_BVT_D);
                        break;
                    case Types.Commutation.CommutationMode.BVTR:
                        CallAction(m_Type6 ? ACT_COMM6_BVT_R : ACT_COMM2_BVT_R);
                        break;
                    case Types.Commutation.CommutationMode.DVDT:
                        //для dvdt может быть только коммутация ACT_COMM2_DVDT
                        CallAction(ACT_COMM2_DVDT);
                        break;
                    case Types.Commutation.CommutationMode.ATU:
                        //для ATU может быть только коммутация ACT_COMM2_ATU
                        CallAction(ACT_COMM2_ATU);
                        break;
                    case Types.Commutation.CommutationMode.TOU:
                        //для TOU может быть только коммутация ACT_COMM2_TOU
                        CallAction(ACT_COMM2_TOU);
                        break;
                }

                FireSwitchEvent(Mode, CommutationType, Position);

                return DeviceState.Success;
            }
            catch (Exception ex)
            {
                FireExceptionEvent(ex.Message);
                return DeviceState.Fault;
            }
        }

        private void FireConnectionEvent(DeviceConnectionState State, string Message, LogMessageType type = LogMessageType.Info)
        {
            SystemHost.AppendLog(m_ID, type, Message);

            m_Communication.PostDeviceConnectionEvent(m_ID, State, Message);
        }

        private void FireNotificationEvent(Types.Commutation.HWWarningReason Warning, Types.Commutation.HWFaultReason Fault)
        {
            SystemHost.AppendLog(m_ID, LogMessageType.Error,
                                         string.Format("Commutation device notification: warning {0}, fault {1}",
                                                       Warning, Fault));

            m_Communication.PostCommutationNotificationEvent(Warning, Fault);
        }

        private void FireSwitchEvent(Types.Commutation.CommutationMode SwitchState, Types.Commutation.HWModuleCommutationType CommutationType, Types.Commutation.HWModulePosition Position)
        {
            SystemHost.AppendLog(m_ID, LogMessageType.Info,
                                         string.Format("Switch state {0} on {1}:{2}", SwitchState, CommutationType, Position));

            m_Communication.PostCommutationSwitchEvent(SwitchState);
        }

        private void FireExceptionEvent(string Message)
        {
            SystemHost.AppendLog(m_ID, LogMessageType.Error, Message);

            m_Communication.PostExceptionEvent(m_ID, Message);
        }

        private void FireSafetyEvent(bool Alarm)
        {
            //уведомлять UI о наступлении события срабатывания оптической шторки имеет смысл если тип установленной системы безопасности - оптическая шторка
            if (m_SafetyType == ComplexSafety.Optical)
            {
                SystemHost.AppendLog(ComplexParts.Commutation, LogMessageType.Info, string.Format("Commutation optical safety system alarm={0}", Alarm.ToString()));
                m_Communication.PostSafetyEvent(Alarm, ComplexSafety.Optical, ComplexButtons.None);
            }
        }

        #region Registers

        /// <summary>
        /// Enable safety input #1 (Internal)
        /// </summary>
        internal const ushort REG_EN_SFTY_IN1 = 80;

        /// <summary>
        /// Enable safety input #2 (STOP)
        /// </summary>
        internal const ushort REG_EN_SFTY_IN2 = 81;

        /// <summary>
        /// Enable safety input #3 (External)
        /// </summary>
        internal const ushort REG_EN_SFTY_IN3 = 82;

        /// <summary>
        /// Enable safety input #4 (Reserved)
        /// </summary>
        internal const ushort REG_EN_SFTY_IN4 = 83;

        internal const ushort

            ACT_ENABLE_POWER = 1,
            ACT_DISABLE_POWER = 2,
            ACT_CLEAR_FAULT = 3,
            ACT_CLEAR_WARNING = 4,
            ACT_SAFETY_ON = 100,
            ACT_SAFETY_OFF = 101,
            ACT_COMM2_NONE = 110,
            ACT_COMM2_GATE = 111,
            ACT_COMM2_SL = 112,
            ACT_COMM2_BVT_D = 113,
            ACT_COMM2_BVT_R = 114,
            ACT_COMM2_DVDT = 115,
            ACT_COMM2_ATU = 115,
            ACT_COMM2_TOU = 115,
            ACT_COMM2_GATE_SL = 116,
            ACT_COMM2_VGNT = 117,
            ACT_COMM_IH = 116,
            ACT_COMM6_NONE = 120,
            ACT_COMM6_GATE = 121,
            ACT_COMM6_SL = 122,
            ACT_COMM6_BVT_D = 123,
            ACT_COMM6_BVT_R = 124,
            ACT_COMM2_RCC = 127,

            REG_DEVICE_STATE = 96,
            REG_FAULT_REASON = 97,
            REG_DISABLE_REASON = 98,
            REG_WARNING = 99,
            REG_MODULE_TYPE = 70,
            REG_MODULE_POSITION = 71,
            SAFETY_REASON_SC1 = 301,
            SAFETY_REASON_SC2 = 302,

            ACT_COMM2_GATE_BVT = 117;

        #endregion
    }
}