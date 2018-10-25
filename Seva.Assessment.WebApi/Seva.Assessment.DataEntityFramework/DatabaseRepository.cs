using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Transactions;

namespace Seva.Assessment.DataEntityFramework
{
    public interface IDatabaseRepository
    {
        int AddEntity<T>(T entity, bool withLastInsertID = false) where T : class, ISqlDataEntity, new();
        SqlCommand BuildCommand(string query, Dictionary<string, object> parameters = null);
        SqlCommand BuildCommand(string query, bool IsProc, Dictionary<string, object> parameters = null);
        void CloseConnection();
        int DeleteEntity<T>(T entity) where T : class, ISqlDataEntity, new();
        List<T> ExecuteEntity<T>(Dictionary<string, object> parameters = null) where T : class, ISqlDataEntity, new();
        List<T> ExecuteEntity<T>(string query, Dictionary<string, object> parameters = null) where T : ISqlDataEntity, new();
        List<T> ExecuteEntity<T>(string query, bool isProc, Dictionary<string, object> parameters = null) where T : ISqlDataEntity, new();
        int ExecuteFile(string filePath);
        int ExecuteInsert(string query, Dictionary<string, object> parameters = null);
        int ExecuteInsert(string query, bool IsProc, Dictionary<string, object> parameters = null);
        bool ExecuteMultiple(params SqlCommand[] commands);
        int ExecuteNonQuery(string query, Dictionary<string, object> parameters = null);
        int ExecuteNonQuery(string query, bool isProc, Dictionary<string, object> parameters = null);
        IDataReader ExecuteReader(string query, Dictionary<string, object> parameters = null);
        IDataReader ExecuteReader(string query, bool IsProc, Dictionary<string, object> parameters = null);
        object ExecuteScalar(string query, Dictionary<string, object> parameters = null);
        object ExecuteScalar(string query, bool IsProc, Dictionary<string, object> parameters = null);
        T ExecuteSingleEntity<T>(Dictionary<string, object> parameters = null) where T : class, ISqlDataEntity, new();
        T ExecuteSingleEntity<T>(string query, Dictionary<string, object> parameters = null) where T : class, ISqlDataEntity, new();
        T ExecuteSingleEntity<T>(string query, bool IsProc, Dictionary<string, object> parameters = null) where T : class, ISqlDataEntity, new();
        bool HasTableInDB(string tableName);
        void OpenConnection();
        int UpdateEntity<T>(T entity) where T : class, ISqlDataEntity, new();
    }

    public class DatabaseRepository : IDatabaseRepository, IDisposable
    {
        private SqlTransaction _transaction = null;
        private SqlConnection _connection = null;        


        public DatabaseRepository(SqlConnection connection)
        {
            _connection = connection;
        }

        public void OpenConnection()
        {
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }

        }

        public void CloseConnection()
        {
            if (_transaction == null)
                _connection.Close();
        }

        public bool ExecuteMultiple(params SqlCommand[] commands)
        {
            bool result = false;
            _transaction = _connection.BeginTransaction();
            _connection.Open();
            try
            {
                foreach (SqlCommand sqlCommand in commands)
                {
                    sqlCommand.Connection = _connection;
                    sqlCommand.Transaction = _transaction;
                    sqlCommand.ExecuteNonQuery();
                }
                _transaction.Commit();
                result = true;
            }
            catch (Exception)
            {
                _transaction.Rollback();
                throw;
            }
            finally
            {
                _connection.Close();
            }
            return result;
        }

        public SqlCommand BuildCommand(string query, Dictionary<string, object> parameters = null)
        {
            return BuildCommand(query, false, parameters);
        }

