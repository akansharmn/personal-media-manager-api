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

namespace MediaManager.API
{
    public class Startup
    {
        IConfiguration configuration;
        DbConnection connectionString;
        
        public Startup(IHostingEnvironment environment)
        {
            var builder = new ConfigurationBuilder().SetBasePath(environment.ContentRootPath).AddJsonFile("appsettings.json");
            configuration = builder.Build();
           

        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(ExceptionHandler));
            // var connectionString = configuration["connectionstring"];
            // var abc = environment.ContentRootPath;
            var sp = services.BuildServiceProvider();
            var service = sp.GetService<IHostingEnvironment>();
            var connectionString = "Data Source = " + Path.Combine(service.ContentRootPath, "MediaManager.db");
           
            
            services.AddDbContext<DatabaseContext>(option => option.UseSqlite(connectionString));
            // services.AddScoped<IRepository<Video>, VideoRepository>();
            services.AddMvc(setup =>
            {
                setup.Filters.Add(new RequireHttpsAttribute());
                setup.ReturnHttpNotAcceptable = true;
                setup.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
                setup.InputFormatters.Add(new XmlDataContractSerializerInputFormatter());

                var jsonOutputFormatter = setup.OutputFormatters.OfType<JsonOutputFormatter>().FirstOrDefault();

                if(jsonOutputFormatter != null)
                {
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.ak.hateoas.json");
                }
            }).AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });
            services.AddResponseCaching();//before hhtpcache headers


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
            services.AddScoped<IRepository<Participant>, ParticipantRepository>();
            services.AddScoped<VideoRepository, VideoRepository>();
            services.AddScoped<UserRepository, UserRepository>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
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
                            });

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

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, DatabaseContext context, UserIdentityInitializer initializer)
        {

            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}

            app.UseMiddleware<ExceptionHandler>();

            app.UseHttpCacheHeaders(
                
            
            );  // should be before mvc

            app.UseAuthentication();


            //app.UseJwtBearerAuthentication(new JwtBearerOptions()
            //{
            //    AutomaticAuthenticate = true,
            //    AutomaticChallenge = true,

            //});
            context.Database.EnsureCreatedAsync().Wait();
           // initializer.Seed().Wait();
            app.UseMvc();

            MetadataStore.LoadEntities(context);

            
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Video, VideoForDisplayDTO>()
            .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => TimeSpan.FromSeconds(src.Duration)))
            .ForMember(dest => dest.WatchOffset, opt => opt.MapFrom(src => TimeSpan.FromSeconds(src.WatchOffset)))
            .ForMember(dest => dest.Participant, opt => opt.MapFrom(src => src.VideoParticipants));

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
                .ForMember(x => x.VideoId, opt => opt.Ignore())
                .ForMember(x => x.VideoParticipants, opt => opt.Ignore());

                cfg.CreateMap<VideoParticipant, string>().ConvertUsing(x => x.Participant.ParticipantName);

                cfg.CreateMap<UserForCreationDTO, User>()
                .ForMember(x => x.RegistrationDate, opt => opt.Ignore());

                cfg.CreateMap<User, UserForDisplayDTO>();
            }
            );
            
           


        }
    }
}
