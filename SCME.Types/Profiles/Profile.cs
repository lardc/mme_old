﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;
using SCME.Types.BaseTestParams;
using SCME.Types.SQL;

namespace SCME.Types.Profiles
{
    [DataContract(Namespace = "http://proton-electrotex.com/SCME", IsReference = true)]
    [KnownType(typeof(Profile))]
    [KnownType(typeof(ProfileSet))]
    [KnownType(typeof(ProfileFolder))]
    public abstract class ProfileDictionaryObject : INotifyPropertyChanged
    {
        [DataMember]
        private string m_Name;

        public string Name
        {
            get { return m_Name; }
            set
            {
                m_Name = value;
                OnPropertyChanged("Name");
            }
        }
        [DataMember]
        public Guid Key { get; set; }

        [DataMember]
        public Guid NextGenerationKey { get; set; }

        [DataMember]
        public int Version { get; set; }

        [DataMember]
        public DateTime Timestamp { get; set; }


        public string TimeStampFormated
        {
            get { return Timestamp.ToString("dd.MM.yyyy HH:mm"); }
        }

        [DataMember]
        public ObservableCollection<ProfileDictionaryObject> ChildrenList { get; private set; }
        [DataMember]
        public ProfileDictionaryObject Parent { get; set; }

        public ProfileDictionaryObject()
        {
            Name = "Default";
            Key = new Guid();
            Timestamp = DateTime.Now;
            Version = 1;

            ChildrenList = new ObservableCollection<ProfileDictionaryObject>();
        }

        public ProfileDictionaryObject(string Name, Guid Key, int version, DateTime Timestamp)
        {
            this.Name = Name;
            this.Key = Key;
            this.Timestamp = Timestamp;
            this.Version = version;

            ChildrenList = new ObservableCollection<ProfileDictionaryObject>();
        }

        public override string ToString()
        {
            return Name;
        }

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string PropertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(PropertyName));
        }

        #endregion
    }

    public sealed class ProfileFolder : ProfileDictionaryObject
    {
        public ProfileFolder()
        {

        }

        public ProfileFolder(string Name, Guid Key, DateTime TimeStamp)
           : base(Name, Key, 0, TimeStamp)
        {
            throw new NotImplementedException();
        }

        public ProfileFolder(string Name, Guid Key, int version, DateTime TimeStamp)
            : base(Name, Key, version, TimeStamp)
        {

        }
    }

    public sealed class ProfileSet : ProfileDictionaryObject
    {
        public ProfileSet()
        {

        }

        public ProfileSet(string Name, Guid Key, int version, DateTime TimeStamp)
            : base(Name, Key, version, TimeStamp)
        {

        }

        public ProfileSet(string Name, Guid Key, DateTime TimeStamp)
           : base(Name, Key, 0, TimeStamp)
        {
            throw new NotImplementedException();
        }

        
    }

    [DataContract(Namespace = "http://proton-electrotex.com/SCME")]
    public sealed class Profile : ProfileDictionaryObject
    {
        [DataMember]
        private bool m_IsTop;

        [DataMember]
        public ObservableCollection<BaseTestParametersAndNormatives> TestParametersAndNormatives { get; set; }

        [DataMember]
        public Gate.TestParameters ParametersGate { get; set; }
        [DataMember]
        public Gate.ResultNormatives NormativesGate { get; set; }
        [DataMember]
        public SL.TestParameters ParametersVTM { get; set; }
        [DataMember]
        public SL.ResultNormatives NormativesVTM { get; set; }
        [DataMember]
        public BVT.TestParameters ParametersBVT { get; set; }
        [DataMember]
        public BVT.ResultNormatives NormativesBVT { get; set; }
        [DataMember]
        public Commutation.ModuleCommutationType ParametersComm { get; set; }
        [DataMember]
        public float ParametersClamp { get; set; }
        [DataMember]
        public bool IsHeightMeasureEnabled { get; set; }
        [DataMember]
        public int Height { get; set; }
        [DataMember]
        public int Temperature { get; set; }

        public bool IsParent { get; set; }

        public List<Profile> ChilProfiles { get; set; }


        public bool IsTop
        {
            get { return m_IsTop; }
            set
            {
                m_IsTop = value;
                OnPropertyChanged("IsTop");
            }
        }


        private void ConstructorInit()
        {
            ParametersGate = new Gate.TestParameters();
            ParametersVTM = new SL.TestParameters();
            ParametersBVT = new BVT.TestParameters();
            ParametersComm = new Commutation.ModuleCommutationType();
            ParametersClamp = 5.0f;
            TestParametersAndNormatives = new ObservableCollection<BaseTestParametersAndNormatives>(new List<BaseTestParametersAndNormatives>());


            NormativesGate = new Gate.ResultNormatives();
            NormativesVTM = new SL.ResultNormatives();
            NormativesBVT = new BVT.ResultNormatives();
        }

        public Profile()
        {
            ConstructorInit();
        }

        public Profile(ProfileForSqlSelect prof)
           : base(prof.Name, prof.Key, Convert.ToInt32(prof.Version), prof.TS)
        {
            ConstructorInit();
        }

        public Profile(string Name, Guid Key, long version, DateTime TS)
            : base(Name, Key, Convert.ToInt32(version), TS)
        {
            ConstructorInit();
        }

        public Profile(string Name, Guid Key, DateTime TS)
          : base(Name, Key, 0, TS)
        {
            throw new NotImplementedException();
        }
    }
}
