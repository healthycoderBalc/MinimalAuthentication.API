using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MinimalAuthentication.API
{
    public class JwtTokenGenerator
    {
        private readonly string _key;

        private readonly IConfiguration _configuration;

        public JwtTokenGenerator(string key, IConfiguration configuration)
        {
            _key = key;
            _configuration = configuration ??
                throw new ArgumentNullException(nameof(configuration));
        }
        public string GenerateToken(string username)
        {
            var key = _configuration["JWTToken:Key"];
            var securityKey = new SymmetricSecurityKey(
                Convert.FromBase64String(key));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claimsForToken = new List<Claim>();
            claimsForToken.Add(new Claim("name", username));
            var jwtSecurityToken = new JwtSecurityToken(
                _configuration["JWTToken:Issuer"],
                _configuration["JWTToken:Audience"],
                claimsForToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddDays(1),
                signingCredentials
                );

            var tokenToReturn = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            return tokenToReturn;
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = _configuration["JWTToken:Key"];
            var securityKey = new SymmetricSecurityKey(
                Convert.FromBase64String(key));
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["JWTToken:Issuer"],
                ValidAudience = _configuration["JWTToken:Audience"],
                IssuerSigningKey = securityKey
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            return principal;
        }

    }
}
