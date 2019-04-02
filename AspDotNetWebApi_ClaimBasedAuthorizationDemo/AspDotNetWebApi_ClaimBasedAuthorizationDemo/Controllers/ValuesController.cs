using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspDotNetWebApi_ClaimBasedAuthorizationDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private IAuthorizationService _authorizationService;

        public ValuesController(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // Checking the authorization
        [HttpGet("{id}"), Authorize]
        public string Get(int id)
        {
            var user = HttpContext.User;

            var claims = user.Claims;

            var subject = HttpContext.User.Claims.FirstOrDefault(claim => claim.Subject.NameClaimType.ToLower() == "sub");
            return $"{subject.Value} is authorized!";

        }



        //Role based authorization
        [HttpGet("profile")]
        [Authorize(Roles = "admin")]
        public string GetProfile()
        {
            return $"{HttpContext.User.Identity.Name} is authorized!";
        }

        //Claim Based authorization
        [HttpGet("report")]
        [Authorize(Policy = "hasReportAccess")]
        public string GetReport()
        {
            return $"{HttpContext.User.Identity.Name} is authorized!";
        }

        //Policy based authorization
        [HttpGet("financereport")]
        [Authorize(Policy = "accessibleOnlyDuringOfficeHours")]
        public string GetFinanceReport()
        {
            return $"{HttpContext.User.Identity.Name} is authorized!";
        }

        //Resource based authorization
        [HttpPut("report/{id}")]
        public async Task<IActionResult> PutReport(string id)
        {

            string a = "fff";
            string b = $"moiohfogjf{a}";

         
            var report = new Report { Author = "alice", Content = "" }; // Here we would get the resource from somewhere
            var result = await _authorizationService.AuthorizeAsync(HttpContext.User, report, new AuthorRequirement());
            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }

        public string Login(string returnUrl = null)
        {
          

            const string Issuer = "https://gov.uk";

                         var claims = new List<Claim> {
                     new Claim(ClaimTypes.Name, "Andrew", ClaimValueTypes.String, Issuer),
                     new Claim(ClaimTypes.Surname, "Lock", ClaimValueTypes.String, Issuer),
                     new Claim(ClaimTypes.Country, "UK", ClaimValueTypes.String, Issuer),
                     new Claim("ChildhoodHero", "Ronnie James Dio", ClaimValueTypes.String)
            };

            var userIdentity = new ClaimsIdentity(claims, "Passport");

            var userPrincipal = new ClaimsPrincipal(userIdentity);
            HttpContext.User = userPrincipal;

          
            //await HttpContext.Authentication.SignInAsync("Cookie", userPrincipal,
            //    new AuthenticationProperties
            //    {
            //        ExpiresUtc = DateTime.UtcNow.AddMinutes(20),
            //        IsPersistent = false,
            //        AllowRefresh = false
            //    });

            return string.Empty;
        }


    }
}
