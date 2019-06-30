// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityServer4AspNetIdentity
{
    public static class Config
    {
        // Tell Identity Server about the API resources we are protecting / that a client might want to access 
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                // custom claims I want 
                new IdentityResource
                {
                    Name = "userrole",
                    Description = "User level Claim",
                    UserClaims = { "role" }
                }
            };
        }

        // Tell Identity Server about the API resources we are protecting / that a client might want to access 
        public static IEnumerable<ApiResource> GetApis()
        {
            return new ApiResource[]
            {
                new ApiResource("resourcesapi", "Resource API")
                {
                    Scopes = {
                        new Scope("api.read"),
                        new Scope("api.write")
                    }
                }
            };
        }

        // Tell Identity Server about the Clients that will be accessing it 
        public static IEnumerable<Client> GetClients()
        {
            return new[]
            {
              
                // SPA client using implicit flow
                new Client
                {
                    ClientId = "AngularClientApp",
                    ClientName = "Angular SPA Client",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    RequirePkce = false,
                    RequireClientSecret = false,
                    RedirectUris = {"http://localhost:4200/auth-callback"},
                    PostLogoutRedirectUris = { "http://localhost:4200" },
                    AllowedCorsOrigins = { "http://localhost:4200" },
                    AllowAccessTokensViaBrowser = true,
                    AccessTokenLifetime = 3600, // 1hr,
                    AlwaysSendClientClaims = true,
                    AllowedScopes = { "openid", "profile", "email", "api.read", "userrole" }
                }
            };
        }
    }
}