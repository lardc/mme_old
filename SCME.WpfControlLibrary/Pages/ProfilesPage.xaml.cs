﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using SCME.InterfaceImplementations.NewImplement.SQLite;
using SCME.Types;
using SCME.Types.BaseTestParams;
using SCME.Types.Database;
using SCME.Types.Profiles;
using SCME.WpfControlLibrary.CustomControls;
using SCME.WpfControlLibrary.ViewModels;

namespace SCME.WpfControlLibrary.Pages
{
    /// <summary>
    /// Логика взаимодействия для ProfilesPage.xaml
    /// </summary>
    public partial class ProfilesPage
    {
        public ProfilesPageProfileVm ProfileVm { get; set; }
        private readonly IDbService _dbService;
        private readonly bool _isWithoutChild;
        
        private readonly DispatcherTimer _dispatcherTimerFindProfile = new DispatcherTimer();

        public event Action GoBackAction;
        public event Action AfterLoadAction;
        
        private Dictionary<string, int> GetMMeCodes => ProfileVm.IsSingleMmeCode
            ? _dbService.GetMmeCodes().Where(m => m.Key == ProfileVm.SelectedMmeCode).ToDictionary(m => m.Key, m => m.Value)
            : _dbService.GetMmeCodes().Where(m => m.Key != Constants.MME_CODE_IS_ACTIVE_NAME).ToDictionary(m => m.Key, m => m.Value);
        
        public ProfilesPage(IDbService dbService, string mmeCode, bool isSingleMmeCode = false, bool isWithoutChild = false, bool readOnlyMode = false, bool specialMeasure = false)
        {
            ProfileVm = new ProfilesPageProfileVm(dbService);

            if (dbService == null) throw new ArgumentNullException(nameof(dbService));
            if (mmeCode == null) throw new ArgumentNullException(nameof(mmeCode));
//            if (mmeCode.Trim() == "")  throw new ArgumentException (nameof(mmeCode)); 

            ProfileVm.IsSingleMmeCode = isSingleMmeCode;
            InitializeComponent();

            AddTestParameterUserControl.IsReadOnly = ProfileVm.ReadOnlyMode = readOnlyMode;

            _dbService = dbService;
            _isWithoutChild = isWithoutChild;

            ProfileVm.SpecialMeasure = specialMeasure;
            
            ProfileVm.MmeCodes = GetMMeCodes;
            
            if (mmeCode == string.Empty)
                mmeCode = ProfileVm.MmeCodes.First().Key;
            
            ProfileVm.SelectedMmeCode = mmeCode;

            if (!ProfileVm.MmeCodes.ContainsKey(mmeCode))
            {
                _dbService.InsertMmeCode(mmeCode);
                ProfileVm.MmeCodes = GetMMeCodes;
            }

            _dispatcherTimerFindProfile.Tick += OnDispatcherTimerFindProfileOnTick;
            _dispatcherTimerFindProfile.Interval = new TimeSpan(0, 0, 1);
        }

        private void OnDispatcherTimerFindProfileOnTick(object o1, EventArgs args1)
        {
            _dispatcherTimerFindProfile.Stop();
            ProfileVm.ProfilesSource.View.Refresh();
        }


        public void LoadTopProfiles() =>
            ProfileVm.ProfilesSource.Source = ProfileVm.Profiles = new ObservableCollection<MyProfile>(_dbService.GetProfilesSuperficially(ProfileVm.SelectedMmeCode));


//        private void ProfilesTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
//        {
//            if (_isIgnoreTreeViewSelectionChanged)
//                return;
//
//            ProfileVm.SelectedProfile = e.NewValue as MyProfile;
//            if (ProfileVm.SelectedProfile == null)
//                return;
//
//            ProfileVm.SelectedProfileNameCopy = ProfileVm.SelectedProfile.Name;
//            ProfileVm.SelectedProfile.DeepData = _dbService.LoadProfileDeepData(ProfileVm.SelectedProfile);
//            if (ProfileVm.SelectedProfile.IsTop && _isWithoutChild == false)
//            {
//                ProfileVm.SelectedProfile.Children = new ObservableCollection<MyProfile>(_dbService.GetProfileChildSuperficially(ProfileVm.SelectedProfile));
//                foreach (var i in ProfileVm.SelectedProfile.Children)
//                    i.IsTop = false;
//            }
//
//            ProfileVm.ProfileDeepDataCopy = ProfileVm.SelectedProfile.DeepData;
//        }


        private void BeginEditProfile_Click(object sender, RoutedEventArgs e)
        {
            //((TreeViewItem) ProfilesTreeView.ItemContainerGenerator.ContainerFromItem(ProfileVm.SelectedProfile)).IsSelected = true;
            ProfileVm.ProfileDeepDataCopy = ProfileVm.SelectedProfile.DeepData.Copy();
            ProfileVm.SelectedProfileNameCopy = ProfileVm.SelectedProfile.Name.Copy();
            ProfileVm.IsEditModeActive = true;
        }

