using SCME.Types;
using SCME.Types.Commutation;
using SCME.Types.Profiles;
using SCME.Types.SQL;
using SCME.UIServiceConfig.Properties;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;

namespace SCME.UI
{
    public class ExternalControlServer : IExternalControl
    {
        private readonly BroadcastCommunication m_Communication;
        private readonly LogicContainer _IoMain;

        private const string PRINTING_ENDPOINT_NAME = "SCME.PrintingService";

        internal ExternalControlServer()
        {
            m_Communication = new BroadcastCommunication();
            //чтобы SystemHost мог говорить UI о том, что процесс синхронизации баз данных как-то завершился - сообщаем ему значение m_Communication
            SystemHost.SetCommunication(m_Communication);

            _IoMain = new LogicContainer(m_Communication);
        }

        #region Interface implementation

        void IExternalControl.Check()
        {
        }

        void IExternalControl.Subscribe()
        {
            var item = OperationContext.Current.GetCallbackChannel<IClientCallback>();

            if (!m_Communication.Subscribers.Contains(item))
                m_Communication.Subscribers.Add(item);
        }

        void IExternalControl.Unsubscribe()
        {
            var item = OperationContext.Current.GetCallbackChannel<IClientCallback>();

            if (m_Communication.Subscribers.Contains(item))
                m_Communication.Subscribers.Add(item);
        }

        void IExternalControl.Initialize(TypeCommon.InitParams Param)
        {
            ThreadPool.QueueUserWorkItem(Func => _IoMain.Initialize(Param));
        }

        void IExternalControl.Deinitialize()
        {
            _IoMain.Deinitialize();
        }

        InitializationResponse IExternalControl.IsInitialized()
        {
            return _IoMain.InitializationResponse;
//            /*return (m_IOMain.IsInitialized ? InitializationResult.ModulesInitialized : InitializationResult.None)
//                   | (SystemHost.IsSyncedWithServer ? InitializationResult.SyncedWithServer : InitializationResult.None);
//            */
//
//
//            InitializationResponce initializationResponse = new InitializationResponce()
//            {
//                IsLocal = Properties.Settings.Default.IsLocal,
//                MMECode = Properties.Settings.Default.MMECode
//            };
//
//            //в switch нельзя проверять это условие в default - компилятор успешно откомпилирует, но работать будет с ошибкой
//            if (SystemHost.IsSyncedWithServer == null)
//            {
//                //синхронизация данных в данный момент выполняется
//                initializationResponse.InitializationResult = InitializationResult.SyncInProgress;
//            }
//            else
//            {
//                switch (SystemHost.IsSyncedWithServer)
//                {
//                    case (true):
//                        //синхронизация данных завершена, данные синхронизированы
//                        initializationResponse.InitializationResult = InitializationResult.SyncedWithServer;
//                        break;
//
//                    default:
//                        //синхронизация данных завершена, данные не синхронизированы
//                        initializationResponse.InitializationResult = InitializationResult.None;
//                        break;
//                }
//            }
//
//            //Ужасное выражение, которое я оставляю потому что работает.
//            initializationResponse.InitializationResult = (m_IOMain.IsInitialized ? InitializationResult.ModulesInitialized : InitializationResult.None) | initializationResponse.InitializationResult;
//            
//            return initializationResponse;
        }

        Types.GTU.CalibrationResultGate IExternalControl.GatePulseCalibrationGate(ushort Current)
        {
            return _IoMain.GatePulseCalibrationGate(Current);
        }

        ushort IExternalControl.GatePulseCalibrationMain(ushort Current)
        {
            return _IoMain.GatePulseCalibrationMain(Current);
        }

        void IExternalControl.GateWriteCalibrationParameters(Types.GTU.CalibrationParameters Parameters)
        {
            _IoMain.GateWriteCalibrationParams(Parameters);
        }

        Types.GTU.CalibrationParameters IExternalControl.GateReadCalibrationParameters()
        {
            return _IoMain.GateReadCalibrationParams();
        }

        bool IExternalControl.GetButtonState(ComplexButtons Button)
        {
            return _IoMain.GetButtonState(Button);
        }

        void IExternalControl.ProvocationButtonResponse(ComplexButtons Button)
        {
            _IoMain.ProvocationButtonResponse(Button);
        }

        ComplexSafety IExternalControl.GetSafetyType()
        {
            return _IoMain.GetSafetyType();
        }

        bool IExternalControl.Start(Types.GTU.TestParameters ParametersGate, Types.SL.TestParameters ParametersSL,
                                    Types.BVT.TestParameters ParametersBvt, Types.ATU.TestParameters ParametersAtu, Types.QrrTq.TestParameters ParametersQrrTq,
                                    Types.Commutation.TestParameters ParametersComm, Types.Clamping.TestParameters ParametersClamp, Types.TOU.TestParameters ParametersTOU)
        {
            return _IoMain.Start(ParametersGate, ParametersSL, ParametersBvt, ParametersAtu, ParametersQrrTq, ParametersComm, ParametersClamp, ParametersTOU);
        }

