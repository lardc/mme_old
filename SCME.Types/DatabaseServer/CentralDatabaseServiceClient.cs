﻿using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using SCME.Types.DataContracts;
using SCME.Types.Profiles;
using SCME.Types.SQL;

namespace SCME.Types.DatabaseServer
{
    public class CentralDatabaseServiceClient : ClientBase<ICentralDatabaseService>, ICentralDatabaseService
    {
        public CentralDatabaseServiceClient() { }
        public CentralDatabaseServiceClient(string uri) : base(WcfClientBindings.DefaultNetTcpBinding, new EndpointAddress(uri))
        {

        }

        public int? ReadDeviceRTClass(string devCode, string profileName)
        {
            return Channel.ReadDeviceRTClass(devCode, profileName);
        }

        public int? ReadDeviceClass(string devCode, string profileName)
        {
            return Channel.ReadDeviceClass(devCode, profileName);
        }

        public void Check()
        {
            Channel.Check();
        }

        public long SaveResults(ResultItem results, List<string> errors)
        {
            return Channel.SaveResults(results, errors);
        }

        public List<ProfileItem> GetProfileItems()
        {
            return Channel.GetProfileItems();
        }

        public List<ProfileItem> GetProfileItemsByMme(string mmeCode)
        {
            return Channel.GetProfileItemsByMme(mmeCode);
        }

        public ProfileItem GetProfileByProfName(string profName, string mmmeCode, ref bool Found)
        {
            return Channel.GetProfileByProfName(profName, mmmeCode, ref Found);
        }

        public List<ProfileForSqlSelect> SaveProfiles(List<ProfileItem> profileItems)
        {
            return Channel.SaveProfiles(profileItems);
        }

        public List<ProfileForSqlSelect> SaveProfilesFromMme(List<ProfileItem> profileItems, string mmeCode)
        {
            return Channel.SaveProfilesFromMme(profileItems, mmeCode);
        }

        public List<string> GetGroups(DateTime? @from, DateTime? to)
        {
            return Channel.GetGroups(from, to);
        }



        public List<DeviceItem> GetDevices(string @group)
        {
            return Channel.GetDevices(@group);
        }

        public List<int> ReadDeviceErrors(long internalId)
        {
            return Channel.ReadDeviceErrors(internalId);
        }

        public List<ParameterItem> ReadDeviceParameters(long internalId)
        {
            return Channel.ReadDeviceParameters(internalId);
        }

        public List<ConditionItem> ReadDeviceConditions(long internalId)
        {
            return Channel.ReadDeviceConditions(internalId);
        }

        public List<ParameterNormativeItem> ReadDeviceNormatives(long internalId)
        {
            return Channel.ReadDeviceNormatives(internalId);
        }

        public bool SendResultToServer(DeviceLocalItem localDevice)
        {
            return Channel.SendResultToServer(localDevice);
        }

        public IEnumerable<MmeCode> GetMmeCodes()
        {
            return Channel.GetMmeCodes();
        }

        public IEnumerable<ProfileMme> GetMmeProfiles(long mmeCodeId)
        {
            return Channel.GetMmeProfiles(mmeCodeId);
        }

        public void SaveConnections(List<MmeCode> mmeCodes)
        {
            Channel.SaveConnections(mmeCodes);
        }

        public List<ProfileItem> GetProfileItemsSuperficially(string mmeCode)
        {
            return Channel.GetProfileItemsSuperficially(mmeCode);
        }

        public List<ProfileItem> GetProfileItemsDeep(string mmeCode)
        {
            return Channel.GetProfileItemsDeep(mmeCode);
        }

        public List<ProfileItem> GetProfileItemsWithChildSuperficially(string mmeCode)
        {
            return Channel.GetProfileItemsWithChildSuperficially(mmeCode);
        }

        public Profile GetProfileDeep(Guid key)
        {
            return Channel.GetProfileDeep(key);
        }
    }
}
