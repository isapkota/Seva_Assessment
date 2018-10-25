using FluentNHibernate.Cfg;
using Seva.Assessment.DataService.User;
using Seva.Assessment.FluentNHibernetImpl;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace Seva.Assessment.DataService
{
    public class UserRepositoryNHibernateImpl
    {
        private NHibernate.ISessionFactory _sessionFactory = null;
        

        public UserRepositoryNHibernateImpl(string connectionString)
        {
            SetupSessionFactory(connectionString);
        }


        private void SetupSessionFactory(string connectionString)
        {
            _sessionFactory = Fluently.Configure()
                .Database(FluentNHibernate.Cfg.Db.MsSqlConfiguration.MsSql2012
                    .ConnectionString(connectionString)
                    .ShowSql()
                )
                .ExposeConfiguration(x => { x.SetInterceptor(new SqlStatementInterceptor()); })
                .Mappings(m => m.FluentMappings
                    .AddFromAssemblyOf<UserData>()
                )
                .BuildSessionFactory();
        }
        

        public IList<UserData> GetUsers(string name)
        {
            using(DataContext<UserData> context = new DataContext<UserData>(_sessionFactory))
            {
                return context.Data.FindAll(x => x.FirstName.Contains(name) || x.LastName.Contains(name));
            }
        }


        public IList<UserData> GetUsers()
        {
            using (DataContext<UserData> context = new DataContext<UserData>(_sessionFactory))
            {
                return context.Data.FindAll();
            }
        }

        public bool AddUsers(UserData user)
        {
            try
            {
                using (DataContext<UserData> context = new DataContext<UserData>(_sessionFactory))
                {
                    var newUserData = new UserData()
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        EmailAddress = user.EmailAddress
                    };
                    context.Data.Add(newUserData);
                    var interestList = new List<UserInterest>();
                    foreach (var interest in user.Interest)
                    {                        
                        interest.UserID = newUserData.Id;
                        interestList.Add(interest);
                    }
                    newUserData.Interest = interestList;
                    context.Data.Update(newUserData);
                    return true;
                }
            }
            catch(Exception ex)
            {
                //TODO : Exception Handling
                return false;
            }
            
        }

        public bool RemoveUser(int id)
        {
            try
            {
                using (DataContext<UserData> context = new DataContext<UserData>(_sessionFactory))
                {
                    var user = context.Data.FindOne(x => x.Id == id);
                    context.Data.Remove(user);
                    return true;
                }
            }
            catch(Exception ex)
            {
                //TODO: Exception Handling
                return false;
            }
            
            
        }
    }
}
