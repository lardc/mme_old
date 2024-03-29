﻿using System;
using System.Collections.Generic;
using System.Data.Common;

namespace SCME.InterfaceImplementations
{
    public class MigratorInserterTemplate<T, TDbParametr> : MigratorInserter  where T : class where TDbParametr : DbParameterCollection 
    {
        public ICollection<T> DataSet { get; set; }
        public Action<TDbParametr, TDbParametr> AddParameters { get; set; }
        public Action<TDbParametr, T> SetCountParametersValue { get; set; }
        public Action<TDbParametr, T> SetInsertParametersValue { get; set; }
        public override void Migrate(DbConnection _mConnection)
        {
            DbTransaction trans = null;
            try
            {
                trans = _mConnection.BeginTransaction();
                using (var insertCmd = _mConnection.CreateCommand())
                {
                    using (var countCmd = _mConnection.CreateCommand())
                    {

                        countCmd.CommandText = $@"SELECT COUNT (*) FROM {TableName} WHERE {ColumnWhereName} = @WHERE_PARAMETR";
                        countCmd.Transaction = trans;

                        insertCmd.CommandText = InsertCommand;
                        insertCmd.Transaction = trans;

                        AddParameters(countCmd.Parameters as TDbParametr, insertCmd.Parameters as TDbParametr);
                        countCmd.Prepare();
                        insertCmd.Prepare();

                        foreach (var i in DataSet)
                        {
                            SetCountParametersValue(countCmd.Parameters as TDbParametr, i);
                            if (Convert.ToInt32(countCmd.ExecuteScalar()) != 0)
                                continue;
                            SetInsertParametersValue(insertCmd.Parameters as TDbParametr, i);
                            insertCmd.ExecuteNonQuery();
                        }
                    }
                }
                trans.Commit();
            }
            catch
            {
                trans?.Rollback();
                throw;
            }
        }

        public MigratorInserterTemplate(string tableName, string columnWhereName, string insertCommand,
            ICollection<T> dataSet,
            Action<TDbParametr, TDbParametr> addParameters,
            Action<TDbParametr, T> setCountParametersValue,
            Action<TDbParametr, T> setInsertParametersValue) : base(tableName, columnWhereName, insertCommand)
        {
            DataSet = dataSet;
            AddParameters = addParameters;
            SetCountParametersValue = setCountParametersValue;
            SetInsertParametersValue = setInsertParametersValue;
        }

        public static void MigrateStatic()
        {

        }
    }
}
