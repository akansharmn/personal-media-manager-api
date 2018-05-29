using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MediaManager.API.Controllers;
using MediaManager.API.Helpers;
using MediaManager.API.Models;
using MediaManager.API.Repository;
using MediaManager.API.Services;
using MediaManager.DAL.DBEntities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using IdentityServer4.AccessTokenValidation;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.Extensions.PlatformAbstractions;

namespace MediaManager.API
{
    /// <summary>
    /// Class containing code to be executed first time
    /// </summary>
    public class Startup
    {
        IConfiguration configuration;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="environment">environemt to get the configuration file</param>
        public Startup(IHostingEnvironment environment)
        {
            var builder = new ConfigurationBuilder().SetBasePath(environment.ContentRootPath).AddJsonFile("appsettings.json");
            configuration = builder.Build();


        }

        // This method gets called by the runtime. Use this method to add services to the container.
        /// <summary>
        /// the method is called at runtime
        /// </summary>
        /// <param name="services">container containg services</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(ExceptionHandler));
            var serviceBuilder = services.BuildServiceProvider();
            var service = serviceBuilder.GetService<IHostingEnvironment>();
            var connectionString = "Data Source = " + Path.Combine(service.ContentRootPath, "MediaManager.db");

            services.AddDbContext<DatabaseContext>(option => option.UseSqlite(connectionString));
            services.AddMvc(setup =>
            {
                setup.Filters.Add(new RequireHttpsAttribute());
                setup.ReturnHttpNotAcceptable = true;
                setup.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
                setup.InputFormatters.Add(new XmlDataContractSerializerInputFormatter());

                var jsonOutputFormatter = setup.OutputFormatters.OfType<JsonOutputFormatter>().FirstOrDefault();

                if (jsonOutputFormatter != null)
                {
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.ak.hateoas+json");
                }
            }).AddJsonOptions(options =>
            {
                options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });
            services.AddResponseCaching();


            services.AddHttpCacheHeaders(
                (expirationModelOptions) =>
            {
                expirationModelOptions.MaxAge = 60;
            },
                (validationModelOptions) =>
                {
                    validationModelOptions.AddMustRevalidate = true;
                });
            services.AddSingleton<MetadataStore>();
            services.AddScoped<ParticipantRepository, ParticipantRepository>();
            services.AddScoped<VideoRepository, VideoRepository>();
            services.AddScoped<UserRepository, UserRepository>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<TagRepository, TagRepository>();

            services.AddScoped<IUrlHelper, UrlHelper>(implementationFactory =>
            {
                var actionContext = implementationFactory.GetService<IActionContextAccessor>().ActionContext;
                return new UrlHelper(actionContext);
            });
            services.AddTransient<IPropertyMappingService, PropertyMappingService>();
            services.AddTransient<ITypeHelperService, TypeHelperService>();
            services.AddCors(cfg =>
            {
                cfg.AddPolicy("default", bldr =>
                {
                    bldr.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://wildermuth.com");

                });

                cfg.AddPolicy("AnyGET", bldr =>
                {
                    bldr.AllowAnyHeader().WithMethods("GET").AllowAnyOrigin();

                });
            });
         //   services.AddMvcCore().AddApiExplorer();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info
                {
                    Title = "MediaManager API",
                    Contact = new Contact { Name = "Akansha Raman", Email = "akansha.raman23@gmail.com", Url = "akansharman.github.io" },
                    Description = "An API to manage your offline activities.",
                    Version = "1"
                });

