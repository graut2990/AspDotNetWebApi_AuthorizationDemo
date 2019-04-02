using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AspDotNetWebApi_ClaimBasedAuthorizationDemo
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
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(options =>
               {
                   options.TokenValidationParameters = new TokenValidationParameters()
                   {
                       ValidateIssuer = false,
                       ValidateAudience = false,
                       ValidateLifetime = false,
                       ValidateIssuerSigningKey = false,
                       ValidateActor = false,
                       RequireSignedTokens = false,
                       NameClaimType = JwtRegisteredClaimNames.Sub,

                       #region Role Based Auth
                       RoleClaimType = "Role"
                       #endregion

                   };                  

               });

            services.AddAuthorization(opt =>
            {
                #region Claim Based Auth
                opt.AddPolicy("hasReportAccess",
                    policy => policy
                    .RequireClaim("accesses", "report")
                    .RequireRole("user"));
                #endregion

                #region Policy Based Auth
                opt.AddPolicy("accessibleOnlyDuringOfficeHours",
                   policy => policy.AddRequirements(new OfficeHoursRequirement(8, 23))
                       .RequireClaim("accesses", "report")
                       .RequireRole("user"));
                #endregion
            });

            services.AddSingleton<IAuthorizationHandler, OfficeHoursRequirementHandler>();

            services.AddSingleton<IAuthorizationHandler, AuthorRequirementHandler>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
