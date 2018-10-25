using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Seva.Assessment.WebApi.Model;
using Seva.Assessment.DataService;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace Seva.Assessment.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
       
        private string _connectionString = "";
        public UserController(IConfiguration config)
        {
           _connectionString = config.GetConnectionString("DefaultConnection");
        }
        
        //private void Setup() => _connectionString = "Data Source=.;Initial Catalog=Seva_Assessment;Persist Security Info=True;User ID=sa;Password=CaFt@#85647";


        // GET api/values
        [HttpGet]
        public IEnumerable<UserModel> Get()
        {
            var userRepo = new UserRepositoryNHibernateImpl(_connectionString);
            var data= userRepo.GetUsers();
            return data.Select(x => new UserModel()
            {
                FirstName = x.FirstName,
                LastName = x.LastName,
                EmailAddress = x.EmailAddress,
                Interests = x.Interest.Select(y=>y.Interest).ToList()
            });
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public IEnumerable<UserModel> Get(string id)
        {
            var userRepo = new UserRepositoryNHibernateImpl(_connectionString);
            var data = userRepo.GetUsers(id);
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
        public void Post([FromBody]UserModel value)
        {

        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]UserModel value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
        }
    }
}
