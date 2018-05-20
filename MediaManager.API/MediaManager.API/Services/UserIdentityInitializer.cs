using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MediaManager.DAL.DBEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MediaManager.API
{
    public class UserIdentityInitializer
    {
        private RoleManager<IdentityRole> _roleMgr;
        private UserManager<IdentityUser> _userMgr;

        public UserIdentityInitializer(UserManager<IdentityUser> userMgr, RoleManager<IdentityRole> roleMgr)
        {
            _userMgr = userMgr;
            _roleMgr = roleMgr;
        }

        public async Task Seed()
        {
            try
            {


                var user1 = await _userMgr.FindByNameAsync("superuser");
                var user = await _userMgr.FindByNameAsync("normaluser");
                if(user == null)
                {
                     user = new IdentityUser
                    {
                        UserName = "normaluser",
                        SecurityStamp = Guid.NewGuid().ToString(),

                    };
                    var result = await _userMgr.CreateAsync(user, "Password@123");
                }


                // Add User

                var role = await _roleMgr.FindByNameAsync("Admin");
                if (role == null)
                {
                    role = new IdentityRole
                    {
                        Name = "Admin",

                    };

                    //var roleClaim = new IdentityRoleClaim<string>() { ClaimType = "IsAdmin", ClaimValue = "True" };
                    //// roleClaim.RoleId = role.Id;

                    //await _roleMgr.CreateAsync(role);
                    //var rd = await _roleMgr.AddClaimAsync(role, new Claim("IsAdmin", "False"));
                }


                var roleClaim = new IdentityRoleClaim<string>() { ClaimType = "IsAdmin", ClaimValue = "True" };
                // roleClaim.RoleId = role.Id;

               
                var rd = await _roleMgr.AddClaimAsync(role, new Claim("IsAdmin", "False"));


                //     var claims =await  _roleMgr.GetClaimsAsync(role);

                var roleResult = await _userMgr.AddToRoleAsync(user1, "Admin");
              //  var claimResult = await _userMgr.AddClaimAsync(user, new Claim("SuperUser", "True"));
                var claimResult1 = await _userMgr.AddClaimAsync(user, new Claim("Reader", "True"));
                var claimResult2 = await _userMgr.AddClaimAsync(user, new Claim("Writer", "True"));

                //if (!userResult.Succeeded || !roleResult.Succeeded || !claimResult.Succeeded)
                //{
                //    throw new InvalidOperationException("Failed to build user and roles");
                //}


            }
            catch (Exception ex)
            {

            }
        }

        }
    }


