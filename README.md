# IdentityServer
OpenID Connect (Authentication, Authorization) protocol implementation using IdentityServer. 
Angular App as client, users Authenticate against the Authorization Server (Identity Server 4) and gets back
a JWT Token with which they can access Identity Server4 protected WebAPI, using the OpenID Connect Implicit grant flow 
and ASP.NET Identity has user provider.

Implements IProfileService to add users claims to the access token.

https://github.com/IdentityServer/IdentityServer4.Templates/tree/master/src/IdentityServer4AspNetIdentity

