using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Seva.Assessment.WebApi.Model;
using Seva.Assessment.DataService;
using Seva.Assessment.DataService.User;

namespace Seva.Assessment.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {

        private UserRepositoryNHibernateImpl _userRepo = null;
        public UserController()
        {
            _userRepo = new UserRepositoryNHibernateImpl();
        }
        
        // GET api/values
        [HttpGet]
        public IEnumerable<UserModel> GetAllUsers()
        {
            
            var data= _userRepo.GetUsers();
            return data.Select(x => new UserModel()
            {
                FirstName = x.FirstName,
                LastName = x.LastName,
                EmailAddress = x.EmailAddress,
                Interests = x.Interest.Select(y=>y.Interest).ToList()
            });
        }

        [HttpGet("{searchname}")]
        public IEnumerable<UserModel> GetUsersBySearchName(string searchname)
        {
           
            var data = _userRepo.GetUsers(searchname);
            return data.Select(x => new UserModel()
            {
                FirstName = x.FirstName,
                LastName = x.LastName,
                EmailAddress = x.EmailAddress,
                Interests = x.Interest.Select(y => y.Interest).ToList()
            });
        }

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody]UserModel value)
        {
            if (!ModelState.IsValid) return new BadRequestResult();
            if (value == null) return new BadRequestResult();

            _userRepo.AddUser(new DataService.User.UserData()
            {
                FirstName=value.FirstName,
                LastName=value.LastName,
                EmailAddress=value.EmailAddress,
                Interest = value.Interests.Select(x=>new UserInterest() { Interest=x}).ToList()
            });
            return new OkObjectResult("Successfully Added");
        }


        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {            
            _userRepo.RemoveUser(id);
        }
    }
}
