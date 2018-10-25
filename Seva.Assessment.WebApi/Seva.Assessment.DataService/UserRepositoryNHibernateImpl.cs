using FluentNHibernate.Cfg;
using Seva.Assessment.DataService.User;
using Seva.Assessment.FluentNHibernetImpl;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Seva.Assessment.DataService
{
    public class UserRepositoryNHibernateImpl
    {
        public IList<UserData> GetUsers(string name)
        {
            return ErrorHandler.Handle(() =>
            {
                using (var context = DataFactory.GetInstance<IDataContext<UserData>>())
                {
                    return context.Data.FindAll(x => x.FirstName.ToLower().Contains(name.ToLower()) || x.LastName.ToLower().Contains(name.ToLower())).ToList();
                }
            });
        }

        public IList<UserData> GetUsers()
        {
            return ErrorHandler.Handle(() =>
           {
               using (var context = DataFactory.GetInstance<IDataContext<UserData>>())
               {
                   return context.Data.FindAll().ToList();
               }
           });
        }

        public bool AddUser(UserData user)
        {
            return ErrorHandler.Handle(() =>
            {
                using (var context = DataFactory.GetInstance<IDataContext<UserData>>())
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
            });
            
        }

        public bool RemoveUser(int id)
        {
            return ErrorHandler.Handle( ()=>
            {
                using (var context = DataFactory.GetInstance<IDataContext<UserData>>())
                {
                    var user = context.Data.FindOne(x => x.Id == id);
                    context.Data.Remove(user);
                    return true;
                }
            });            
            
        }
    }
}
