using System.ComponentModel.DataAnnotations;

namespace MediaManager.API.Controllers
{
    /// <summary>
    /// credential model 
    /// </summary>
    public class CredentialModel
    {
        /// <summary>
        /// Username
        /// </summary>
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}