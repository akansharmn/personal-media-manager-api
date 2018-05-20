using System.ComponentModel.DataAnnotations;

namespace MediaManager.API.Controllers
{
    public class CredentialModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}