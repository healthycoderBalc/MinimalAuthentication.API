using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinimalAuthentication.API.Models;

namespace MinimalAuthentication.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly JwtTokenGenerator _jwtTokenGenerator;

        public AuthenticationController(JwtTokenGenerator jwtTokenGenerator)
        {
            _jwtTokenGenerator = jwtTokenGenerator;

        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            bool userValidated = ValidateUserCredentials(model.Username, model.Password);
            if (!userValidated)
            {
                return Unauthorized();
            }

            var token = _jwtTokenGenerator.GenerateToken(model.Username);
            return Ok(token);
        }

        [Authorize]
        [HttpGet("weather")]
        public IActionResult GetWeather()
        {
            return Ok("Welcome! Weather today is: 5ºC and Sunny.");
        }


        private bool ValidateUserCredentials(string username, string password)
        {
            return username == "testusername" && password == "testpassword";
        }


    }
}