        public SqlCommand BuildCommand(string query, bool IsProc, Dictionary<string, object> parameters = null)
        {
            var command = new SqlCommand(query, _connection);

            if (IsProc)
            {
                command.CommandType = CommandType.StoredProcedure;
            }
            else
            {
                command.CommandType = CommandType.Text;
            }


            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(new SqlParameter(parameter.Key, parameter.Value));
                }
            }
            return command;
        }

        public int ExecuteNonQuery(string query, Dictionary<string, object> parameters = null)
        {
            return ExecuteNonQuery(query, false, parameters);
        }

        public int ExecuteNonQuery(string query, bool isProc, Dictionary<string, object> parameters = null)
        {
            var command = BuildCommand(query, isProc, parameters);
            int result = 0;
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }
            result = command.ExecuteNonQuery();
            _connection.Close();
            return result;
        }

        public int ExecuteInsert(string query, Dictionary<string, object> parameters = null)
        {
            return ExecuteInsert(query, false, parameters);
        }

        public int ExecuteInsert(string query, bool IsProc, Dictionary<string, object> parameters = null)
        {
            var command = BuildCommand(query, IsProc, parameters);

            using (TransactionScope scope = new TransactionScope())
            {
                if (_connection.State != ConnectionState.Open)
                {
                    _connection.Open();
                }
                command.ExecuteNonQuery();
                var obj = ExecuteScalar("select @@IDENTITY;");
                scope.Complete();
                _connection.Close();
                return Convert.ToInt32(obj);
            }

        }

        public object ExecuteScalar(string query, Dictionary<string, object> parameters = null)
        {
            return ExecuteScalar(query, false, parameters);
        }
        public object ExecuteScalar(string query, bool IsProc, Dictionary<string, object> parameters = null)
        {
            var command = BuildCommand(query, IsProc, parameters);
            object result = 0;
            if (_connection.State != ConnectionState.Open)
                _connection.Open();
            result = command.ExecuteScalar();
            _connection.Close();
            return result;
        }

        public bool HasTableInDB(string tableName)
        {
            var obj = ExecuteScalar("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME=@name", new Dictionary<string, object>()
            {
                {"name",tableName}
            });

            if (obj != null && obj.ToString() == tableName)
            {
                return true;
            }

            return false;
        }

        public IDataReader ExecuteReader(string query, Dictionary<string, object> parameters = null)
        {
            return ExecuteReader(query, false, parameters);
        }

        public IDataReader ExecuteReader(string query, bool IsProc, Dictionary<string, object> parameters = null)
        {
            var command = BuildCommand(query, IsProc, parameters);
            _connection.Open();
            var result = command.ExecuteReader();
            return result;
        }

        public List<T> ExecuteEntity<T>(string query, bool isProc, Dictionary<string, object> parameters = null) where T : ISqlDataEntity, new()
        {
            var reader = ExecuteReader(query, isProc, parameters);
            var list = new List<T>();
            while (reader.Read())
            {
                var t = new T();
                t.MapReader(reader);
                list.Add(t);
            }
            _connection.Close();
            return list;
        }

        public List<T> ExecuteEntity<T>(string query, Dictionary<string, object> parameters = null) where T : ISqlDataEntity, new()
        {
            return ExecuteEntity<T>(query, false, parameters);
        }

        public List<T> ExecuteEntity<T>(Dictionary<string, object> parameters = null)
            where T : class, ISqlDataEntity, new()
        {
            var t = new T();
            var query = t.SqlStatements[EntityCommandType.SELECTALL];            
            return ExecuteEntity<T>(query, false, parameters);
        }

        public T ExecuteSingleEntity<T>(string query, Dictionary<string, object> parameters = null)
            where T : class, ISqlDataEntity, new()
        {
            return ExecuteSingleEntity<T>(query, false, parameters);
        }

        public T ExecuteSingleEntity<T>(string query, bool IsProc, Dictionary<string, object> parameters = null)
            where T : class, ISqlDataEntity, new()
        {
            var list = ExecuteEntity<T>(query, IsProc, parameters);
            if (list.Count > 0)
                return list[0];
            return null as T;


        }
        
        public T ExecuteSingleEntity<T>(Dictionary<string, object> parameters = null)
            where T : class, ISqlDataEntity, new()
        {
            var t = new T();
            var query = t.SqlStatements[EntityCommandType.SELECT];
            return ExecuteSingleEntity<T>(query, false, parameters);
        }

        public int AddEntity<T>(T entity, bool withLastInsertID = false) where T : class, ISqlDataEntity, new()
        {
            Dictionary<string, object> parameters = entity.InsertParameters();
            var query = entity.SqlStatements[EntityCommandType.INSERT];
            if (withLastInsertID)
            {
                return ExecuteInsert(query,false, parameters);
            }
            return ExecuteNonQuery(query, false, parameters);
        }

        public int UpdateEntity<T>(T entity) where T : class, ISqlDataEntity, new()
        {
            Dictionary<string, object> parameters = entity.UpdateParameters();
            var query = entity.SqlStatements[EntityCommandType.UPDATE];
            return ExecuteNonQuery(query, false, parameters);
        }

        public int DeleteEntity<T>(T entity) where T : class, ISqlDataEntity, new()
        {
            Dictionary<string, object> parameters = entity.DeleteParameters();
            
            var query = entity.SqlStatements[EntityCommandType.DELETE];
            return ExecuteNonQuery(query, false, parameters);
        }

        public int ExecuteFile(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            if (!fileInfo.Exists)
                throw new System.IO.FileNotFoundException("Script file doesnot exist.");

            StreamReader stream = fileInfo.OpenText();
            var fileContent = stream.ReadToEnd();
            var command = BuildCommand(fileContent);
            int result = 0;
            _connection.Open();
            result = command.ExecuteNonQuery();
            _connection.Close();
            return result;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_connection.State != ConnectionState.Closed)
                {
                    _connection.Close();
                    _connection = null;
                }
                    
            }
        }


        ~DatabaseRepository()
        {
            Dispose(false);
        }
    }

}
