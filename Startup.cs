using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using IdentityServer4.Validation;

namespace auth
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryClients(Config.GetClients());
            services.AddTransient<IResourceOwnerPasswordValidator,ResourceOwnerPasswordValidator>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)        .AddJwtBearer(options => {
                options.Audience = "api1";
                options.Authority = "http://127.0.0.1:5000/";
                options.RequireHttpsMetadata = false;
            });
            services.AddMvcCore().AddAuthorization().AddJsonFormatters() ;
            services.AddTransient<IApplicationUserRepository,ApplicationUserRepository>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            //loggerFactory.AddConsole();
            app.UseDeveloperExceptionPage();
            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
