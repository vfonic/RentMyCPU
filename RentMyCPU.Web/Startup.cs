using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using RentMyCPU.Backend.Data;
using RentMyCPU.Backend.Data.Entities;
using RentMyCPU.Backend.Hubs;
using RentMyCPU.Backend.Logic.Cryptography;
using RentMyCPU.Backend.Logic.Jwt;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RentMyCPU.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                Configuration.GetConnectionString("DefaultConnection"), dbContextOptions =>
                {
                    dbContextOptions.EnableRetryOnFailure();

                }));

            services.AddIdentity<User, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidateLifetime = true,
                            LifetimeValidator = (before, expires, token, param) => expires > DateTime.UtcNow,
                            ValidateAudience = false,
                            ValidateIssuer = false,
                            ValidateActor = false,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["JwtBearer:SecurityKey"]))
                        };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            // If the request is for our hub...
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) &&
                                (path.StartsWithSegments("/hubs/taskhub") || path.StartsWithSegments("/taskhub")))
                            {
                                // Read the token out of the query string
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddJwtTokenProvider(options =>
            {
                options.Audience = Configuration["JwtBearer:Issuer"];
                options.Issuer = Configuration["JwtBearer:Audience"];
                options.SecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["JwtBearer:SecurityKey"]));
                options.SigningCredentials = new SigningCredentials(options.SecurityKey, SecurityAlgorithms.HmacSha256);
                options.Expiration = TimeSpan.FromDays(30 * 6);
            });

            services.AddSignalR();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            CryptoConfig.AddAlgorithm(typeof(RSAPKCS1SHA256SignatureDescription), "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256");

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });


            services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
            {
                builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .WithOrigins("https://localhost:5001", "http://localhost:5000", "https://rentmycpu.azurewebsites.net/");
            })); 
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime,
            ApplicationDbContext applicationDbContext)
        {
            // applicationLifetime.ApplicationStopping.Register(OnShutdown);

            applicationDbContext.Database.Migrate();
            var devicesConnected = applicationDbContext.Workers.Where(x => x.ConnexionId != null).ToList();
            devicesConnected.ForEach(x => x.ConnexionId = null);
            applicationDbContext.SaveChanges(); 

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseCors("CorsPolicy");

            app.UseHttpsRedirection();
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    var ext = Path.GetExtension(ctx.File.Name).ToLower();
                    if (ext == ".js" || ext == ".css")
                    {
                        ctx.Context.Response.Headers.Add("Cache-Control", "no-cache");
                    }
                }
            });
            app.UseSpaStaticFiles();

            app.UseAuthentication();
            app.UseSignalR(route =>
            {
                route.MapHub<TaskHub>("/taskhub", options =>
                {
                    options.ApplicationMaxBufferSize = 0;
                    options.TransportMaxBufferSize = 0;
                });
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
        private void OnShutdown()
        {
        }
    }
}
