using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MediaManager.API.Models
{
    public class UserForCreationDTO
    {
       
        [Required]
        [MinLength(8, ErrorMessage = "Username should be at least 8 caracters")]
        public string Username { get; set; }


        [Required]
        public string Email { get; set; }


        [Required]
        public string Name { get; set; }

     
    }
}
