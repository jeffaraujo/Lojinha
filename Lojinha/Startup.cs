﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication;
using Lojinha.Infraestructure.Storage;
using Lojinha.Infraestructure.Redis;
using Lojinha.Core.Services;
using AutoMapper;
using Lojinha.Infraestructure.Mappings;

namespace Lojinha
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

            services.ConfigureApplicationCookie( options => 
            {
                options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None;
            });

            services.AddAuthentication(AzureADDefaults.AuthenticationScheme).AddAzureAD(options => 
            {
                Configuration.Bind("AzureAd", options);
            });

            services.AddSingleton<IRedisCache, RedisCache>();
            services.AddScoped<IProdutoServices, ProdutoServices>();
            services.AddScoped<IAzureStorage, AzureStorage>();
            Mapper.Initialize(options => options.AddProfile<ProdutoProfile>()); //Inicializando o AutoMapper
            services.AddAutoMapper();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
