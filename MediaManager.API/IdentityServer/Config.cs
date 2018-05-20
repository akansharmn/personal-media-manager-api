using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace IdentityServer
{
    public static class Config
    {
        public static List<TestUser> GetUsers()
        {
            return  new List<TestUser>()
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "tester",
                    Password = "password",

                    Claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, "tester"),
                        new Claim(ClaimTypes.Role, "Read"),
                        new Claim(ClaimTypes.Role, "Write")
                    }
                },
                new TestUser
                {
                    SubjectId = "2",
                    Username = "admin",
                    Password = "password",

                    Claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name, "admin"),
                        new Claim(ClaimTypes.Role, "UpdateSchema"),
                        new Claim(ClaimTypes.Role, "Read"),
                        new Claim(ClaimTypes.Role, "Write")
                    }
                },

                new TestUser
                {
                    SubjectId = "2",
                    Username = "reader",
                    Password = "password",

                    Claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name, "reader"),
                        new Claim(ClaimTypes.Role, "Read"),

                    }
                }
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
               new IdentityResources.OpenId(),
               new IdentityResources.Profile()
            };
        }

        public static List<Client> GetClients()
        {
            return new List<Client>()
            {
                new Client
                {
                    ClientId = "reactapp",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowedScopes = { "api" , IdentityServerConstants.StandardScopes.Profile, IdentityServerConstants.StandardScopes.OpenId},
                    RequireConsent = true,
                    Enabled = true,
                    ClientName = "React-Based client",

                    RedirectUris = new List<string>()
                    {
                        "http://localhost:59019/"
                        //"https://localhost:44381/"
                    }
                }
            };
        }

        public static List<ApiResource> GetApiResources()
        {
            return new List<ApiResource>()
            {
                new ApiResource("api", "Media Manager API")

             };
        }
    }
}
