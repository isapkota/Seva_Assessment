using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Seva.Assessment.WebApi.Model
{
    public class UserModel
    {
        [StringLength(50, ErrorMessage = "The {0} can be up to 50 characters long.")]
        [Required(ErrorMessage = "The FirstName needs a value!")]
        public string FirstName { get; set; }

        [StringLength(50, ErrorMessage = "The {0} can be up to 50 characters long.")]
        [Required(ErrorMessage = "The LastName needs a value!")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string EmailAddress { get; set; }


        public List<string> Interests { get; set; }
    }
}
