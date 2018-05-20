using System.ComponentModel.DataAnnotations;

namespace MediaManager.API.Controllers
{
    public class UserForUpdateDTO
    {
       


        [Required]
        public string Email { get; set; }


       
    }
}