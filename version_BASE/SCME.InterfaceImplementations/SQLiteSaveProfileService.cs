﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using SCME.Interfaces;
using SCME.Types;
using SCME.Types.BaseTestParams;
using SCME.Types.BVT;
using SCME.Types.dVdt;
using SCME.Types.SL;
using TestParameters = SCME.Types.Gate.TestParameters;

namespace SCME.InterfaceImplementations
{
    public class SQLiteSaveProfileService : ISaveProfileService
    {
        private readonly SQLiteConnection _connection;

        public SQLiteSaveProfileService(SQLiteConnection connection)
        {
            _connection = connection;
            _testTypes = new Dictionary<string, long>(5);
            _conditions = new Dictionary<string, long>(64);
            _params = new Dictionary<string, long>(64);
            if (_connection.State != ConnectionState.Open)
                _connection.Open();
        }

        private readonly Dictionary<string, long> _testTypes;
        private readonly Dictionary<string, long> _conditions;
        private readonly Dictionary<string, long> _params;
        private int orderNew;

        /// <summary>
        /// Populate dictionaries from db
        /// </summary>
        private void PopulateDictionaries()
        {
            if (_connection != null && _connection.State == ConnectionState.Open)
            {
                _testTypes.Clear();
                _conditions.Clear();
                _params.Clear();

                using (var condCmd = _connection.CreateCommand())
                {
                    condCmd.CommandText = "SELECT C.COND_ID, C.COND_NAME FROM CONDITIONS C";

                    using (var reader = condCmd.ExecuteReader())
                    {
                        while (reader.Read())
                            _conditions.Add((string)reader[1], (long)reader[0]);
                    }
                }

                using (var paramCmd = _connection.CreateCommand())
                {
                    paramCmd.CommandText = "SELECT P.PARAM_ID, P.PARAM_NAME FROM PARAMS P";

                    using (var reader = paramCmd.ExecuteReader())
                    {
                        while (reader.Read())
                            _params.Add((string)reader[1], (long)reader[0]);
                    }
                }

                using (var typesCmd = _connection.CreateCommand())
                {
                    typesCmd.CommandText = "SELECT T.ID, T.NAME FROM TEST_TYPE T";

                    using (var reader = typesCmd.ExecuteReader())
                    {
                        while (reader.Read())
                            _testTypes.Add((string)reader[1], (long)reader[0]);
                    }
                }
            }
        }

        /// <summary>
        /// Saving profile to Db
        /// </summary>
        /// <param name="profileItem"></param>
        public long SaveProfileItem(ProfileItem profileItem)
        {
            if (_testTypes.Count == 0)
                PopulateDictionaries();
            if (_connection != null && _connection.State == ConnectionState.Open)
            {
                var trans = _connection.BeginTransaction();

                try
                {
                    orderNew = 0;
                    var profileId = InsertProfile(profileItem);

                    var commutationTestTypeId = InsertCommutationTestType(profileId);
                    InsertCommutationConditions(profileItem, commutationTestTypeId, profileId);

                    var clampingTestTypeId = InsertClampingTestType(profileId);
                    InsertClampingConditions(profileItem, clampingTestTypeId, profileId);

                    foreach (var gateTestParameter in profileItem.GateTestParameters)
                    {
                        var testTypeId = InsertGateTestType(profileId, gateTestParameter.Order);
                        gateTestParameter.IsEnabled = true;
                        InsertConditions(gateTestParameter, testTypeId, profileId);
                        InsertParameters(gateTestParameter, testTypeId, profileId);
                    }

                    foreach (var bvtTestParameter in profileItem.BVTTestParameters)
                    {
                        var testTypeId = InsertBvtTestType(profileId, bvtTestParameter.Order);
                        bvtTestParameter.IsEnabled = true;
                        InsertConditions(bvtTestParameter, testTypeId, profileId);
                        InsertParameters(bvtTestParameter, testTypeId, profileId);
                    }

                    foreach (var vtmTestParameter in profileItem.VTMTestParameters)
                    {
                        var testTypeId = InsertSlTestType(profileId, vtmTestParameter.Order);
                        vtmTestParameter.IsEnabled = true;
                        InsertConditions(vtmTestParameter, testTypeId, profileId);
                        InsertParameters(vtmTestParameter, testTypeId, profileId);
                    }

                    foreach (var dvDTestParameter in profileItem.DvDTestParameterses)
                    {
                        var testTypeId = InsertDvdtTestType(profileId, dvDTestParameter.Order);
                        dvDTestParameter.IsEnabled = true;
                        InsertConditions(dvDTestParameter, testTypeId, profileId);
                        InsertParameters(dvDTestParameter, testTypeId, profileId);
                    }

                    trans.Commit();
                    return profileId;
                }
                catch (Exception)
                {
                    trans.Rollback();
                    throw;
                }
            }
            return 0;
        }


