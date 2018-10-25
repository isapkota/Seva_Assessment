using System;
using Xunit;
using Seva.Assessment.DataEntityFramework;
using System.Collections.Generic;
using System.Data;

namespace Seva.Assessment.DataEntityFramework.Tests
{     
    public class DatabaseRepositoryTests
    {
        private string _connectionString = @"Data Source=.;Initial Catalog=Seva_Assessment;Persist Security Info=True;User ID=sa;Password=CaFt@#85647";

        public DatabaseRepositoryTests()
        {
            DatabaseRepository repo = new DatabaseRepository(new System.Data.SqlClient.SqlConnection(_connectionString));
            repo.ExecuteNonQuery("IF EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLES a WHERE a.TABLE_NAME='TestUser') DROP TABLE TestUser;CREATE TABLE TestUser(Id INT IDENTITY(1,1) PRIMARY KEY,LoginUser NVARCHAR(100),[Password] NVARCHAR(100));");
        }

        [Fact]
        public void ExecuteInsertTest()
        {
            DatabaseRepository repo = new DatabaseRepository(new System.Data.SqlClient.SqlConnection(_connectionString));
            
            var obj = repo.ExecuteInsert("INSERT INTO TestUser(LoginUser,Password) VALUES(@Login,@Password);", new Dictionary<string, object>() { { "Login", "Ishwor" }, { "Password", "password" } });
            Assert.Equal(1, obj);
        }

        [Fact]
        public void ExecuteScalarTest()
        {
            DatabaseRepository repo = new DatabaseRepository(new System.Data.SqlClient.SqlConnection(_connectionString));
            var obj = repo.ExecuteScalar("SELECT GETDATE() AS TodaysDate;");
            DateTime dt = Convert.ToDateTime(obj);
            DateTime today = DateTime.Today;
            Assert.Equal(dt.Date, today.Date);
        }

        [Fact]
        public void AddEntityTest()
        {
            DatabaseRepository repo = new DatabaseRepository(new System.Data.SqlClient.SqlConnection(_connectionString));
            TestUser test = new TestUser()
            {
                LoginUser = "Bhagwan",
                Password = "password"
            };
            var res = repo.AddEntity(test, true);
            Assert.Equal(1, res);

        }

        [Fact]
        public void ExecuteEntityTest()
        {
            DatabaseRepository repo = new DatabaseRepository(new System.Data.SqlClient.SqlConnection(_connectionString));
            TestUser test = new TestUser()
            {
                LoginUser = "Bhagwan",
                Password = "password"
            };
            var res = repo.AddEntity(test, true);
            var result = repo.ExecuteEntity<TestUser>("SELECT * FROM TestUser");
            Assert.Single(result);
        }
    }

    public partial class TestUser 
    {
        public int Id { get; set; }
        public string LoginUser { get; set; }
        public string Password { get; set; }
    }

    public partial class TestUser : ISqlDataEntity
    {
        public Dictionary<EntityCommandType, string> SqlStatements { get
            {
                var statements = new Dictionary<EntityCommandType, string>();
                statements.Add(EntityCommandType.INSERT, "INSERT INTO TestUser(LoginUser,Password) VALUES(@LoginUser,@Password)");
                statements.Add(EntityCommandType.UPDATE, "UPDATE TestUser SET LoginUser = @LoginUser, Password=@Password WHERE Id = @Id");
                statements.Add(EntityCommandType.DELETE, "DELETE FROM TestUser WHERE Id = @Id");
                statements.Add(EntityCommandType.SELECTALL, "SELECT Id,LoginUser,Password FROM TestUser");
                statements.Add(EntityCommandType.SELECT, "SELECT Id,LoginUser,Password FROM TestUser WHERE Id = @Id");
                return statements;

            } set => throw new NotImplementedException();
        }

        #region DataParameters 
        public Dictionary<string, object> DeleteParameters()
        {
            return new Dictionary<string, object>()
            {
                {"Id", Id }
            };
        }

        public Dictionary<string, object> InsertParameters()
        {
            return new Dictionary<string, object>()
            {
                {"LoginUser", LoginUser },
                {"Password",Password }
            };

        }

        public Dictionary<string, object> UpdateParameters()
        {
            return new Dictionary<string, object>()
            {
                {"Id", Id },
                {"LoginUser", LoginUser },
                {"Password",Password }
            };

        }
        #endregion

        #region Map Data
        public void MapReader(IDataRecord record)
        {
            Id = Convert.ToInt32(record["Id"]);
            LoginUser = record["LoginUser"].ToString();
            Password = record["Password"].ToString();
        }
        #endregion



    }
}
