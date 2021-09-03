using PE.SCCI;
using PE.SCCI.Master;
using SCME.Types;
using SCME.UIServiceConfig.Properties;
using System;
using System.Collections.Generic;
using System.IO.Ports;

namespace SCME.UI.IO
{
    internal class IOAdapter
    {
        private readonly bool m_IsAdapterEmulation;
        private readonly BroadcastCommunication m_Communication;

        private SCCIMasterAdapter m_Adapter;
        private DeviceConnectionState m_ConnectionState;

        internal IOAdapter(BroadcastCommunication Communication)
        {
            m_Communication = Communication;

            m_IsAdapterEmulation = Settings.Default.AdapterEmulation;

            SystemHost.AppendLog(ComplexParts.Adapter, LogMessageType.Milestone,
                                         String.Format("Adapter created. Emulation mode: {0}", m_IsAdapterEmulation));
        }

        internal DeviceConnectionState Initialize(int Timeout)
        {
            m_ConnectionState = DeviceConnectionState.ConnectionInProcess;

            FireConnectionEvent(m_ConnectionState, "Adapter initializing");

            if (m_IsAdapterEmulation)
            {
                m_ConnectionState = DeviceConnectionState.ConnectionSuccess;

                FireConnectionEvent(m_ConnectionState, "Adapter initialized", LogMessageType.Milestone);

                return m_ConnectionState;
            }

            try
            {
                if (m_Adapter != null)
                {
                    if (m_Adapter.Connected)
                        m_Adapter.Close();
                }

                m_Adapter = new SCCIMasterAdapter(true);
                m_Adapter.Initialize(new SerialPortConfigurationMaster
                    {
                        PortNumber = Settings.Default.AdapterPort,
                        BaudRate = 115200,
                        DataBits = 8,
                        StopBits = StopBits.One,
                        ParityMode = Parity.None,
                        TimeoutForSyncReceive = Timeout,
                        TimeoutForSyncStreamReceive = Timeout,
                        UseRetransmitsForArrays = true,
                        RetransmitsCountOnError = 5
                    });

                m_ConnectionState = DeviceConnectionState.ConnectionSuccess;

                FireConnectionEvent(m_ConnectionState, "Adapter initialized", LogMessageType.Milestone);
            }
            catch (SerialConnectionException ex)
            {
                m_ConnectionState = DeviceConnectionState.ConnectionFailed;

                FireConnectionEvent(m_ConnectionState, String.Format("Adapter initialization error: {0}", ex.Message), LogMessageType.Error);
            }

            return m_ConnectionState;
        }

        internal void Deinitialize()
        {
            m_ConnectionState = DeviceConnectionState.DisconnectionInProcess;
            FireConnectionEvent(DeviceConnectionState.DisconnectionInProcess, "Adapter closing");

            if (m_Adapter != null && m_Adapter.Connected)
            {
                m_Adapter.Close();
                m_Adapter = null;
            }

            m_ConnectionState = DeviceConnectionState.DisconnectionSuccess;
            FireConnectionEvent(DeviceConnectionState.DisconnectionSuccess, "Adapter closed", LogMessageType.Milestone);
        }

        internal ushort Read16(ushort Node, ushort Address)
        {
            ushort res = 0;

            if (m_IsAdapterEmulation)
                return res;

            try
            {
                res = m_Adapter.Read16(Node, Address);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Operation - {3}, Node - {0}, Address - {1}, Message - {2}", Node, Address,
                                                  ex.Message, @"@Read16"));
            }

            return res;
        }

        internal short Read16S(ushort Node, ushort Address)
        {
            short res = 0;

            if (m_IsAdapterEmulation)
                return res;

            try
            {
                res = m_Adapter.Read16S(Node, Address);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Operation - {3}, Node - {0}, Address - {1}, Message - {2}", Node, Address,
                                                  ex.Message, @"@Read16S"));
            }

            return res;
        }

        internal IList<ushort> ReadArrayFast16(ushort Node, ushort Address)
        {
            IList<ushort> res = new List<ushort>();

            if (m_IsAdapterEmulation)
                return res;

            try
            {
                res = m_Adapter.ReadArrayFast16(Node, Address);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Operation - {3}, Node - {0}, Address - {1}, Message - {2}", Node, Address,
                                                  ex.Message, @"@ReadArrayFast16"));
            }

            return res;
        }

        internal IList<short> ReadArrayFast16S(ushort Node, ushort Address)
        {
            IList<short> res = new List<short>();

            if (m_IsAdapterEmulation)
                return res;

            try
            {
                res = m_Adapter.ReadArrayFast16S(Node, Address);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Operation - {3}, Node - {0}, Address - {1}, Message - {2}", Node, Address,
                                                  ex.Message, @"@ReadArrayFast16S"));
            }

            return res;
        }

        internal void Write16(ushort Node, ushort Address, ushort Value)
        {
            if (m_IsAdapterEmulation)
                return;

            try
            {
                m_Adapter.Write16(Node, Address, Value);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Operation - {4}, Node - {0}, Address - {1}, Value - {2}, Message - {3}", Node,
                                                  Address, Value, ex.Message, @"@Write16"));
            }
        }

        internal void Write16S(ushort Node, ushort Address, short Value)
        {
            if (m_IsAdapterEmulation)
                return;

            try
            {
                m_Adapter.Write16S(Node, Address, Value);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Operation - {4}, Node - {0}, Address - {1}, Value - {2}, Message - {3}", Node,
                                                  Address, Value, ex.Message, @"@Write16S"));
            }
        }

        internal void Call(ushort Node, ushort Address)
        {
            if (m_IsAdapterEmulation)
                return;

            try
            {
                m_Adapter.Call(Node, Address);
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Operation - {4}, Node - {0}, Address - {1}, Value - {2}, Message - {3}", Node, Address,
                                                  ex.Message, @"@Call"));
            }
        }

        private void FireConnectionEvent(DeviceConnectionState State, string Message, LogMessageType type = LogMessageType.Info)
        {
            SystemHost.AppendLog(ComplexParts.Adapter, type, Message);

            m_Communication.PostDeviceConnectionEvent(ComplexParts.Adapter, State, Message);
        }
    }
}