using Seva.Assessment.DataService.User;
using System;
using System.Collections.Generic;
using Xunit;

namespace Seva.Assessment.DataService.Test
{
    public class UserRepositoryNHibernateImplTest
    {
        private UserRepositoryNHibernateImpl _userRepo;
        public UserRepositoryNHibernateImplTest()
        {
            _userRepo = new Seva.Assessment.DataService.UserRepositoryNHibernateImpl();
        }

        private List<UserData> _dataForTest;

        private void prepareUserForTest()
        {
            _dataForTest = new List<UserData>();
            var data = new UserData() { FirstName = "Ishwor", LastName = "Sapkota", EmailAddress = "saathee.ishwor@gmail.com" };
            data.Interest = new List<UserInterest>(){
                new UserInterest(){ Interest="Playing Chess"},new UserInterest(){ Interest="Playing Computer Games"}
            };
            _dataForTest.Add(data);

        }


        private void AddUsersForTest()
        {
            prepareUserForTest();
            foreach (var dt in _dataForTest)
            {
                _userRepo.AddUsers(dt);
            }
        }



        [Fact]
        public void AddUserTest()
        {
            AddUsersForTest();
            var data = _userRepo.GetUsers();
            Assert.NotEmpty(data);

            bool hasAddedUser = false;
            
            foreach (var dt in data)
            {
                var addedTestData = _dataForTest.Find(x => x.EmailAddress == dt.EmailAddress);
                if (addedTestData != null)
                {
                    hasAddedUser = true;
                    Assert.NotEmpty(addedTestData.Interest);
                }
            }

            Assert.True(hasAddedUser);
        }

    }
}
