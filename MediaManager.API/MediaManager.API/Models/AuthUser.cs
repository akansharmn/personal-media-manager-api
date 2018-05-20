using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MediaManager.API.Models
{
    public class AuthUser1 :     IdentityUser
    {
        public string Name { get; set; }
    }
}
