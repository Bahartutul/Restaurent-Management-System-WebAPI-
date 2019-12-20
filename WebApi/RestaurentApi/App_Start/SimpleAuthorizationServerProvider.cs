using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Security.OAuth;
using RestaurentApi.Models;

namespace RestaurentApi.App_Start
{
    internal class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            ResContext _context = new ResContext();

            //RegisterUser userData = await (from a in _context.RegisterUsers
            //                               where a.UserName == context.UserName && a.UserPassword == context.Password).Select().FirstOrDefault();

            var userData= _context.RegisterUsers.SingleOrDefault(m => m.UserName==context.UserName && m.UserPassword==context.Password);
            if (userData != null)
            {
                identity.AddClaim(new Claim(ClaimTypes.Name, userData.UserName));
                context.Validated(identity);
            }
            else
            {
                context.SetError("invalid_grant", "Provided username and password is incorrect");
                context.Rejected();
            }
        }
    }
}