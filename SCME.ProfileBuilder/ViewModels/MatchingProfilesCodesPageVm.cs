using PropertyChanged;
using SCME.Types;
using SCME.Types.BaseTestParams;
using SCME.Types.Database;
using SCME.Types.Profiles;
using SCME.WpfControlLibrary.Commands;
using SCME.WpfControlLibrary.CustomControls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Xml.Linq;

namespace SCME.ProfileBuilder.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class MatchingProfilesCodesPageVm 
    {
        private readonly IDbService _dbService;

        public MatchingProfilesCodesPageVm(IDbService dbService)
        {
            _dbService = dbService;
            MmeCodes = new ObservableCollection<string>(_dbService.GetMmeCodes().Select(m => m.Key).Except(new string[] {Constants.MME_CODE_IS_ACTIVE_NAME}));

            ActiveProfiles = new CollectionViewSource()
            {
                Source = new ObservableCollection<MyProfile>(_dbService.GetProfilesSuperficially(Constants.MME_CODE_IS_ACTIVE_NAME)),
                SortDescriptions = {new SortDescription(nameof(MyProfile.Name), ListSortDirection.Ascending)}
            };
            
           InactiveProfiles = new CollectionViewSource()
            {
                Source = new ObservableCollection<MyProfile>(_dbService.GetProfilesSuperficially(string.Empty).Except(ActiveProfilesSource)),
                SortDescriptions = {new SortDescription(nameof(MyProfile.Name), ListSortDirection.Ascending)}
            };
           
           ProfilesFromMmeCode = new CollectionViewSource()
           {
               SortDescriptions = {new SortDescription(nameof(MyProfile.Name), ListSortDirection.Ascending)}
           };

           ProfilesForMmeCode = new CollectionViewSource()
           {
               SortDescriptions = {new SortDescription(nameof(MyProfile.Name), ListSortDirection.Ascending)}
           };
                
            ActiveProfiles.Filter += (sender, args) =>
            {
                var profile = (MyProfile) args.Item;
                args.Accepted = string.IsNullOrEmpty(ActiveProfileFilterName) || profile.Name.ToUpper().Contains(ActiveProfileFilterName.ToUpper());
            };

            InactiveProfiles.Filter += (sender, args) =>
            {
                var profile = (MyProfile) args.Item;
                args.Accepted = string.IsNullOrEmpty(InactiveProfileFilterName) || profile.Name.ToUpper().Contains(InactiveProfileFilterName.ToUpper());
            };
            
            ProfilesForMmeCode.Filter += (sender, args) =>
            {
                var profile = (MyProfile) args.Item;
                args.Accepted = string.IsNullOrEmpty(ProfileForMmeCodeFilterName) || profile.Name.ToUpper().Contains(ProfileForMmeCodeFilterName.ToUpper());
            };
            
            ProfilesFromMmeCode.Filter += (sender, args) =>
            {
                var profile = (MyProfile) args.Item;
                if (string.IsNullOrEmpty(_selectedMmeCode))
                    args.Accepted = false;
                else
                    args.Accepted = string.IsNullOrEmpty(ProfileFromMmeCodeFilterName) || profile.Name.ToUpper().Contains(ProfileFromMmeCodeFilterName.ToUpper());
                    
            };
                
            ProfilesForMmeCode.Filter += (sender, args) =>
            {
                var profile = (MyProfile) args.Item;
                args.Accepted = string.IsNullOrEmpty(ProfileForMmeCodeFilterName) || profile.Name.ToUpper().Contains(ProfileForMmeCodeFilterName.ToUpper());
                    
            };
        }

        #region ProfilesActiveInactive

        private ObservableCollection<MyProfile> ActiveProfilesSource => (ObservableCollection<MyProfile>) ActiveProfiles.Source;
        private  ObservableCollection<MyProfile> InactiveProfilesSource => (ObservableCollection<MyProfile>) InactiveProfiles.Source;

        public CollectionViewSource ActiveProfiles { get; set; }
        public CollectionViewSource InactiveProfiles { get; set; }

        public List<MyProfile> SelectedActiveProfiles { get; } = new List<MyProfile>();
        public List<MyProfile> SelectedInactiveProfiles { get; } = new List<MyProfile>();

        public string ActiveProfileFilterName { get; set; }
        public string InactiveProfileFilterName { get; set; }

        public RelayCommand MoveToInactive => new RelayCommand(o =>
        {
            foreach (var i in SelectedActiveProfiles.ToList())
            {
                foreach (var j in _dbService.GetMmeCodesByProfile(i.Id))
                    _dbService.RemoveMmeCodeToProfile(i.Id, j);
                
                ActiveProfilesSource.Remove(i);
                InactiveProfilesSource.Add(i);
                
                ProfilesFromMmeCodeSource?.Remove(i);
                ProfilesForMmeCodeSource?.Remove(i);
            }
        }, o => SelectedActiveProfiles.Count > 0);

        public RelayCommand MoveToActive => new RelayCommand(o =>
        {
            foreach (var i in SelectedInactiveProfiles.ToList())
            {
                _dbService.InsertMmeCodeToProfile(i.Id, Constants.MME_CODE_IS_ACTIVE_NAME);
                
                InactiveProfilesSource.Remove(i);
                ActiveProfilesSource.Add(i);
                
                if(ProfilesFromMmeCodeSource?.Contains(i) == false)
                    ProfilesForMmeCodeSource?.Add(i);
            }
        }, o => SelectedInactiveProfiles.Count > 0);

        #endregion

        #region MatchingProfiles

        private string _selectedMmeCode;

        public string SelectedMmeCode
        {
            get => _selectedMmeCode;
            set
            {
                _selectedMmeCode = value;

                ProfilesFromMmeCodeSource = new ObservableCollection<MyProfile>(_dbService.GetProfilesSuperficially(_selectedMmeCode));
                ProfilesForMmeCodeSource = new ObservableCollection<MyProfile>(ActiveProfilesSource.Except(ProfilesFromMmeCodeSource));
            }
        }

        public string ProfileFromMmeCodeFilterName { get; set; }
        public string ProfileForMmeCodeFilterName { get; set; }

        private ObservableCollection<MyProfile> ProfilesFromMmeCodeSource
        {
            get => (ObservableCollection<MyProfile>) ProfilesFromMmeCode.Source;
            set => ProfilesFromMmeCode.Source = value;
        }

        private ObservableCollection<MyProfile> ProfilesForMmeCodeSource
        {
            get => (ObservableCollection<MyProfile>) ProfilesForMmeCode.Source;
            set => ProfilesForMmeCode.Source = value;
        }

        public CollectionViewSource ProfilesFromMmeCode { get; set; }
        public CollectionViewSource ProfilesForMmeCode { get; set; }

        public List<MyProfile> SelectedProfilesFromMmeCode { get; } = new List<MyProfile>();
        public List<MyProfile> SelectedProfilesForMmeCode { get; } = new List<MyProfile>();
        
        public RelayCommand AddProfileToMmeCode => new RelayCommand(o =>
        {
            //Парсинг параметров нужного комплекса
            XDocument ParamsConfig = XDocument.Load("SCME.ParamsConfig.xml");
            XElement CurrentMme = ParamsConfig.Element("mmes").Elements("mme").FirstOrDefault(mme => mme.Attribute("name").Value == SelectedMmeCode);
            //Перебор всех добавляемыхк комплексу профилей
            foreach (MyProfile Profile in SelectedProfilesForMmeCode.ToList())
            {
                //Флаг корректности профиля
                bool IsProfileCorrect = true;
                //Перебор всех типов тестирования в профиле
                foreach (BaseTestParametersAndNormatives Parameter in Profile.DeepData.TestParametersAndNormatives)
                {
                    if (Parameter.TestParametersType == TestParametersType.GTU)
                        if (!GTUParameters_Compare(CurrentMme, Parameter))
                        {
                            MessageBox.Show("Параметры тестирования GTU не соответствуют данному КИП", "Ошибка добавления");
                            IsProfileCorrect = false;
                            break;
                        }
                    if (Parameter.TestParametersType == TestParametersType.SL)
                        if (!SLParameters_Compare(CurrentMme, Parameter))
                        {
                            MessageBox.Show("Параметры тестирования SL не соответствуют данному КИП", "Ошибка добавления");
                            IsProfileCorrect = false; 
                            break;
                        }
                    if (Parameter.TestParametersType == TestParametersType.BVT)
                        if (!BVTParameters_Compare(CurrentMme, Parameter))
                        {
                            MessageBox.Show("Параметры тестирования BVT не соответствуют данному КИП", "Ошибка добавления");
                            IsProfileCorrect = false; 
                            break;
                        }
                    if (Parameter.TestParametersType == TestParametersType.dVdt)
                        if (!dUdtParameters_Compare(CurrentMme, Parameter))
                        {
                            MessageBox.Show("Параметры тестирования dUdt не соответствуют данному КИП", "Ошибка добавления");
                            IsProfileCorrect = false; 
                            break;
                        }
                    if (Parameter.TestParametersType == TestParametersType.ATU)
                        if (!ATUParameters_Compare(CurrentMme, Parameter))
                        {
                            MessageBox.Show("Параметры тестирования ATU не соответствуют данному КИП", "Ошибка добавления");
                            IsProfileCorrect = false; 
                            break;
                        }
                    if (Parameter.TestParametersType == TestParametersType.QrrTq)
                        if (!QrrTqParameters_Compare(CurrentMme, Parameter))
                        {
                            MessageBox.Show("Параметры тестирования QrrTq не соответствуют данному КИП", "Ошибка добавления");
                            IsProfileCorrect = false; 
                            break;
                        }
                    if (Parameter.TestParametersType == TestParametersType.TOU)
                        if (!TOUParameters_Compare(CurrentMme, Parameter))
                        {
                            MessageBox.Show("Параметры тестирования TOU не соответствуют данному КИП", "Ошибка добавления");
                            IsProfileCorrect = false; 
                            break;
                        }
                }
                //Некорректный профиль, проверка следующего
                if (!IsProfileCorrect)
                    continue;
                _dbService.InsertMmeCodeToProfile(Profile.Id, SelectedMmeCode);
                ProfilesForMmeCodeSource.Remove(Profile);
                ProfilesFromMmeCodeSource.Add(Profile);
            }
        }, o => SelectedProfilesForMmeCode.Count > 0);
        
        private bool GTUParameters_Compare(XElement mme, BaseTestParametersAndNormatives parameters) //Проверка параметров GTU
        {
            Types.GTU.TestParameters Parameters = (Types.GTU.TestParameters)parameters;
            //Требуемый блок
            XElement NeededBlock = mme.Element("mme.blocks").Elements("block").FirstOrDefault(block => block.Attribute("name").Value == "GTU");
            if (!Parameters_Compare(NeededBlock, "Itm", Parameters.Itm))
                return false;
            if (!Parameters_Compare(NeededBlock, "CurrentLimit", Parameters.CurrentLimit))
                return false;
            if (!Parameters_Compare(NeededBlock, "VoltageLimit", Parameters.VoltageLimitD))
                return false;
            if (!Parameters_Compare(NeededBlock, "PlateTime", Parameters.PlateTime))
                return false;
            if (!Parameters_Compare(NeededBlock, "RampUpVoltage", Parameters.RampUpVoltage))
                return false;
            if (!Parameters_Compare(NeededBlock, "StartVoltage", Parameters.StartVoltage))
                return false;
            return true;
        }

        private bool SLParameters_Compare(XElement mme, BaseTestParametersAndNormatives parameters) //Проверка параметров SL
        {
            Types.VTM.TestParameters Parameters = (Types.VTM.TestParameters)parameters;
            //Требуемый блок
            XElement NeededBlock = mme.Element("mme.blocks").Elements("block").FirstOrDefault(block => block.Attribute("name").Value == "SL");
            if (!Parameters_Compare(NeededBlock, "RampCurrent", Parameters.RampCurrent))
                return false;
            if (!Parameters_Compare(NeededBlock, "RampTime", Parameters.RampTime))
                return false;
            if (!Parameters_Compare(NeededBlock, "RampOpeningCurrent", Parameters.RampOpeningCurrent))
                return false;
            if (!Parameters_Compare(NeededBlock, "RampOpeningTime", Parameters.RampOpeningTime))
                return false;
            if (!Parameters_Compare(NeededBlock, "SinusCurrent", Parameters.SinusCurrent))
                return false;
            if (!Parameters_Compare(NeededBlock, "SinusTime", Parameters.SinusTime))
                return false;
            if (!Parameters_Compare(NeededBlock, "CurveCurrent", Parameters.CurveCurrent))
                return false;
            if (!Parameters_Compare(NeededBlock, "CurveTime", Parameters.CurveTime))
                return false;
            if (!Parameters_Compare(NeededBlock, "CurveFactor", Parameters.CurveFactor))
                return false;
            if (!Parameters_Compare(NeededBlock, "CurveAddTime", Parameters.CurveAddTime))
                return false;
            if (!Parameters_Compare(NeededBlock, "Count", Parameters.Count))
                return false;
            return true;
        }

        private bool BVTParameters_Compare(XElement mme, BaseTestParametersAndNormatives parameters) //Проверка параметров BVT
        {
            Types.BVT.TestParameters Parameters = (Types.BVT.TestParameters)parameters;
            //Требуемый блок
            XElement NeededBlock = mme.Element("mme.blocks").Elements("block").FirstOrDefault(block => block.Attribute("name").Value == "BVT");
            if (!Parameters_Compare(NeededBlock, "CurrentLimit", Parameters.CurrentLimit))
                return false;
            if (!Parameters_Compare(NeededBlock, "VoltageLimitD", Parameters.VoltageLimitD))
                return false;
            if (!Parameters_Compare(NeededBlock, "VoltageLimitR", Parameters.VoltageLimitR))
                return false;
            if (!Parameters_Compare(NeededBlock, "PlateTime", Parameters.PlateTime))
                return false;
            if (!Parameters_Compare(NeededBlock, "RampUpVoltage", Parameters.RampUpVoltage))
                return false;
            if (!Parameters_Compare(NeededBlock, "StartVoltage", Parameters.StartVoltage))
                return false;
            if (!Parameters_Compare(NeededBlock, "UdsmUrsmCurrentLimit", Parameters.UdsmUrsmCurrentLimit))
                return false;
            if (!Parameters_Compare(NeededBlock, "UdsmUrsmVoltageLimitD", Parameters.UdsmUrsmVoltageLimitD))
                return false;
            if (!Parameters_Compare(NeededBlock, "UdsmUrsmVoltageLimitR", Parameters.UdsmUrsmVoltageLimitR))
                return false;
            if (!Parameters_Compare(NeededBlock, "UdsmUrsmPlateTime", Parameters.UdsmUrsmPlateTime))
                return false;
            if (!Parameters_Compare(NeededBlock, "UdsmUrsmRampUpVoltage", Parameters.UdsmUrsmRampUpVoltage))
                return false;
            if (!Parameters_Compare(NeededBlock, "UdsmUrsmStartVoltage", Parameters.UdsmUrsmStartVoltage))
                return false;
            return true;
        }

        private bool dUdtParameters_Compare(XElement mme, BaseTestParametersAndNormatives parameters) //Проверка параметров dUdt
        {
            Types.dVdt.TestParameters Parameters = (Types.dVdt.TestParameters)parameters;
            //Требуемый блок
            XElement NeededBlock = mme.Element("mme.blocks").Elements("block").FirstOrDefault(block => block.Attribute("name").Value == "dUdt");
            if (!Parameters_Compare(NeededBlock, "Voltage", Parameters.Voltage))
                return false;
            if (!Parameters_Compare(NeededBlock, "ConfirmationCount", Parameters.ConfirmationCount))
                return false;
            if (!Parameters_Compare(NeededBlock, "VoltageRateOffset", Parameters.VoltageRateOffSet))
                return false;
            if (!Parameters_Compare(NeededBlock, "VoltageRateLimit", Parameters.VoltageRateLimit))
                return false;
            return true;
        }

        private bool ATUParameters_Compare(XElement mme, BaseTestParametersAndNormatives parameters) //Проверка параметров ATU
        {
            Types.ATU.TestParameters Parameters = (Types.ATU.TestParameters)parameters;
            //Требуемый блок
            XElement NeededBlock = mme.Element("mme.blocks").Elements("block").FirstOrDefault(block => block.Attribute("name").Value == "ATU");
            if (!Parameters_Compare(NeededBlock, "PrePulseValue", Parameters.PrePulseValue))
                return false;
            if (!Parameters_Compare(NeededBlock, "PowerValue", Parameters.PowerValue))
                return false;
            return true;
        }

        private bool QrrTqParameters_Compare(XElement mme, BaseTestParametersAndNormatives parameters) //Проверка параметров QrrTq
        {
            Types.QrrTq.TestParameters Parameters = (Types.QrrTq.TestParameters)parameters;
            //Требуемый блок
            XElement NeededBlock = mme.Element("mme.blocks").Elements("block").FirstOrDefault(block => block.Attribute("name").Value == "QrrTq");
            if (!Parameters_Compare(NeededBlock, "DirectCurrent", Parameters.DirectCurrent))
                return false;
            if (!Parameters_Compare(NeededBlock, "DCPulseWidth", Parameters.DCPulseWidth))
                return false;
            if (!Parameters_Compare(NeededBlock, "DCRiseRate", Parameters.DCRiseRate))
                return false;
            if (!Parameters_Compare(NeededBlock, "OffStateVoltage", Parameters.OffStateVoltage))
                return false;
            return true;
        }

        private bool TOUParameters_Compare(XElement mme, BaseTestParametersAndNormatives parameters) //Проверка параметров TOU
        {
            Types.TOU.TestParameters Parameters = (Types.TOU.TestParameters)parameters;
            //Требуемый блок
            XElement NeededBlock = mme.Element("mme.blocks").Elements("block").FirstOrDefault(block => block.Attribute("name").Value == "TOU");
            if (!Parameters_Compare(NeededBlock, "CurrentAmplitude", Parameters.CurrentAmplitude))
                return false;
            return true;
        }

        private bool Parameters_Compare(XElement neededBlock, string parameterName, double value) //Сравнение параметров с нормативными по данному комплексу
        {
            //Требуемый параметр
            XElement Parameter = neededBlock.Element("block.parameters").Elements("parameter").FirstOrDefault(parameter => parameter.Attribute("name").Value == parameterName);
            //Граничащие значения
            double MinValue = double.Parse(Parameter.Attribute("minValue").Value);
            double MaxValue = double.Parse(Parameter.Attribute("maxValue").Value);
            if (value < MinValue || value > MaxValue)
                return false;
            return true;
        }

        public RelayCommand RemoveProfileFromMmeCode => new RelayCommand(o =>
        {
            foreach (var i in SelectedProfilesFromMmeCode.ToList())
            {
                _dbService.RemoveMmeCodeToProfile(i.Id, SelectedMmeCode);
                ProfilesFromMmeCodeSource.Remove(i);
                ProfilesForMmeCodeSource.Add(i);
            }
        }, o => SelectedProfilesFromMmeCode.Count > 0);
        
        #endregion

        #region MmeCodes

        public ObservableCollection<string> MmeCodes { get; set; }

        public string NewMmeCode { get; set; } = string.Empty;

        public RelayCommand AddMmeCode => new RelayCommand((o) =>
        {
            if (new DialogWindow(WpfControlLibrary.Properties.Resources.Confirmation, $"{WpfControlLibrary.Properties.Resources.AddMmeCode}: {NewMmeCode}", true).ShowDialogWithResult() == false)
                return;

            _dbService.InsertMmeCode(NewMmeCode);
            MmeCodes.Add(NewMmeCode);
        }, (o) => string.IsNullOrWhiteSpace((NewMmeCode)) == false && MmeCodes.Contains(NewMmeCode) == false);

        public string SelectedMmeCodeFromRemove { get; set; }

        public RelayCommand RemoveMmeCode => new RelayCommand((o) =>
        {
            if (new DialogWindow(WpfControlLibrary.Properties.Resources.Confirmation, $"{WpfControlLibrary.Properties.Resources.RemoveMmeCode}: {SelectedMmeCodeFromRemove}", true).ShowDialogWithResult() == false)
                return;
            _dbService.RemoveMmeCode(SelectedMmeCodeFromRemove);
            MmeCodes.Remove(SelectedMmeCodeFromRemove);

        }, (o) => SelectedMmeCodeFromRemove != null);

        #endregion
    }
}