using chd.Poomsae.Scoring.Contracts.Constants;
using chd.Poomsae.Scoring.Contracts.Dtos;
using chd.Poomsae.Scoring.Contracts.Settings;
using chd.UI.Base.Client.Implementations.Authorization;
using chd.UI.Base.Contracts.Dtos.Authentication;
using Microsoft.Extensions.Options;
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
    public class LicenseTokenProfileService : ProfileService<Guid, int>
    {
        private const string KEY = "hopefullymkeyislongenought123456";
        private readonly IOptionsMonitor<LicenseSettings> _optionsMonitor;

        public LicenseTokenProfileService(IOptionsMonitor<LicenseSettings> optionsMonitor)
        {
            this._optionsMonitor = optionsMonitor;
        }

        protected override async Task<UserPermissionDto<int>> GetPermissions(UserDto<Guid, int> dto, CancellationToken cancellationToken = default)
        {
            var perm = new UserPermissionDto<int>();
            var lst = new List<UserRightDto<int>>();
            if (dto is PSUserDto psUser)
            {
                if (psUser.IsAdmin)
                {
                    lst.Add(new UserRightDto<int>()
                    {
                        Id = RightConstants.IS_ADMIN,
                        Name = "Admin"
                    });

                    lst.Add(new UserRightDto<int>()
                    {
                        Id = RightConstants.IS_ALLOWED,
                        Name = "Allowed"
                    });
                }
                else if (psUser.HasLicense || (psUser.ValidTo > DateTime.Now))
                {
                    lst.Add(new UserRightDto<int>()
                    {
                        Id = RightConstants.IS_ALLOWED,
                        Name = "Allowed"
                    });
                }
            }


            perm.UserRightLst = lst;
            return perm;
        }

        protected override Task<UserDto<Guid, int>> GetUser(LoginDto<Guid> dto, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        private string GenerateLicenseToken(PSUserDto user, DateTime expiryDate)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim("user", JsonSerializer.Serialize(user)),
            new Claim("license_end", expiryDate.ToString("yyyy-MM-dd"))
        };

            var token = new JwtSecurityToken(
                issuer: "chdscoring",
                audience: "chd.poomsae.scoring",
                claims: claims,
                expires: expiryDate,
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private (PSUserDto, DateTime) ValidateLicenseToken(string token)
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
                var licenseEnd = jwtToken.Claims.First(x => x.Type == "license_end").Value;
                var user = jwtToken.Claims.First(x => x.Type == "user").Value;
                return (JsonSerializer.Deserialize<PSUserDto>(user), DateTime.Parse(licenseEnd));
            }
            catch
            {
                return (null, DateTime.MinValue);
            }
        }
    }
}
