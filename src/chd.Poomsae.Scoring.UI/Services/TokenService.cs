using chd.Poomsae.Scoring.Contracts.Dtos;
using chd.Poomsae.Scoring.Contracts.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.UI.Services
{
    public class TokenService : ITokenService
    {
        private const string KEY = "hopefullymyKEYislongenougth123456789#!";

        public string GenerateLicenseToken(PSUserDto user, DateTime expiryDate)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim("user", JsonSerializer.Serialize(user)),
        };

            var token = new JwtSecurityToken(
                issuer: "chdscoring",
                audience: "chd.poomsae.scoring",
                claims: claims,
                expires: expiryDate > DateTime.Today.AddDays(7) ? DateTime.Today.AddDays(7) : expiryDate,
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public PSUserDto ValidateLicenseToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(KEY);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = "chdscoring",
                    ValidAudience = "chd.poomsae.scoring",
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var user = jwtToken.Claims.First(x => x.Type == "user").Value;
                return JsonSerializer.Deserialize<PSUserDto>(user);
            }
            catch
            {
                return null;
            }
        }
    }
}
