using Moq;
using Seva.Assessment.DataService.User;
using Seva.Assessment.FluentNHibernetImpl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace Seva.Assessment.DataService.Test
{
    public class UserRepositoryNHibernateImplTest
    {
        private UserRepositoryNHibernateImpl _userRepo;
        private IQueryable<UserData> _testUserData;
        public UserRepositoryNHibernateImplTest()
        {
            prepareUserForTest();            
        }

        private void prepareUserForTest()
        {
            _testUserData = new List<UserData>(new[]
            {
            new UserData()
            {
                FirstName = "Ishwor",
                LastName = "Sapkota",
                EmailAddress = "saathee.ishwor@gmail.com",
                Interest = new List<UserInterest>()
                {
                new UserInterest(){ Interest="Playing Chess"},new UserInterest(){ Interest="Playing Computer Games"}
                }
            },
            new UserData()
            {
                FirstName = "Sanjita",
                LastName = "Sapkota",
                EmailAddress = "saathee.sanjita@gmail.com",
                Interest = new List<UserInterest>()
                {
                new UserInterest(){ Interest="Playing Ludo"},new UserInterest(){ Interest="Cooking"}
                }
            },
            new UserData()
            {
                FirstName = "Samyam",
                LastName = "Sapkota",
                EmailAddress = "saathee.samyam@gmail.com",
                Interest = new List<UserInterest>()
                {
                new UserInterest(){ Interest="Watching Television"},new UserInterest(){ Interest="Playing Computer Games"}
                }
            },
            new UserData()
            {
                FirstName = "Safal",
                LastName = "Sapkota",
                EmailAddress = "saathee.safal@gmail.com",
                Interest = new List<UserInterest>()
                {
                new UserInterest(){ Interest="Watching Television"},new UserInterest(){ Interest="Playing Computer Games"},new UserInterest(){ Interest="Riding Bicycle"}
                }
            }

            }).AsQueryable();

        }

        [Fact]
        public void UserRepository_ShouldReturnAllUsers()
        {
            var mockDbSet = new Mock<IDbSet<UserData>>();
            mockDbSet.Setup(d => d.FindAll()).Returns(_testUserData);

            var mockContext = new Mock<IDataContext<UserData>>();
            mockContext.Setup(c => c.Data).Returns(mockDbSet.Object);

            DataFactory.BuildUp(r => r.RegisterInstance(typeof(IDataContext<UserData>), mockContext.Object));
            var userRepo = new Seva.Assessment.DataService.UserRepositoryNHibernateImpl();
            var allUsers= userRepo.GetAllUsers();
            Assert.NotNull(allUsers);
            Assert.Equal(allUsers.Count(), _testUserData.Count());
        }

        [Fact]
        public void UserRepository_ShouldReturnUsersStartingWithSAM()
        {
            var selectedData = _testUserData.Where(x => x.FirstName.ToLower().Contains("sam") || x.LastName.ToLower().Contains("sam"));

            var mockDbSet = new Mock<IDbSet<UserData>>();
            mockDbSet.Setup(d => d.FindAll(It.IsAny<Expression<Func<UserData, bool>>>())).Returns(selectedData);

            var mockContext = new Mock<IDataContext<UserData>>();
            mockContext.Setup(c => c.Data).Returns(mockDbSet.Object);

            DataFactory.BuildUp(r => r.RegisterInstance(typeof(IDataContext<UserData>), mockContext.Object));
            var userRepo = new Seva.Assessment.DataService.UserRepositoryNHibernateImpl();
            var selectedUsers = userRepo.GetUsersBySearchString(It.IsAny<string>());
            Assert.NotNull(selectedUsers);
            Assert.Equal(selectedUsers.Count(), selectedData.Count());
        }

        [Fact]
        public void UserRepository_ShouldCallCreateOnce()
        {
            var mockDbSet = new Mock<IDbSet<UserData>>();
            mockDbSet.Setup(d => d.Add(It.IsAny<UserData>()));

            var mockContext = new Mock<IDataContext<UserData>>();
            mockContext.Setup(c => c.Data).Returns(mockDbSet.Object);

            DataFactory.BuildUp(
                r => r.RegisterInstance(typeof(IDataContext<UserData>), mockContext.Object));

            var userRepo = new Seva.Assessment.DataService.UserRepositoryNHibernateImpl();
            var createResult = userRepo.AddUser(_testUserData.FirstOrDefault());

            Assert.True(createResult);
            mockDbSet.Verify(d => d.Add(It.IsAny<UserData>()), Times.Exactly(1));
        }

        [Fact]
        public void UserRepository_ShouldCallRemoveOnce()
        {
            var mockDbSet = new Mock<IDbSet<UserData>>();
            mockDbSet.Setup(d => d.Remove(It.IsAny<UserData>()));

            var mockContext = new Mock<IDataContext<UserData>>();
            mockContext.Setup(c => c.Data).Returns(mockDbSet.Object);

            DataFactory.BuildUp(
                r => r.RegisterInstance(typeof(IDataContext<UserData>), mockContext.Object));

            var userRepo = new Seva.Assessment.DataService.UserRepositoryNHibernateImpl();
            var deleteResult = userRepo.RemoveUser(1);

            Assert.True(deleteResult);
            mockDbSet.Verify(d => d.Remove(It.IsAny<UserData>()), Times.Exactly(1));
        }
    }
}
