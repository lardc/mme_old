﻿using SCME.Types;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SCME.UI
{
    public class BroadcastCommunication
    {
        private readonly List<IClientCallback> m_Subscribers;
        private IClientCallback m_SubToDelete;

        public BroadcastCommunication()
        {
            m_Subscribers = new List<IClientCallback>();
        }

        public List<IClientCallback> Subscribers
        {
            get { return m_Subscribers; }
        }

        public void PostDbSyncState(DeviceConnectionState state, string message)
        {
            EnumerateClients(Client => Client.DbSyncState(state, message));
        }

        public void PostCommonConnectionEvent(DeviceConnectionState State, string Message)
        {
            EnumerateClients(Client => Client.CommonConnectionHandler(State, Message));
        }

        public void PostDeviceConnectionEvent(ComplexParts Device, DeviceConnectionState State, string Message)
        {
            EnumerateClients(Client => Client.DeviceConnectionHandler(Device, State, Message));
        }

        public void PostTestAllEvent(DeviceState State, string Message)
        {
            EnumerateClients(Client => Client.TestAllHandler(State, Message));
        }

        public void PostExceptionEvent(ComplexParts Device, string Message)
        {
            EnumerateClients(Client => Client.ExceptionHandler(Device, Message));
        }

        public void PostButtonPressedEvent(ComplexButtons Button, bool State)
        {
            EnumerateClients(Client => Client.GatewayButtonPressHandler(Button, State));
        }

        public void PostSafetyEvent(bool Alarm, ComplexSafety SafetyType, ComplexButtons Button)
        {
            EnumerateClients(Client => Client.SafetyHandler(Alarm, SafetyType, Button));
        }

        public void PostStopEvent()
        {
            EnumerateClients(Client => Client.StopHandler());
        }

        public void PostSyncDBAreProcessedEvent()
        {
            throw new NotImplementedException();
            //EnumerateClients(Client => Client.SyncDBAreProcessedHandler());
        }

        public void PostGatewayNotificationEvent(Types.Gateway.HWWarningReason Warning, Types.Gateway.HWFaultReason Fault, Types.Gateway.HWDisableReason Disable)
        {
            EnumerateClients(Client => Client.GatewayNotificationHandler(Warning, Fault, Disable));
        }

        public void PostCommutationSwitchEvent(Types.Commutation.CommutationMode ComSwitch)
        {
            EnumerateClients(Client => Client.CommutationSwitchHandler(ComSwitch));
        }

        public void PostCommutationNotificationEvent(Types.Commutation.HWWarningReason Warning,
                                                     Types.Commutation.HWFaultReason Fault)
        {
            EnumerateClients(Client => Client.CommutationNotificationHandler(Warning, Fault));
        }

        public void PostGateAllEvent(DeviceState state)
        {
            EnumerateClients(client => client.GateAllHandler(state));
        }

        public void PostGateKelvinEvent(DeviceState state, Types.GTU.TestResults result)
        {
            EnumerateClients(client => client.GateKelvinHandler(state, result.IsKelvinOk, result.ArrayKelvin, result.TestTypeId));
        }

        public void PostGateResistanceEvent(DeviceState state, Types.GTU.TestResults result)
        {
            EnumerateClients(client => client.GateResistanceHandler(state, result.Resistance, result.TestTypeId));
        }

        public void PostGateGateEvent(DeviceState state, Types.GTU.TestResults result)
        {
            EnumerateClients(client => client.GateIgtVgtHandler(state, result.IGT, result.VGT, result.ArrayIGT, result.ArrayVGT, result.TestTypeId));
        }

        public void PostGateIhEvent(DeviceState state, Types.GTU.TestResults result)
        {
            EnumerateClients(client => client.GateIhHandler(state, result.IH, result.ArrayIH, result.TestTypeId));
        }

        public void PostGateIlEvent(DeviceState state, Types.GTU.TestResults result)
        {
            EnumerateClients(client => client.GateIlHandler(state, result.IL, result.TestTypeId));
        }

        public void PostGateVgntEvent(DeviceState state, Types.GTU.TestResults result)
        {
            EnumerateClients(client => client.GateVgntHandler(state, result.IGNT, result.VGNT, result.TestTypeId));
        }

        public void PostGateNotificationEvent(Types.GTU.HWProblemReason Problem, Types.GTU.HWWarningReason Warning,
                                              Types.GTU.HWFaultReason Fault, Types.GTU.HWDisableReason Disable)
        {
            EnumerateClients(Client => Client.GateNotificationHandler(Problem, Warning, Fault, Disable));
        }

        public void PostSLEvent(DeviceState State, Types.SL.TestResults Result)
        {
            EnumerateClients(Client => Client.SLHandler(State, Result));
        }

        public void PostSLNotificationEvent(Types.SL.HWProblemReason Problem, Types.SL.HWWarningReason Warning,
                                             Types.SL.HWFaultReason Fault, Types.SL.HWDisableReason Disable)
        {
            EnumerateClients(Client => Client.SLNotificationHandler(Problem, Warning, Fault, Disable));
        }

        public void PostBVTAllEvent(DeviceState State)
        {
            EnumerateClients(Client => Client.BVTAllHandler(State));
        }

        public void PostBVTDirectEvent(DeviceState State, Types.BVT.TestResults Result)
        {
            EnumerateClients(Client => Client.BVTDirectHandler(State, Result));
        }

        public void PostBVTReverseEvent(DeviceState State, Types.BVT.TestResults Result)
        {
            EnumerateClients(Client => Client.BVTReverseHandler(State, Result));
        }

        public void PostBVTNotificationEvent(Types.BVT.HWProblemReason Problem, Types.BVT.HWWarningReason Warning,
                                             Types.BVT.HWFaultReason Fault, Types.BVT.HWDisableReason Disable)
        {
            EnumerateClients(Client => Client.BVTNotificationHandler(Problem, Warning, Fault, Disable));
        }

        public void PostBVTUdsmUrsmDirectEvent(DeviceState State, Types.BVT.TestResults Result)
        {
            EnumerateClients(Client => Client.BVTUdsmUrsmDirectHandler(State, Result));
        }

        public void PostBVTUdsmUrsmReverseEvent(DeviceState State, Types.BVT.TestResults Result)
        {
            EnumerateClients(Client => Client.BVTUdsmUrsmReverseHandler(State, Result));
        }

        public void PostClampingSwitchEvent(Types.Clamping.SqueezingState SQState, IList<float> ArrayF, IList<float> ArrayFd)
        {
            EnumerateClients(Client => Client.ClampingSwitchHandler(SQState, ArrayF, ArrayFd));
        }

        public void PostClampingNotificationEvent(Types.Clamping.HWWarningReason Warning,
                                                  Types.Clamping.HWProblemReason Problem,
                                                  Types.Clamping.HWFaultReason Fault)
        {
            EnumerateClients(Client => Client.ClampingNotificationHandler(Warning, Problem, Fault));
        }

        public void PostClampingTemperatureEvent(Types.Clamping.HeatingChannel channel, int temeprature)
        {
            EnumerateClients(client => client.ClampingTemperatureHandler(channel, temeprature));
        }

        public void PostdVdtEvent(DeviceState State, Types.dVdt.TestResults Result)
        {
            EnumerateClients(Client => Client.DvDtHandler(State, Result));
        }

        public void PostATUEvent(DeviceState State, Types.ATU.TestResults Result)
        {
            EnumerateClients(Client => Client.ATUHandler(State, Result));
        }

        public void PostQrrTqEvent(DeviceState State, Types.QrrTq.TestResults Result)
        {
            EnumerateClients(Client => Client.QrrTqHandler(State, Result));
        }

        public void PostTOUEvent(DeviceState State, Types.TOU.TestResults Result)
        {
            EnumerateClients(Client => Client.TOUHandler(State, Result));
        }

        public void PostdVdtNotificationEvent(Types.dVdt.HWWarningReason Warning,
                                     Types.dVdt.HWFaultReason Fault, Types.dVdt.HWDisableReason Disable)
        {
            EnumerateClients(Client => Client.DvDtNotificationHandler(Warning, Fault, Disable));
        }

        public void PostATUNotificationEvent(ushort Warning, ushort Fault, ushort Disable)
        {
            EnumerateClients(Client => Client.ATUNotificationHandler(Warning, Fault, Disable));
        }

        public void PostQrrTqNotificationEvent(ushort Problem, ushort Warning, ushort Fault, ushort Disable)
        {
            EnumerateClients(Client => Client.QrrTqNotificationHandler(Problem, Warning, Fault, Disable));
        }

        public void PostQrrTqKindOfFreezingEvent(ushort KindOfFreezing)
        {
            EnumerateClients(Client => Client.QrrTqKindOfFreezingHandler(KindOfFreezing));
        }

        public void PostTOUNotificationEvent(ushort Problem, ushort Warning, ushort Fault, ushort Disable)
        {
            EnumerateClients(Client => Client.TOUNotificationHandler(Problem, Warning, Fault, Disable));
        }

        private void EnumerateClients(Action<IClientCallback> Act)
        {
            foreach (var cbk in m_Subscribers)
            {
                var temp = cbk;
                TryCatchWrapper(() => Act(temp), temp);
            }

            HandleSubToDelete();
        }

        private void TryCatchWrapper(Action Act, IClientCallback Obj)
        {
            try
            {
                Act();
            }
            catch (ObjectDisposedException)
            {
                m_SubToDelete = Obj;
            }
            catch (CommunicationObjectAbortedException)
            {
                m_SubToDelete = Obj;
            }
            catch (CommunicationObjectFaultedException)
            {
                m_SubToDelete = Obj;
            }
            catch (Exception ex)
            {
                SystemHost.AppendLog(ComplexParts.None, LogMessageType.Error,
                                             string.Format("Exception {0} during callback action: {1}", ex.GetType(), ex.Message));
                m_SubToDelete = Obj;
            }
        }

        private void HandleSubToDelete()
        {
            if (m_SubToDelete != null)
            {
                m_Subscribers.Remove(m_SubToDelete);
                m_SubToDelete = null;
            }
        }
    }
}