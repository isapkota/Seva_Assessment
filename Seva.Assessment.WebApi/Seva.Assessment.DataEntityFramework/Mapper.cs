using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace Seva.Assessment.DataEntityFramework
{
    public class Mapper<T> : ISqlDataEntity where T : SqlDataEntity, new()
    {

        private readonly T _t;
        private Dictionary<EntityCommandType, string> _sqlStatements = null;
        public Mapper(T t)
        {
            this._t = t;
        }

        public string TableName { get { return typeof(T).Name; } }
        private string _updateColumnValueString { get; set; }
        public List<string> Columns
        {
            get
            {
                var properties = typeof(T).GetProperties(BindingFlags.Public);
                List<string> propNames = new List<string>();
                _updateColumnValueString = "";
                foreach (var prop in properties)
                {
                    propNames.Add(prop.Name);
                    _updateColumnValueString = _updateColumnValueString!=string.Empty? (_updateColumnValueString + ",") : "" + $"{prop.Name}=@{prop.Name}";
                }
                return propNames;
            }
        }

        private string columnsString { get { return string.Join(',', Columns.ToArray()); } }
        private string valuesString { get { return "@" + string.Join(",@", Columns.ToArray()); } }
        private string criteriaString
        {
            get
            {
                var returnStr = "";
                foreach(var col in _t.CriteriaColumns)
                {
                    returnStr= returnStr != string.Empty ? (returnStr + " AND ") : "" + $"{col}=@{col}";
                }
                return returnStr;
            }
        }

        public Dictionary<EntityCommandType, string> SqlStatements
        {
            get
            {
                if (_sqlStatements == null)
                {
                    var statements = new Dictionary<EntityCommandType, string>();
                    statements.Add(EntityCommandType.INSERT, $"INSERT INTO {TableName}({columnsString}) VALUES ({valuesString})");
                    statements.Add(EntityCommandType.UPDATE, $"UPDATE {TableName} SET {_updateColumnValueString} WHERE 1=1 AND {criteriaString} ");
                    statements.Add(EntityCommandType.DELETE, $"DELETE FROM {TableName} WHERE 1=1 AND {criteriaString}");
                    statements.Add(EntityCommandType.SELECTALL, $"SELECT {columnsString} FROM {TableName}");
                    statements.Add(EntityCommandType.SELECT, $"SELECT {columnsString} FROM {TableName} WHERE 1=1 AND {criteriaString}");
                    _sqlStatements = statements;
                }
                return _sqlStatements;
            }
            set { _sqlStatements = value; }
        }

        public Mapper() : this(new T())
        {

        }

        public void MapReader(IDataRecord record)
        {
            foreach (var propertyInfo in typeof(T).GetProperties(BindingFlags.Public))
            {
                var colName = propertyInfo.Name;
                propertyInfo.SetValue(_t, Convert.ChangeType(record[colName], propertyInfo.GetType()), null);
            }
        }
        public void MapReader(DataRow row)
        {
            foreach (var propertyInfo in typeof(T).GetProperties(BindingFlags.Public))
            {
                var colName = propertyInfo.Name;
                propertyInfo.SetValue(_t, Convert.ChangeType(row[colName], propertyInfo.GetType()), null);
            }
        }
        public Dictionary<string, object> InsertParameters()
        {
            Dictionary<string, object> parameters = null;
            foreach (var propertyInfo in typeof(T).GetProperties(BindingFlags.Public))
            {
                var colName = propertyInfo.Name;
                if (parameters == null)
                    parameters = new Dictionary<string, object>();
                parameters.Add("@" + colName, propertyInfo.GetValue(_t, null));
            }
            return parameters;
        }

        public Dictionary<string, object> UpdateParameters()
        {
            Dictionary<string, object> parameters = null;
            foreach (var propertyInfo in typeof(T).GetProperties(BindingFlags.Public))
            {
                var colName = propertyInfo.Name;
                if (parameters == null)
                    parameters = new Dictionary<string, object>();
                parameters.Add("@" + colName, propertyInfo.GetValue(_t, null));
            }
            return parameters;
        }

        public Dictionary<string, object> DeleteParameters()
        {
            Dictionary<string, object> parameters = null;
            foreach (var propertyInfo in typeof(T).GetProperties(BindingFlags.Public))
            {
                var colName = propertyInfo.Name;
                if (_t.CriteriaColumns.Contains(colName))
                {
                    if (parameters == null)
                        parameters = new Dictionary<string, object>();
                    parameters.Add("@" + colName, propertyInfo.GetValue(_t, null));
                }
            }
            return parameters;
        }
    }
}