        public void DeleteProfiles(List<ProfileItem> profilesToDelete)
        {
            foreach (var profileItem in profilesToDelete)
            {
                try
                {
                    var profileId = GetProfileId(profileItem.ProfileKey);

                    //var condDeleteCommand = new SQLiteCommand("DELETE FROM PROF_COND WHERE PROF_ID = @PROF_ID", _connection);
                    //condDeleteCommand.Parameters.Add("@PROF_ID", DbType.Int64);
                    //condDeleteCommand.Prepare();
                    //condDeleteCommand.Parameters["@PROF_ID"].Value = profileId;
                    //condDeleteCommand.ExecuteNonQuery();

                    //var profParamDeleteCommand = new SQLiteCommand("DELETE FROM PROF_PARAM WHERE PROF_ID = @PROF_ID", _connection);
                    //profParamDeleteCommand.Parameters.Add("@PROF_ID", DbType.Int64);
                    //profParamDeleteCommand.Prepare();
                    //profParamDeleteCommand.Parameters["@PROF_ID"].Value = profileId;
                    //profParamDeleteCommand.ExecuteNonQuery();

                    //var profTestTypeDeleteCommand = new SQLiteCommand("DELETE FROM PROF_TEST_TYPE WHERE PROF_ID = @PROF_ID", _connection);
                    //profTestTypeDeleteCommand.Parameters.Add("@PROF_ID", DbType.Int64);
                    //profTestTypeDeleteCommand.Prepare();
                    //profTestTypeDeleteCommand.Parameters["@PROF_ID"].Value = profileId;
                    //profTestTypeDeleteCommand.ExecuteNonQuery();

                    var profileDeleteCommand = new SQLiteCommand("UPDATE PROFILES SET IsDelete=1 WHERE PROF_ID = @PROF_ID", _connection);
                    profileDeleteCommand.Parameters.Add("@PROF_ID", DbType.Int64);
                    profileDeleteCommand.Prepare();
                    profileDeleteCommand.Parameters["@PROF_ID"].Value = profileId;
                    profileDeleteCommand.ExecuteNonQuery();


                }
                catch (ArgumentException)
                {
                }

            }
        }

        public void DeleteProfiles(List<ProfileItem> profilesToDelete, string mmeCode)
        {
            foreach (var profileItem in profilesToDelete)
            {
                try
                {
                    var profileId = GetProfileId(profileItem.ProfileKey);
                    var mmeCodeId = GetMmeCodeId(mmeCode);

                    var condDeleteCommand = new SQLiteCommand("DELETE FROM MME_CODES_TO_PROFILES WHERE PROFILE_ID = @PROF_ID AND MME_CODE_ID=@MME_CODE_ID", _connection);
                    condDeleteCommand.Parameters.Add("@PROF_ID", DbType.Int64);
                    condDeleteCommand.Parameters.Add("@MME_CODE_ID", DbType.Int64);
                    condDeleteCommand.Prepare();
                    condDeleteCommand.Parameters["@PROF_ID"].Value = profileId;
                    condDeleteCommand.Parameters["@MME_CODE_ID"].Value = mmeCodeId;
                    condDeleteCommand.ExecuteNonQuery();
                }
                catch (ArgumentException)
                {
                }

            }
        }

