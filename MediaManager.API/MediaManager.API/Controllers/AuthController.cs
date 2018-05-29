using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MediaManager.API.Models;
using MediaManager.DAL.DBEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace MediaManager.API.Controllers
{
    /// <summary>
    /// Conroller to handle authentication requests
    /// </summary>
    public class AuthController : Controller
    {
        private DatabaseContext context;
        private SignInManager<IdentityUser> signInManager;
        private UserManager<IdentityUser> userManager;
        private IPasswordHasher<IdentityUser> hasher;

        /// <summary>
        /// Constructor of Auth conroller
        /// </summary>
        /// <param name="context">DbContext object</param>
        /// <param name="signInManager">signin manager</param>
        /// <param name="userManager">user manager</param>
        /// <param name="hasher">hasger</param>
        public AuthController(DatabaseContext context, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, IPasswordHasher<IdentityUser> hasher)
        {
            this.context = context;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.hasher = hasher;
        }

        /// <summary>
        /// Logs in a user(works on the basis of tokens)
        /// </summary>
        /// <param name="model">creadential</param>
        /// <returns>result of the login operation</returns>
        [HttpPost("api/auth/login")]
        public async Task<IActionResult> Login([FromBody] CredentialModel model)
        {
            try
            {
                var result = await signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);
                if (result.Succeeded)
                {
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return BadRequest();
        }


        /// <summary>
        /// Creates token
        /// </summary>
        /// <param name="model">Credential of user</param>
        /// <returns>a token</returns>
        [HttpPost("api/auth/token")]
        public async Task<IActionResult> CreateToken([FromBody] CredentialModel model)
        {
            try
            {
                var user = await userManager.FindByNameAsync(model.UserName);
                if (user != null)
                {
                    if (hasher.VerifyHashedPassword(user, user.PasswordHash, model.Password) == PasswordVerificationResult.Success)
                    {
                        var claims = await userManager.GetClaimsAsync(user);

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("verylongkeyvaluethatissecured"));
                        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        var token = new JwtSecurityToken(
                            issuer: "issuer",
                            audience: "audience",
                            claims: claims,
                            expires: DateTime.UtcNow.AddMinutes(15),
                            signingCredentials: creds);

                        return Ok(new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        });
                    }
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
