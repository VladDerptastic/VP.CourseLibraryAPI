using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using VP.CourseLibrary.API.DtoModels;
using VP.CourseLibrary.API.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VP.CourseLibrary.API.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public AccountsController(IUserRepository userRepo)
        {
            _userRepository = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
        }

        // GET: api/<AccountsController>
        [HttpGet]
        public IActionResult Get(string returnUrl = "/authors")
        {
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleLoginCallback"),
                Items =
                {
                    { "returnUrl", returnUrl}
                }
            };
            return Challenge(props, GoogleDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> GoogleLoginCallback() 
        {
            var result = await HttpContext.AuthenticateAsync(
                ExternalAuthenticationDefaults.AuthenticationScheme);

            var externalClaims = result.Principal.Claims.ToList();

            var subjectIdClaim = externalClaims.FirstOrDefault(
                x => x.Type == ClaimTypes.NameIdentifier);
            var subjectValue = subjectIdClaim.Value;

            var user = _userRepository.GetByGoogleId(subjectValue);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("GoogleID", user.GoogleId)
            };

            var identity = new ClaimsIdentity(claims,
                CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // delete temporary cookie used during google authentication
            await HttpContext.SignOutAsync(
                ExternalAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return LocalRedirect(result.Properties.Items["returnUrl"]);
        }

        // GET api/<AccountsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<AccountsController>
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto credentials)
        {
            var user = _userRepository.GetByUsernameAndPassword(credentials.Username, credentials.Password);
            if (user == null)
                return Unauthorized();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("GoogleID", user.GoogleId) //custom claim type
            };

            var authScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            var identity = new ClaimsIdentity(claims, authScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(authScheme, principal,
                new AuthenticationProperties
                {
                    //if we want the cookie to be session only, IsPersistent should remain false
                    IsPersistent = credentials.RememberMe,
                    ExpiresUtc = DateTime.UtcNow.AddDays(4)
                });

            //in a 'normal' webapp, we'd usually redirect people (with LocalRedirect)
            return Ok($"Welcome {user.Name}");
        }

        //This would be used in a 'normal' webapp with webpages
        //public async Task<IActionResult> Logout()
        //{
        //    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        //    return LocalRedirect("/"); //base of the application
        //}

        // PUT api/<AccountsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<AccountsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
