using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seva.Assessment.WebApi.Model
{
    public class UserModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public List<string> Interests { get; set; }
    }
}
