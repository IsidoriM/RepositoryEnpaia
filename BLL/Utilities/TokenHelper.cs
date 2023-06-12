using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace TFI.BLL.Utilities
{
    public static class TokenHelper
    {
        public static string CreateToken(List<Claim> claims, int validityDays = 1)
        {
            var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["jwt_secret_key"]));
            var signingCredentials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha512);
            var jwtSecurityToken = new JwtSecurityToken(claims: claims, notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddDays(validityDays),
                signingCredentials: signingCredentials);

            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            return token;
        }

        public static Dictionary<string, string> ValidateToken(string token, params string[] claimTypes)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["jwt_secret_key"]);
            tokenHandler.ValidateToken(token, new TokenValidationParameters()
            {
                RequireExpirationTime = true,
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var result = claimTypes.ToDictionary(claimType => claimType, 
                claimType => jwtToken.Claims.First(claim => claim.Type == claimType).Value);

            return result;
        }
    }
}
