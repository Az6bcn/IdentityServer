// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using IdentityServer4.AspNetIdentity;
using IdentityServer4.Services;
using IdentityServer4AspNetIdentity.Data;
using IdentityServer4AspNetIdentity.Models;
using IdentityServer4AspNetIdentity.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace IdentityServer4AspNetIdentity
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {

            // Register EF Core Context with dependency injection
            services.AddDbContext<AuthServerApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // Asp.net Identity
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AuthServerApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc().SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_1);

            // Register Identity Server
            services.AddIdentityServer()
                // tell IDServer the cerificate to use for signing JWTs
                .AddSigningCredential("CN=localhost")
                // this adds the operational data from DB (codes, tokens, consents)
                // to store things like authorization grants, consents, and tokens (refresh and reference) in an EF-supported database
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder => builder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                        b => b.MigrationsAssembly("IdentityServer4AspNetIdentity"));
                    // this enables automatic token cleanup. this is optional.
                    options.EnableTokenCleanup = true;
                    options.TokenCleanupInterval = 30; // interval in seconds
                })
                /*extension methods to configure IDserver: this can be from DB/Config file/ Class but must return IDServer expected type */
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApis()) // To register and let IDServer know about the APIs we wanna protect
                .AddInMemoryClients(Config.GetClients()) // To register and let IDServer know about our clients
                .AddAspNetIdentity<ApplicationUser>() // After configuring Asp.Net Identity Core, let IDServer know you're using this Asp.net Core Identity userstore and pass it the IdentityUser
                .AddProfileService<IdentityClaimsProfileService>();

            /* Claims aren’t added to the user’s access token automatically. We have to implement the inbuilt IProfileService to achieve this.
             * i.e to add the users claims to the access token
             * http://docs.identityserver.io/en/latest/reference/profileservice.html?highlight=IProfileService
             */
            services.AddTransient<IProfileService, IdentityClaimsProfileService>();

            services.AddTransient<SeedData>();

            if (Environment.IsDevelopment())
            {
            }
            else
            {
                throw new Exception("need to configure key material");
            }

            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    // register your IdentityServer with Google at https://console.developers.google.com
                    // enable the Google+ API
                    // set the redirect URI to http://localhost:5000/signin-google
                    options.ClientId = "copy client ID from Google here";
                    options.ClientSecret = "copy client secret from Google here";
                });
        }

        public void Configure(IApplicationBuilder app, SeedData seedData)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            // Seed Users
            seedData.Seed();

            // Add Identity Server 4 to the pipeline
            app.UseIdentityServer();

            app.UseMvcWithDefaultRoute();
        }
    }
}