                var filePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "MediaManager.API.xml");
                c.IncludeXmlComments(filePath);

                //Define OAuth2.0 scheme that is in use

                //c.AddSecurityDefinition("oauth2", new OAuth2Scheme
                //{
                //    Type = "oauth2",
                //    Flow = "Hybridand ClientCredential",
                //    AuthorizationUrl = "http://localhost:55470/",
                //    Scopes = new Dictionary<string, string>
                //    {
                //        { "Reader", "Access Read operation" },
                //        {"Writer", "Access Write Operation" },
                //        {"Admin", "Access update user operation" }
                //    }
                //});

                //c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                //{
                //    {"oauth2", new[] {"Reader", "Writer"} }
                //});
            });


            #region jwt auth 
            /*
            services.AddIdentity<IdentityUser, IdentityRole>()
                            .AddEntityFrameworkStores<DatabaseContext>();

                        services.ConfigureApplicationCookie(options =>
                        {
                            options.Events = new CookieAuthenticationEvents()
                            {
                                OnRedirectToLogin = (ctx =>
                                {
                                    if (ctx.Request.Path.StartsWithSegments("/api") && ctx.Response.StatusCode == 200)
                                    {
                                        ctx.Response.StatusCode = 401;
                                    }
                                    return Task.CompletedTask;
                                }),

                                OnRedirectToAccessDenied = (ctx) =>
                                {
                                    if (ctx.Request.Path.StartsWithSegments("/api") && ctx.Response.StatusCode == 200)
                                    {
                                        ctx.Response.StatusCode = 401;
                                    }
                                    return Task.CompletedTask;
                                }
                            };
                        }
                        );
           
                        services.AddTransient<UserIdentityInitializer>();
                            services.AddAuthentication().AddJwtBearer(jwt =>
                            {

                                jwt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                                {
                                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("verylongkeyvaluethatissecured")),
                                    ValidAudience = "audience",
                                    ValidIssuer = "issuer",
                                    ValidateLifetime = true,
                                };
                            });

                            services.AddAuthorization(auth =>
                            {
                                //auth.DefaultPolicy = 
                                //    new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme).RequireAuthenticatedUser().Build();
                                auth.AddPolicy("SuperUser", p =>
                                {
                                    p.RequireClaim("SuperUser", "True");
                                    p.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                                 });
                                auth.AddPolicy("Reader", p =>
                                {
                                    p.RequireClaim("Reader", "True");
                                    p.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                                });
                                auth.AddPolicy("Writer", p =>
                                {
                                    p.RequireClaim("Writer", "True");
                                    p.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                                });
                            });*/
            #endregion



            #region OAuthImplementation

            /*    services.AddAuthentication(options =>
                {
                    options.DefaultScheme = "Cookies";
                    options.DefaultChallengeScheme = "oidc";
                }).AddCookie("Cookies")
                .AddOpenIdConnect("oidc", options =>
                {
                    options.SignInScheme = "Cookies";
                    options.Authority = "http://identityserverakansha.azurewebsites.net/";
                    options.RequireHttpsMetadata = false;
                    options.ClientId = "reactapp";
                    options.SaveTokens = true;
                    options.Events.OnMessageReceived = async ctx =>
                    {

                        Debug.Write(ctx.Principal);
                    };
                });*/

            #endregion

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "http://localhost:55470/";
                    options.RequireHttpsMetadata = false;

                    options.ApiName = "api";

                    //options.ApiSecret = "secret";
                });

            services.AddAuthorization(auth =>
            {

                auth.AddPolicy("Reader", x =>
                 {
                     x.RequireClaim(ClaimTypes.Role, "Read");
                     x.AddAuthenticationSchemes(IdentityServerAuthenticationDefaults.AuthenticationScheme);
                 });
                auth.AddPolicy("Writer", x =>
                {
                    x.RequireClaim(ClaimTypes.Role, "Read");
                    x.AddAuthenticationSchemes(IdentityServerAuthenticationDefaults.AuthenticationScheme);
                });
                auth.AddPolicy("SuperUser", x =>
                {
                    x.RequireClaim(ClaimTypes.Role, "Admin");
                    x.AddAuthenticationSchemes(IdentityServerAuthenticationDefaults.AuthenticationScheme);
                });
                auth.DefaultPolicy = new AuthorizationPolicyBuilder(auth.GetPolicy("Reader")).Build();
            });
        }


        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">ApplicationBuilder</param>
        /// <param name="env">HostingEnvironment</param>
        /// <param name="context">Database context</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, DatabaseContext context)
        {

            if (env.IsDevelopment())
            {
                app.UseMiddleware<ExceptionHandler>();
            }
            app.UseHttpCacheHeaders();  // should be before mvc
            app.UseAuthentication();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.OAuthClientId("tes");
                c.OAuthClientSecret("tes");

                c.SwaggerEndpoint("../swagger/v1/swagger.json", "MediaManager API");
            });

            //app.UseJwtBearerAuthentication(new JwtBearerOptions()
            //{
            //    AutomaticAuthenticate = true,
            //    AutomaticChallenge = true,

            //});
            //  context.Database.EnsureCreatedAsync().Wait();
            // initializer.Seed().Wait();
            app.UseMvc();

            MetadataStore.LoadEntities(context);


            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Participant, ParticipantVideoDisplayDTO>().ForMember(x => x.ParticipantName, opt => opt.MapFrom(src => src.ParticipantName));
                cfg.CreateMap<VideoParticipant, ParticipantVideoDisplayDTO>().ForMember(x => x.ParticipantName, opt => opt.MapFrom(x => x.Participant.ParticipantName));
                cfg.CreateMap<Video, VideoForDisplayDTO>()
                  .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => TimeSpan.FromSeconds(src.Duration)))
                 .ForMember(dest => dest.WatchOffset, opt => opt.MapFrom(src => TimeSpan.FromSeconds(src.WatchOffset)));

                cfg.CreateMap<VideoForCreationDTO, Video>().ForMember(x => x.Author, opt => opt.Ignore())
                 .ForMember(x => x.AuthorId, opt => opt.Ignore())
                 .ForMember(x => x.Domain, opt => opt.Ignore())
                 .ForMember(x => x.DomainId, opt => opt.Ignore())
                 .ForMember(x => x.VideoId, opt => opt.Ignore())
                 .ForMember(x => x.VideoParticipants, opt => opt.Ignore());

                cfg.CreateMap<VideoForUpdateDTO, Video>()
                .ForMember(x => x.Author, opt => opt.Ignore())
                .ForMember(x => x.AuthorId, opt => opt.Ignore())
                .ForMember(x => x.Domain, opt => opt.Ignore())
                .ForMember(x => x.DomainId, opt => opt.Ignore())
                .ForMember(x => x.VideoParticipants, opt => opt.Ignore());

                cfg.CreateMap<VideoParticipant, string>().ConvertUsing(x => x.Participant.ParticipantName);

                cfg.CreateMap<UserForCreationDTO, User>()
                .ForMember(x => x.RegistrationDate, opt => opt.Ignore());

                cfg.CreateMap<User, UserForDisplayDTO>();

                cfg.CreateMap<UserForUpdateDTO, User>().ForMember(x => x.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(x => x.Username, opt => opt.Ignore())
                .ForMember(x => x.RegistrationDate, opt => opt.Ignore())
                .ForMember(x => x.Name, opt => opt.Ignore());
            }
            );
        }
    }
}