        private long GetMmeCodeId(string mmeCode)
        {
            var mmeSelectCommand = new SQLiteCommand("SELECT MME_CODE_ID FROM MME_CODES WHERE MME_CODE = @MME_CODE", _connection);
            mmeSelectCommand.Parameters.Add("@MME_CODE", DbType.String);
            mmeSelectCommand.Prepare();
            mmeSelectCommand.Parameters["@MME_CODE"].Value = mmeCode;
            var posibleMmeCode = mmeSelectCommand.ExecuteScalar();

            if (posibleMmeCode != null) return (long)posibleMmeCode;

            var mmeInsertCommand = new SQLiteCommand("INSERT INTO MME_CODES (MME_CODE) VALUES (@MME_CODE)", _connection);
            mmeInsertCommand.Parameters.Add("@MME_CODE", DbType.String);
            mmeInsertCommand.Prepare();
            mmeInsertCommand.Parameters["@MME_CODE"].Value = mmeCode;
            mmeInsertCommand.ExecuteNonQuery();
            return _connection.LastInsertRowId;
        }

        public void SaveProfileItem(ProfileItem profileItem, string mmeCode)
        {
            var profileId = SaveProfileItem(profileItem);
            var mmeCodeId = GetMmeCodeId(mmeCode);

            var codesInsertCommand =
              new SQLiteCommand("INSERT INTO MME_CODES_TO_PROFILES (MME_CODE_ID,PROFILE_ID) VALUES (@MME_CODE_ID,@PROFILE_ID)", _connection);
            codesInsertCommand.Parameters.Add("@MME_CODE_ID", DbType.Int64);
            codesInsertCommand.Parameters.Add("@PROFILE_ID", DbType.Int64);
            codesInsertCommand.Prepare();
            codesInsertCommand.Parameters["@MME_CODE_ID"].Value = mmeCodeId;
            codesInsertCommand.Parameters["@PROFILE_ID"].Value = profileId;
            codesInsertCommand.ExecuteScalar();

        }

        #region ProfileHelpers

