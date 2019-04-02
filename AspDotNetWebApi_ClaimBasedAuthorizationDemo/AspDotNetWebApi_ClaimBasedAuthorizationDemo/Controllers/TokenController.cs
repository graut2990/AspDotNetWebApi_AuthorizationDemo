using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Mvc;


namespace AspDotNetWebApi_ClaimBasedAuthorizationDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        [HttpGet("{role}")]
        public string Get(string role)
        {
            const string Issuer = "https://gov.uk";

            //var claims = new List<Claim> {
            //         new Claim(ClaimTypes.Name, "Andrew", ClaimValueTypes.String, Issuer),
            //         new Claim(ClaimTypes.Surname, "Lock", ClaimValueTypes.String, Issuer),
            //         new Claim(ClaimTypes.Country, "UK", ClaimValueTypes.String, Issuer),
            //         new Claim("ChildhoodHero", "Ronnie James Dio", ClaimValueTypes.String)
            //};
            var claims = new Claim[]
           {
                #region setting the different at the time authenticating the userfor authorization

                new Claim(JwtRegisteredClaimNames.Sub, "alice"),
                          new Claim(ClaimTypes.Name, "Andrew", ClaimValueTypes.String, Issuer),
                     new Claim(ClaimTypes.Surname, "Lock", ClaimValueTypes.String, Issuer),
                     new Claim(ClaimTypes.Country, "UK"),
                  new Claim("Role", role),
                  new Claim("Role", "admin"),
                new Claim("accesses", "report")

               #endregion
           };

            var userIdentity = new ClaimsIdentity(claims);

            var userPrincipal = new ClaimsPrincipal(userIdentity);            
            HttpContext.User = userPrincipal;
            HttpContext.User.AddIdentity(userIdentity);



            var jwt = new JwtSecurityToken(claims: claims);
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return encodedJwt;
        }
    }
}