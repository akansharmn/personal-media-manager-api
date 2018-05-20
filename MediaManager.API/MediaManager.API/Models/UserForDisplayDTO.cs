using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaManager.API.Models
{
    public class UserForDisplayDTO
    {
        
        public string Username { get; set; }


        
        public string Email { get; set; }


       
        public string Name { get; set; }


        public DateTimeOffset RegistrationDate { get; set; }
    }
}
