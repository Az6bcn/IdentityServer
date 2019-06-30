using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4AspNetIdentity.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServer4AspNetIdentity.Service
{
    public class IdentityClaimsProfileService : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityClaimsProfileService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            string sub = context.Subject.GetSubjectId();
            ApplicationUser user = await _userManager.FindByIdAsync(sub);
            IList<Claim> userClaims = await _userManager.GetClaimsAsync(user);

            List<Claim> claims = new List<Claim>()
            {
                new Claim(JwtClaimTypes.Subject, user.Id),
            };

            claims.AddRange(userClaims);

            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            string sub = context.Subject.GetSubjectId();
            ApplicationUser user = await _userManager.FindByIdAsync(sub);
            context.IsActive = user != null;
        }
    }
}
