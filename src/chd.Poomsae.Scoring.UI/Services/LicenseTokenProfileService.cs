using chd.Poomsae.Scoring.Contracts.Constants;
using chd.Poomsae.Scoring.Contracts.Dtos;
using chd.Poomsae.Scoring.Contracts.Interfaces;
using chd.UI.Base.Client.Implementations.Authorization;
using chd.UI.Base.Contracts.Dtos.Authentication;
using DocumentFormat.OpenXml.Office2010.Drawing;
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
    public abstract class LicenseTokenProfileService : ProfileService<Guid, int>, ILicenseTokenProfileService
    {
        private const int ValidDays = 7;
        private readonly ISettingManager _settingManager;
        private readonly ITokenService _tokenService;
        private PSUserDto _userDto;
        private DateTime? _lastLogin;

        public LicenseTokenProfileService(ISettingManager settingManager, ITokenService tokenService)
        {
            this._settingManager = settingManager;
            this._tokenService = tokenService;
        }

        public async Task RenewLicense(CancellationToken cancellationToken = default)
        {
            await this._settingManager.SetToken(string.Empty);
            await this.LogoutAsync(cancellationToken);
            await this.LoginAsync(new(), cancellationToken);
        }

        public async Task<(PSUserDto, DateTime)> GetLicense(CancellationToken cancellationToken = default)
        {
            var token = await this._settingManager.GetToken();
            return this._tokenService.ValidateLicenseToken(token);
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

        protected abstract Task<PSUserDto> SignIn(CancellationToken cancellationToken);

        protected override sealed async Task<UserDto<Guid, int>> GetUser(LoginDto<Guid> dto, CancellationToken cancellationToken = default)
        {
            var time = DateTime.Today;
            if (this._userDto is null)
            {
                var (user, validTo) = await this.GetUserFromToken();
                if (user is not null && validTo > time)
                {
                    this._userDto = user;
                }
                else
                {
                    user = await this.SignIn(cancellationToken);
                    if (user is not null)
                    {
                        await this.GenerateToken(user, time.AddDays(ValidDays));
                        this._userDto = user;
                    }
                }
                this._lastLogin = time;

            }
            return this._userDto;
        }

        private async Task<(PSUserDto, DateTime)> GetUserFromToken()
        {
            var token = await this._settingManager.GetToken();
            if (string.IsNullOrWhiteSpace(token)) { return (null, DateTime.MinValue); }
            return this._tokenService.ValidateLicenseToken(token);
        }

        private async Task GenerateToken(PSUserDto user, DateTime expiryDate)
        {
            var token = this._tokenService.GenerateLicenseToken(user, expiryDate);
            await this._settingManager.SetToken(token);
        }


    }
}