        private void CreateNewProfile_Click(object sender, RoutedEventArgs e)
        {
//            if (ProfileVm.SelectedProfile != null)
//                ((TreeViewItem) ProfilesTreeView.ItemContainerGenerator.ContainerFromItem(ProfileVm.SelectedProfile)).IsSelected = false;
            ProfileVm.ProfileDeepDataCopy = new ProfileDeepData();
            ProfileVm.SelectedProfileNameCopy = _dbService.GetFreeProfileName();
            ProfileVm.IsEditModeActive = true;
        }

        private bool CheckName(string oldName)
        {
            if (ProfileVm.SelectedProfileNameCopy.Equals(oldName))
                return true;
            if (_dbService.ProfileNameExists(ProfileVm.SelectedProfileNameCopy) == false)
                return true;
            new DialogWindow(Properties.Resources.Error, "").ShowDialog();
            return false;
        }


        private void EndEditProfile_Click(object sender, RoutedEventArgs e)
        {
            var oldName = ProfileVm.SelectedProfile?.Name;
            if (CheckName(oldName) == false)
                return;

            ProfileVm.IsEditModeActive = false;

            var oldProfile = ProfileVm.SelectedProfile;
            MyProfile newProfile;
            if (oldProfile == null)
            {
                newProfile = new MyProfile(0, ProfileVm.SelectedProfileNameCopy, Guid.NewGuid(), 0, DateTime.Now).GenerateNextVersion(ProfileVm.ProfileDeepDataCopy, ProfileVm.SelectedProfileNameCopy);
                if(!ProfileVm.SpecialMeasure)
                    newProfile.Id = _dbService.InsertUpdateProfile(oldProfile, newProfile, ProfileVm.SelectedMmeCode);
                
                ProfileVm.Profiles.Insert(0, newProfile);
            }
            else
            {
                newProfile = oldProfile.GenerateNextVersion(ProfileVm.ProfileDeepDataCopy, ProfileVm.SelectedProfileNameCopy);
                if(!ProfileVm.SpecialMeasure)
                    newProfile.Id = _dbService.InsertUpdateProfile(oldProfile, newProfile, ProfileVm.SelectedMmeCode);

                ProfileVm.Profiles.Insert(ProfileVm.Profiles.IndexOf(oldProfile), newProfile);
                ProfileVm.Profiles.Remove(oldProfile);
            }

            ProfileVm.SelectedProfile = newProfile;
        }

        private void CancelEditProfile_Click(object sender, RoutedEventArgs e)
        {
            ProfileVm.IsEditModeActive = false;
            if (ProfileVm.SelectedProfile == null)
            {
                ProfileVm.ProfileDeepDataCopy = null;
                ProfileVm.SelectedProfileNameCopy = string.Empty;
                return;
            }

            ProfileVm.ProfileDeepDataCopy = ProfileVm.SelectedProfile.DeepData;
            ProfileVm.SelectedProfileNameCopy = ProfileVm.SelectedProfile.Name;
        }

        private void AddTestParametersEvent_Click()
        {
            var testParametersAndNormatives = ProfileVm.ProfileDeepDataCopy.TestParametersAndNormatives;
            var maxOrder = testParametersAndNormatives.Count > 0 ? testParametersAndNormatives.Max(m => m.Order) : 0;

            var newTestParameter = BaseTestParametersAndNormatives.CreateParametersByType(ProfileVm.SelectedTestParametersType);
            newTestParameter.IsEnabled = true;
            newTestParameter.Order = maxOrder + 1;
            testParametersAndNormatives.Add(newTestParameter);
        }


        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProfileVm.ProfileDeepDataCopy = null;
            ProfileVm.SearchingName = string.Empty;
            LoadTopProfiles();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (ProfileVm.MmeCodes.Count == 0)
                MessageBox.Show(Properties.Resources.Error, Properties.Resources.MissingMMECodes);
            AfterLoadAction?.Invoke();
        }

        public void RefreshProfile(MyProfile newProfile)
        {
          
                
            ProfileVm.Profiles.Insert(ProfileVm.Profiles.IndexOf(ProfileVm.SelectedProfile), newProfile);
            ProfileVm.Profiles.Remove(ProfileVm.SelectedProfile);
            ProfileVm.SelectedProfile = newProfile;
           
        }


        private void TextBoxFind_TextChanged(object sender, TextChangedEventArgs e)
        {
            _dispatcherTimerFindProfile.Start();
        }

        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            ProfileVm.SelectedProfile = null;
            ProfileVm.ProfileDeepDataCopy = null;
            if (GoBackAction != null)
                GoBackAction.Invoke();
            else
                NavigationService?.GoBack();
        }
    }
}