        private long InsertProfile(ProfileItem profile)
        {
            try
            {
                var profileId = GetProfileId(profile.ProfileKey);

                var profileVersionSelect =
                    new SQLiteCommand("SELECT P.PROF_VERS FROM PROFILES P WHERE P.PROF_ID = @PROF_ID", _connection);
                profileVersionSelect.Parameters.Add("@PROF_ID", DbType.Int64);
                profileVersionSelect.Prepare();
                profileVersionSelect.Parameters["@PROF_ID"].Value = profileId;
                var version = profileVersionSelect.ExecuteScalar();

                var profileInsertCommand = new SQLiteCommand("INSERT INTO PROFILES(PROF_ID, PROF_NAME, PROF_GUID, PROF_TS,PROF_VERS) VALUES (NULL, @PROF_NAME, @PROF_GUID, @PROF_TS,@VERSION)", _connection);

                profileInsertCommand.Parameters.Add("@PROF_GUID", DbType.Guid);
                profileInsertCommand.Parameters.Add("@VERSION", DbType.Int64);
                profileInsertCommand.Parameters.Add("@PROF_TS", DbType.String);
                profileInsertCommand.Parameters.Add("@PROF_NAME", DbType.String);
                profileInsertCommand.Prepare();
                profileInsertCommand.Parameters["@PROF_GUID"].Value = Guid.NewGuid();
                profileInsertCommand.Parameters["@VERSION"].Value = (long)version + 1;
                profileInsertCommand.Parameters["@PROF_TS"].Value = DateTime.Now.ToString(@"yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                profileInsertCommand.Parameters["@PROF_NAME"].Value = profile.ProfileName;
                profileInsertCommand.ExecuteNonQuery();

                var insertedId = _connection.LastInsertRowId;

                UpdateConnections(profileId, insertedId);

                return insertedId;
            }
            catch (ArgumentException)
            {
                var profileInsertCommand = new SQLiteCommand("INSERT INTO PROFILES(PROF_ID, PROF_NAME, PROF_GUID, PROF_TS,PROF_VERS) VALUES(NULL, @PROF_NAME, @PROF_GUID, @PROF_TS,1)", _connection);
                profileInsertCommand.Parameters.Add("@PROF_NAME", DbType.String);
                profileInsertCommand.Parameters.Add("@PROF_GUID", DbType.Guid);
                profileInsertCommand.Parameters.Add("@PROF_TS", DbType.String);
                profileInsertCommand.Prepare();

                profileInsertCommand.Parameters["@PROF_NAME"].Value = profile.ProfileName;
                profileInsertCommand.Parameters["@PROF_GUID"].Value = profile.ProfileKey;
                profileInsertCommand.Parameters["@PROF_TS"].Value = DateTime.Now.ToString(@"yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                profileInsertCommand.ExecuteNonQuery();

                return _connection.LastInsertRowId;
            }

        }

        private void UpdateConnections(long profileId, long insertedId)
        {
            try
            {
                var connectionsUpdateCommand = new SQLiteCommand("UPDATE MME_CODES_TO_PROFILES SET PROFILE_ID=@NEW_PROFILE_ID WHERE PROFILE_ID=@PROFILE_ID", _connection);
                connectionsUpdateCommand.Parameters.Add("@NEW_PROFILE_ID", DbType.Int64);
                connectionsUpdateCommand.Parameters.Add("@PROFILE_ID", DbType.Int64);
                connectionsUpdateCommand.Prepare();
                connectionsUpdateCommand.Parameters["@NEW_PROFILE_ID"].Value = insertedId;
                connectionsUpdateCommand.Parameters["@PROFILE_ID"].Value = profileId;
                connectionsUpdateCommand.ExecuteNonQuery();
            }
            catch (Exception)
            {
                //ignore
            }

        }

        private long GetProfileId(Guid profileKey)
        {
            var profileSelectCommand = new SQLiteCommand("SELECT P.PROF_ID FROM PROFILES P WHERE P.PROF_GUID = @PROF_GUID", _connection);
            profileSelectCommand.Parameters.Add("@PROF_GUID", DbType.Guid);
            profileSelectCommand.Prepare();
            profileSelectCommand.Parameters["@PROF_GUID"].Value = profileKey;
            var possibleProfileId = profileSelectCommand.ExecuteScalar();

            if (possibleProfileId == null)
                throw new ArgumentException(@"No such baseTestParametersAndNormatives has been found", "profileKey");

            return (long)possibleProfileId;
        }


        #endregion

        #region TestTypeHelpers

        private long InsertCommutationTestType(long profileId)
        {
            return InsertTestType(_testTypes["Commutation"], profileId);
        }

        private long InsertClampingTestType(long profileId)
        {
            return InsertTestType(_testTypes["Clamping"], profileId);
        }

        private long InsertGateTestType(long profileId, int order)
        {
            return InsertTestType(_testTypes["Gate"], profileId, order);
        }

        private long InsertBvtTestType(long profileId, int order)
        {
            return InsertTestType(_testTypes["BVT"], profileId, order);
        }

        private long InsertSlTestType(long profileId, int order)
        {
            return InsertTestType(_testTypes["SL"], profileId, order);
        }

        private long InsertDvdtTestType(long profileId, int order)
        {
            return InsertTestType(_testTypes["Dvdt"], profileId, order);
        }

        private long InsertTestType(long typeId, long profileId, int order = 0)
        {
            orderNew++;
            var profTestTypeInsertCommand = new SQLiteCommand("INSERT INTO PROF_TEST_TYPE (PROF_ID,TEST_TYPE_ID,[ORDER]) VALUES (@PROF_ID, @TEST_TYPE_ID, @ORDER)", _connection);
            profTestTypeInsertCommand.Parameters.Add("@PROF_ID", DbType.Int64);
            profTestTypeInsertCommand.Parameters.Add("@TEST_TYPE_ID", DbType.Int32);
            profTestTypeInsertCommand.Parameters.Add("@ORDER", DbType.Int32);
            profTestTypeInsertCommand.Prepare();
            profTestTypeInsertCommand.Parameters["@PROF_ID"].Value = profileId;
            profTestTypeInsertCommand.Parameters["@TEST_TYPE_ID"].Value = typeId;
            profTestTypeInsertCommand.Parameters["@ORDER"].Value = order == 0 ? orderNew : order;
            profTestTypeInsertCommand.ExecuteNonQuery();

            return _connection.LastInsertRowId;
        }


        #endregion

        #region ConditionsHelpers

        private void InsertCommutationConditions(ProfileItem profile, long testTypeId, long profileId)
        {
            InsertCondition(testTypeId, profileId, "COMM_Type", profile.CommTestParameters);
        }

        private void InsertClampingConditions(ProfileItem profile, long testTypeId, long profileId)
        {
            InsertCondition(testTypeId, profileId, "CLAMP_Type", profile.ClampingForce);
            InsertCondition(testTypeId, profileId, "CLAMP_Force", profile.ParametersClamp);
            InsertCondition(testTypeId, profileId, "CLAMP_HeightMeasure", profile.IsHeightMeasureEnabled);
            InsertCondition(testTypeId, profileId, "CLAMP_HeightValue", profile.Height);
            InsertCondition(testTypeId, profileId, "CLAMP_Temperature", profile.Temperature);
        }

        private void InsertConditions(BaseTestParametersAndNormatives baseTestParametersAndNormatives, long testTypeId, long profileId)
        {
            switch (baseTestParametersAndNormatives.TestParametersType)
            {
                case TestParametersType.Gate:
                    InsertGateConditions(baseTestParametersAndNormatives as TestParameters, testTypeId, profileId);
                    break;
                case TestParametersType.StaticLoses:
                    InsertSlConditions(baseTestParametersAndNormatives as Types.SL.TestParameters, testTypeId, profileId);
                    break;
                case TestParametersType.Bvt:
                    InsertBvtConditions(baseTestParametersAndNormatives as Types.BVT.TestParameters, testTypeId, profileId);
                    break;
                case TestParametersType.Dvdt:
                    InsertDvdtConditions(baseTestParametersAndNormatives as Types.dVdt.TestParameters, testTypeId, profileId);
                    break;
            }
        }


        private void InsertGateConditions(TestParameters profile, long testTypeId, long profileId)
        {
            InsertCondition(testTypeId, profileId, "Gate_En", profile.IsEnabled);
            InsertCondition(testTypeId, profileId, "Gate_EnableCurrent", profile.IsCurrentEnabled);
            InsertCondition(testTypeId, profileId, "Gate_IHEn", profile.IsIhEnabled);
            InsertCondition(testTypeId, profileId, "Gate_ILEn", profile.IsIlEnabled);
            InsertCondition(testTypeId, profileId, "Gate_EnableIHStrike", profile.IsIhStrikeCurrentEnabled);

        }

        private void InsertSlConditions(Types.SL.TestParameters profile, long testTypeId, long profileId)
        {
            InsertCondition(testTypeId, profileId, "SL_En", profile.IsEnabled);
            InsertCondition(testTypeId, profileId, "SL_Type", profile.TestType);
            InsertCondition(testTypeId, profileId, "SL_FS", profile.UseFullScale);
            InsertCondition(testTypeId, profileId, "SL_N", profile.Count);


            switch (profile.TestType)
            {
                case VTMTestType.Ramp:
                    InsertCondition(testTypeId, profileId, "SL_ITM", profile.RampCurrent);
                    InsertCondition(testTypeId, profileId, "SL_Time", profile.RampTime);
                    InsertCondition(testTypeId, profileId, "SL_OpenEn", profile.IsRampOpeningEnabled);
                    InsertCondition(testTypeId, profileId, "SL_OpenI", profile.RampOpeningCurrent);
                    InsertCondition(testTypeId, profileId, "SL_TimeEx", profile.RampOpeningTime);
                    break;
                case VTMTestType.Sinus:
                    InsertCondition(testTypeId, profileId, "SL_ITM", profile.SinusCurrent);
                    InsertCondition(testTypeId, profileId, "SL_Time", profile.SinusTime);
                    break;
                case VTMTestType.Curve:
                    InsertCondition(testTypeId, profileId, "SL_ITM", profile.CurveCurrent);
                    InsertCondition(testTypeId, profileId, "SL_Time", profile.CurveTime);
                    InsertCondition(testTypeId, profileId, "SL_Factor", profile.CurveFactor);
                    InsertCondition(testTypeId, profileId, "SL_TimeEx", profile.CurveAddTime);
                    break;
            }

        }

        private void InsertBvtConditions(Types.BVT.TestParameters profile, long testTypeId, long profileId)
        {
            InsertCondition(testTypeId, profileId, "BVT_En", profile.IsEnabled);
            InsertCondition(testTypeId, profileId, "BVT_Type", profile.TestType);
            InsertCondition(testTypeId, profileId, "BVT_I", profile.CurrentLimit);
            InsertCondition(testTypeId, profileId, "BVT_RumpUp", profile.RampUpVoltage);
            InsertCondition(testTypeId, profileId, "BVT_StartV", profile.StartVoltage);
            InsertCondition(testTypeId, profileId, "BVT_F", profile.VoltageFrequency);
            InsertCondition(testTypeId, profileId, "BVT_FD", profile.FrequencyDivisor);
            InsertCondition(testTypeId, profileId, "BVT_Mode", profile.MeasurementMode);

            switch (profile.TestType)
            {
                case BVTTestType.Both:
                    InsertCondition(testTypeId, profileId, "BVT_VD", profile.VoltageLimitD);
                    InsertCondition(testTypeId, profileId, "BVT_VR", profile.VoltageLimitR);
                    break;
                case BVTTestType.Direct:
                    InsertCondition(testTypeId, profileId, "BVT_VD", profile.VoltageLimitD);
                    break;
                case BVTTestType.Reverse:
                    InsertCondition(testTypeId, profileId, "BVT_VR", profile.VoltageLimitR);
                    break;
            }

        }

        private void InsertDvdtConditions(Types.dVdt.TestParameters testParameters, long testTypeId, long profileId)
        {
            InsertCondition(testTypeId, profileId, "DVDT_En", testParameters.IsEnabled);
            InsertCondition(testTypeId, profileId, "DVDT_Mode", testParameters.Mode);
            switch (testParameters.Mode)
            {
                case DvdtMode.Confirmation:
                    InsertCondition(testTypeId, profileId, "DVDT_Voltage", testParameters.Voltage);
                    InsertCondition(testTypeId, profileId, "DVDT_VoltageRate", testParameters.VoltageRate);
                    break;
                case DvdtMode.Detection:
                    InsertCondition(testTypeId, profileId, "DVDT_Voltage", testParameters.Voltage);
                    break;
            }
        }

        private void InsertCondition(long testTypeId, long profileId, string name, object value)
        {
            var profCondInsertCommand = new SQLiteCommand("INSERT INTO PROF_COND(PROF_TESTTYPE_ID, PROF_ID, COND_ID, VALUE) VALUES(@PROF_TESTTYPE_ID, @PROF_ID, @COND_ID, @VALUE)", _connection);
            profCondInsertCommand.Parameters.Add("@PROF_TESTTYPE_ID", DbType.Int64);
            profCondInsertCommand.Parameters.Add("@PROF_ID", DbType.Int64);
            profCondInsertCommand.Parameters.Add("@COND_ID", DbType.Int64);
            profCondInsertCommand.Parameters.Add("@VALUE", DbType.AnsiStringFixedLength);
            profCondInsertCommand.Prepare();

            profCondInsertCommand.Parameters["@PROF_TESTTYPE_ID"].Value = testTypeId;
            profCondInsertCommand.Parameters["@PROF_ID"].Value = profileId;
            profCondInsertCommand.Parameters["@COND_ID"].Value = _conditions[name];
            profCondInsertCommand.Parameters["@VALUE"].Value = value.ToString();
            profCondInsertCommand.ExecuteNonQuery();
        }


        #endregion

        #region ParametersHelpers


        private void InsertParameters(BaseTestParametersAndNormatives baseTestParametersAndNormatives, long testTypeId, long profileId)
        {
            switch (baseTestParametersAndNormatives.TestParametersType)
            {
                case TestParametersType.Gate:
                    InsertGateParameters(baseTestParametersAndNormatives as TestParameters, testTypeId, profileId);
                    break;
                case TestParametersType.StaticLoses:
                    InsertVtmParameters(baseTestParametersAndNormatives as Types.SL.TestParameters, testTypeId, profileId);
                    break;
                case TestParametersType.Bvt:
                    InsertBvtParameters(baseTestParametersAndNormatives as Types.BVT.TestParameters, testTypeId, profileId);
                    break;
            }



        }

        private void InsertGateParameters(TestParameters gateTestParameters, long testTypeId, long profileId)
        {

            InsertParameter(testTypeId, profileId, "RG", DBNull.Value, gateTestParameters.Resistance);
            InsertParameter(testTypeId, profileId, "IGT", DBNull.Value, gateTestParameters.IGT);
            InsertParameter(testTypeId, profileId, "VGT", DBNull.Value, gateTestParameters.VGT);

            if (gateTestParameters.IsIhEnabled)
                InsertParameter(testTypeId, profileId, "IH", DBNull.Value, gateTestParameters.IH);
            if (gateTestParameters.IsIlEnabled)
                InsertParameter(testTypeId, profileId, "IL", DBNull.Value, gateTestParameters.IL);

        }

        private void InsertVtmParameters(Types.SL.TestParameters vtmTestParameters, long testTypeId, long profileId)
        {
            InsertParameter(testTypeId, profileId, "VTM", DBNull.Value, vtmTestParameters.VTM);
        }

        private void InsertBvtParameters(Types.BVT.TestParameters bvtTestParameters, long testTypeId, long profileId)
        {
            InsertParameter(testTypeId, profileId, "VRRM", bvtTestParameters.VRRM, DBNull.Value);
            InsertParameter(testTypeId, profileId, "IRRM", DBNull.Value, bvtTestParameters.IRRM);
            InsertParameter(testTypeId, profileId, "VDRM", bvtTestParameters.VDRM, DBNull.Value);
            InsertParameter(testTypeId, profileId, "IDRM", DBNull.Value, bvtTestParameters.IDRM);
        }

        private void InsertParameter(long testTypeId, long profileId, string name, object min, object max)
        {
            var paramInsertCommand = new SQLiteCommand("INSERT INTO PROF_PARAM(PROF_TESTTYPE_ID, PROF_ID, PARAM_ID, MIN_VAL, MAX_VAL) VALUES(@PROF_TESTTYPE_ID, @PROF_ID, @PARAM_ID, @MIN_VAL, @MAX_VAL)", _connection);
            paramInsertCommand.Parameters.Add("@PROF_TESTTYPE_ID", DbType.Int64);
            paramInsertCommand.Parameters.Add("@PROF_ID", DbType.Int64);
            paramInsertCommand.Parameters.Add("@PARAM_ID", DbType.Int64);
            paramInsertCommand.Parameters.Add("@MIN_VAL", DbType.Single);
            paramInsertCommand.Parameters.Add("@MAX_VAL", DbType.Single);
            paramInsertCommand.Prepare();

            paramInsertCommand.Parameters["@PROF_TESTTYPE_ID"].Value = testTypeId;
            paramInsertCommand.Parameters["@PROF_ID"].Value = profileId;
            paramInsertCommand.Parameters["@PARAM_ID"].Value = _params[name];
            paramInsertCommand.Parameters["@MIN_VAL"].Value = min;
            paramInsertCommand.Parameters["@MAX_VAL"].Value = max;
            paramInsertCommand.ExecuteNonQuery();
        }

        #endregion
    }
}
