using FluentNHibernate.Cfg;
using FluentNHibernate.Mapping;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Xunit;

namespace Seva.Assessment.FluentNHibernetImpl.Tests
{
    public class DataContextIntegrationTest : IDisposable
    {
        NHibernate.ISessionFactory _sessionFactory = null;
         
        public DataContextIntegrationTest()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            var connectionString = config.GetConnectionString("DefaultConnection");
            _sessionFactory = Fluently.Configure()
                .Database(FluentNHibernate.Cfg.Db.MsSqlConfiguration.MsSql2012
                    .ConnectionString(connectionString)
                    .ShowSql()
                )
                .ExposeConfiguration(x => { x.SetInterceptor(new SqlStatementInterceptor()); })
                .Mappings(m => m.FluentMappings
                    .AddFromAssemblyOf<TestUser>()
                )
                .BuildSessionFactory();

            using (DataContext<TestUser> context = new DataContext<TestUser>(_sessionFactory))
            {
                var query = context.CurrentSession.CreateSQLQuery("IF EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLES a WHERE a.TABLE_NAME='TestUser') DROP TABLE TestUser;CREATE TABLE TestUser(Id INT IDENTITY(1,1) PRIMARY KEY,LoginUser NVARCHAR(100),[Password] NVARCHAR(100));");
                query.UniqueResult();                
            }
        }

        [Fact]
        public void DataContext_Data_AddOne_Verify()
        {
            using (DataContext<TestUser> context = new DataContext<TestUser>(_sessionFactory))
            {
                context.Data.Add(new TestUser()
                {
                    LoginUser = "Ishwor",
                    Password="password"
                });
            }

            //verifying added data
            IQueryable<TestUser> users = null;
            using (DataContext<TestUser> context = new DataContext<TestUser>(_sessionFactory))
            {
                users = context.Data.FindAll();
                Assert.NotEmpty(users);
                Assert.Equal(1, users.Count());
            }
            
        }

        [Fact]
        public void DataContext_GetAllData_Added_Recently()
        {
            using (DataContext<TestUser> context = new DataContext<TestUser>(_sessionFactory))
            {
                context.Data.Add(new TestUser()
                {
                    LoginUser = "Ishwor0",
                    Password = "password0"
                });
                context.Data.Add(new TestUser()
                {
                    LoginUser = "Ishwor1",
                    Password = "password"
                });
                context.Data.Add(new TestUser()
                {
                    LoginUser = "Ishwor2",
                    Password = "password"
                });
            }
            //list test
            IList<TestUser> users = null;
            using (DataContext<TestUser> context = new DataContext<TestUser>(_sessionFactory))
            {
                users = context.Data.FindAll().ToList();
            }

            Assert.NotEmpty(users);
            
            //singletest 
            TestUser selectiveUser = null;

            using (DataContext<TestUser> context = new DataContext<TestUser>(_sessionFactory))
            {
                selectiveUser = context.Data.FindOne(x => x.LoginUser == "Ishwor2");
            }

            Assert.NotNull(selectiveUser);
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
                using (DataContext<TestUser> context = new DataContext<TestUser>(_sessionFactory))
                {
                    if (context.CurrentSession.IsOpen)
                    {
                        var query = context.CurrentSession.CreateSQLQuery("IF EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLES a WHERE a.TABLE_NAME='TestUser') DROP TABLE TestUser;");
                        query.UniqueResult();
                    }
                }
            }
        }


        ~DataContextIntegrationTest()
        {
            Dispose(false);
        }
    }

    public class TestUser : Entity
    {
        public virtual string LoginUser { get; set; }
        public virtual string Password { get; set; }
    }

    public class TestUserMap : ClassMap<TestUser>
    {
        public TestUserMap()
        {
            Table("TestUser");
            Id(x => x.Id);
            Map(x => x.LoginUser).Column("LoginUser");
            Map(x => x.Password).Column("Password");
        }
    }
}
