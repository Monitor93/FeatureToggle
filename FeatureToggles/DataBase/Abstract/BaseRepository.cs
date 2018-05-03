﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using FeatureToggle.Config;
using System.Linq;
using FeatureToggle.Attributes;
using System.Reflection;

namespace FeatureToggle.DataBase.Abstract
{
    abstract class BaseRepository<T, TKey> : IDisposable where T : DbObject<TKey>, new()
    {

        protected readonly string TableName = typeof(T).Name;
        protected readonly Type Type = typeof(T);

        protected readonly SqlConnection SqlConnection;

        protected BaseRepository()
        {
            SqlConnection = new SqlConnection(FeatureToggleConfiguration.ConnectionString);
            ExecuteNonQuery(CheckAndCreateTableSqlCommandSql());
        }

        public virtual void Save(params T[] obj)
        {
            if (obj.Any())
            {
                ExecuteNonQuery(InsertOrUpdateSqlCommand(ConvertToDictionary(obj)));
            }
        }

        public virtual T Get(TKey key)
        {
            return ExecuteQuery(SelectByIdSqlCommand(key)).FirstOrDefault();
        }

        public virtual List<T> GetAll()
        {
            return ExecuteQuery(SelectAllSqlCommand());
        }

        public virtual void Delete(TKey key)
        {
            ExecuteNonQuery(DeleteByIdSqlCommand(key));
        }

        protected void ExecuteNonQuery(SqlCommand command)
        {
            Debug.WriteLine($"FeatureToogle SqlNonQuery by '{GetType().Name}'\n{command.CommandText}");
            lock (SqlConnection)
            {
                SqlConnection.Open();
                using (command)
                {
                    command.ExecuteNonQuery();
                }
                SqlConnection.Close();
            }
        }

        protected List<T> ExecuteQuery(SqlCommand command)
        {
            Debug.WriteLine($"FeatureToogle SqlQuery by '{GetType().Name}'\n{command.CommandText}");
            lock (SqlConnection)
            {
                SqlConnection.Open();
                SqlDataReader executeResult;
                using (command)
                {
                    executeResult = command.ExecuteReader();
                }
                var objects = new List<T>();
                var type = typeof(T);
                var properties = type.GetProperties();
                while (executeResult.Read())
                {
                    var readedObj = new T();
                    for (var i = 0; i < executeResult.FieldCount; i++)
                    {
                        var prop = properties.FirstOrDefault(p => p.Name == executeResult.GetName(i));
                        if (prop != null)
                        {
                            prop.SetValue(readedObj, executeResult[i]);
                        }
                    }
                    objects.Add(readedObj);
                }
                executeResult.Close();
                SqlConnection.Close();

                return objects;
            }
        }

        protected string ToSqlValue(object x)
        {
            var xType = x.GetType();
            var typesUseQuotes = new[]
            {
                typeof(string),
                typeof(Guid),
                typeof(DateTime),
                typeof(TimeSpan),
            };
            return typesUseQuotes.Contains(xType)
                ? "'" + x.ToString() + "'"
                : xType == typeof(bool)
                    ? Convert.ToInt32(x).ToString()
                    : x.ToString();
        }

        protected SqlCommand InsertOrUpdateSqlCommand(Dictionary<string, IEnumerable<object>> dicts) 
        {
            var valuesAsStr = new List<string>();
            foreach (var prop in PropertyWithoutIdentity())
            {
                dicts.Remove(prop.Name);
            }
            var valuesTransponete = dicts.Values
                .Select(x => x.ToArray())
                .ToList();

            var paramsAndValues = new Dictionary<string, object>();

            var postfix = 1;

            for (var i = 0; i < valuesTransponete.First().Length; i++)
            {
                var objToStr = valuesTransponete.Select(x => new {
                    Param = $"@Param_{postfix++}",
                    Value = x[i]
                }).ToList();

                valuesAsStr.Add($"({string.Join(",", objToStr.Select(x => x.Param))})");
                foreach (var obj in objToStr)
                {
                    paramsAndValues.Add(obj.Param, obj.Value);
                }
            }
            var fieds = string.Join(",", dicts.Keys);
            var sourceFields = string.Join(",", dicts.Keys.Select(k => $"Source.{k}"));
            var updateFromSource = string.Join(",", dicts.Keys.Select(k => $"{k} = Source.{k}"));
            var insertSql = $"insert ({fieds}) VALUES ({sourceFields})";
            var updateSql = $"UPDATE SET {updateFromSource}";

            var resultSql = $"MERGE {TableName} AS Target USING (VALUES {string.Join(",", valuesAsStr)}) AS Source ({fieds}) ON {MergeCondition()} WHEN NOT MATCHED THEN {insertSql} WHEN MATCHED THEN {updateSql};";
           
            var command = new SqlCommand(resultSql, SqlConnection);

            foreach (var pair in paramsAndValues)
            {
                command.Parameters.AddWithValue(pair.Key, pair.Value);
            }

            return command;
        }