        bool IExternalControl.StartDynamic(TestParameters parametersCommutation, Types.Clamping.TestParameters parametersClamp, Types.GTU.TestParameters[] parametersGate, Types.SL.TestParameters[] parametersSl, Types.BVT.TestParameters[] parametersBvt, Types.dVdt.TestParameters[] parametersDvDt, Types.ATU.TestParameters[] parametersAtu, Types.QrrTq.TestParameters[] parametersQrrTq, Types.TOU.TestParameters[] parametersTOU)
        {
            return _IoMain.Start(parametersCommutation, parametersClamp, parametersGate, parametersSl, parametersBvt, parametersDvDt, parametersAtu, parametersQrrTq, parametersTOU);
        }

        void IExternalControl.ClearSafetyTrig()
        {
            _IoMain.ClearSafetyTrig();
        }

        void IExternalControl.SafetySystemOn()
        {
            _IoMain.SafetySystemOn();
        }

        void IExternalControl.SafetySystemOff()
        {
            _IoMain.SafetySystemOff();
        }

        string IExternalControl.NotReadyDevicesToStart()
        {
            return _IoMain.NotReadyDevicesToStart();
        }

        string IExternalControl.NotReadyDevicesToStartDynamic()
        {
            return _IoMain.NotReadyDevicesToStartDynamic();
        }

        public bool StartHeating(int temperature)
        {
            return _IoMain.StartHeating(temperature);
        }

        public void StopHeating()
        {
            _IoMain.StopHeating();
        }

        public void SetPermissionToUseCanDataBus(bool PermissionToUseCanDataBus)
        {
            _IoMain.SetPermissionToUseCanDataBus(PermissionToUseCanDataBus);
        }

        void IExternalControl.Stop()
        {
            ThreadPool.QueueUserWorkItem(Func => _IoMain.Stop());
        }

        void IExternalControl.StopByButtonStop()
        {
            ThreadPool.QueueUserWorkItem(Func => _IoMain.StopByButtonStop());
        }

        void IExternalControl.SqueezeClamping(Types.Clamping.TestParameters ParametersClamping)
        {
            _IoMain.Squeeze(ParametersClamping);
        }

        void IExternalControl.UnsqueezeClamping(Types.Clamping.TestParameters ParametersClamping)
        {
            _IoMain.Unsqueeze(ParametersClamping);
        }

        void IExternalControl.ClearFault(ComplexParts Device)
        {
            _IoMain.ClearFault(Device);
        }

        ushort IExternalControl.ReadRegister(ComplexParts Device, ushort Address)
        {
            return _IoMain.ReadRegister(Device, Address);
        }

        void IExternalControl.WriteRegister(ComplexParts Device, ushort Address, ushort Value)
        {
            _IoMain.WriteRegister(Device, Address, Value);
        }

        void IExternalControl.CallAction(ComplexParts Device, ushort Address)
        {
            _IoMain.CallAction(Device, Address);
        }



        void IExternalControl.WriteResults(ResultItem Item, List<string> Errors)
        {
            _IoMain.WriteResults(Item, Errors);
        }

        List<ProfileForSqlSelect> IExternalControl.SaveProfiles(List<ProfileItem> profileItems)
        {
            return _IoMain.SaveProfiles(profileItems);
        }

        bool IExternalControl.RequestRemotePrinting(string GroupName, string CustomerName, string DeviceType,
             ReportSelectionPredicate Predicate)
        {
            return true;

            //try
            //{
            //    var printingProxy = new PrintingServiceProxy(PRINTING_ENDPOINT_NAME);
            //    printingProxy.Open();

            //    SystemHost.AppendLog(ComplexParts.Service, LogMessageType.Info, "Printing service connection is opened");

            //    try
            //    {
            //        printingProxy.RequestRemotePrinting(Settings.Default.MMECode, GroupName, CustomerName,
            //            DeviceType, Predicate);

            //        SystemHost.AppendLog(ComplexParts.Service, LogMessageType.Info,
            //            String.Format("Printing request: GroupName - {0}, Selection - {1}", GroupName, Predicate));
            //    }
            //    finally
            //    {
            //        if (printingProxy.State == CommunicationState.Faulted)
            //            printingProxy.Abort();
            //        else
            //            printingProxy.Close();
            //    }

            //    return true;
            //}
            //catch (FaultException<FaultData> ex)
            //{
            //    SystemHost.AppendLog(ComplexParts.Service, LogMessageType.Error,
            //        String.Format("Printing request failed: {0}", ex.Message));

            //    return false;
            //}
            //catch (Exception ex)
            //{
            //    SystemHost.AppendLog(ComplexParts.Service, LogMessageType.Error,
            //        String.Format("Printing request failed: {0}", ex.Message));

            //    return false;
            //}
        }

        public void SetSafetyMode(SafetyMode safetyMode)
        {
            _IoMain.SetSafetyMode(safetyMode);
            SystemHost.AppendLog(ComplexParts.Service, LogMessageType.Info, $"Safety mode is: {safetyMode}");
        }

        public (MyProfile profile, bool IsInMmeCode) SyncProfile(MyProfile profile)
        {
            return _IoMain.IoDbSync.SyncProfile(profile);
        }
        
        

        public void SetUserWorkMode(UserWorkMode userWorkMode)
        {
            _IoMain.SetUserWorkMode(userWorkMode);
        }

        public void WriteJournal(ComplexParts device, LogMessageType type, DateTime dateTime, string message)
        {
            SystemHost.AppendLog(device, type, $"{dateTime}: {message}");
        }



        #endregion
    }
}