using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace MediaManager.DAL.DBEntities
{
    [Table("User")]
    public class User 
    {
        [Key]
        [Required]
        [MinLength(8, ErrorMessage = "Username should be at least 8 caracters")]
        public string Username { get; set; }


        [Required]
        public string Email { get; set; }


        [Required]
        public string Name { get; set; }


        public DateTimeOffset RegistrationDate { get; set; }


    }
}