        protected virtual string MergeCondition()
        {
            return "Target.Id = Source.Id";
        }

        protected SqlCommand SelectAllSqlCommand()
        {
            return new SqlCommand($"select * from {TableName}", SqlConnection);
        }

        protected SqlCommand SelectByIdSqlCommand(TKey id)
        {
            var command = new SqlCommand($"select * from {TableName} where Id = @Id", SqlConnection);
            command.Parameters.AddWithValue("@Id", id);
            return command;
        }

        protected SqlCommand DeleteByIdSqlCommand(TKey id)
        {
            var command = new SqlCommand($"delete from {TableName} where Id = @Id", SqlConnection);
            command.Parameters.AddWithValue("@Id", id);
            return command;
        }

        protected Dictionary<string, IEnumerable<object>> ConvertToDictionary(params T[] obj)
        {
            return Type.GetProperties()
                .ToDictionary(prop => prop.Name, prop => obj.Select(prop.GetValue));
        }

        protected IEnumerable<PropertyInfo> PropertyWithoutIdentity()
        {
            var props = Type.GetProperties();
            return props
                .Where(p => p.GetCustomAttribute(typeof(IdentityAttribute)) != null);
        }

        private SqlCommand CheckAndCreateTableSqlCommandSql()
        {
            var type = typeof(T);
            var cSharpToSqlTypes = new Dictionary<Type, string>
            {
                [typeof(int)] = "int not null",
                [typeof(long)] = "bigint not null",
                [typeof(float)] = "real not null",
                [typeof(double)] = "float not null",
                [typeof(decimal)] = "money not null",
                [typeof(bool)] = "bit not null",
                [typeof(DateTime)] = "datetime not null",
                [typeof(TimeSpan)] = "time not null",

                [typeof(string)] = $"varchar({FeatureToggleConfiguration.VarcharSize}) not null",

                [typeof(int?)] = "int null",
                [typeof(long?)] = "bigint null",
                [typeof(float?)] = "real null",
                [typeof(double?)] = "float null",
                [typeof(decimal?)] = "money null",
                [typeof(bool?)] = "bit null",
                [typeof(DateTime?)] = "datetime null",
                [typeof(TimeSpan?)] = "time null",
            };
            var fieldsStr = type.GetProperties()
                .Select(prop => $"{prop.Name} {cSharpToSqlTypes[prop.PropertyType]}{(prop.GetCustomAttribute<IdentityAttribute>(true) != null ? " IDENTITY(1,1)" : string.Empty)}");
            var sysSelect = $"select * from sysobjects where name='{type.Name}' and xtype='U'";
            var refFields = type.GetProperties()
                .Select(p => new
                {
                    Property = p,
                    Reference = (p.GetCustomAttribute<ForeignKeyAttribute>())?.ReferenceType
                })
                .Where(pair => pair.Reference != null);
            var constraints = refFields.Select(pair => $"FOREIGN KEY ({pair.Property.Name}) REFERENCES {pair.Reference.Name}(Id)").ToList();
            constraints.Insert(0, $"PRIMARY KEY (Id)");
            var sqlQuery = $@"if not exists ({sysSelect}) create table {type.Name}({string.Join(",", fieldsStr)}, {string.Join(",", constraints)})";
            return new SqlCommand(sqlQuery, SqlConnection);
        }

        public void Dispose()
        {
            SqlConnection.Dispose();
        }
    }
}
