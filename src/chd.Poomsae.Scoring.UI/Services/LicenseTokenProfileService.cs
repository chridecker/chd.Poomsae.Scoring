using Blazored.Modal.Services;
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
        private readonly ISettingManager _settingManager;
        private readonly ITokenService _tokenService;
        protected readonly IModalService _modalService;
        protected PSUserDto _userDto;
        private PSDeviceDto _deviceDto;

        public PSDeviceDto Device => this._deviceDto;

        public LicenseTokenProfileService(ISettingManager settingManager, ITokenService tokenService, IModalService modalService)
        {
            this._settingManager = settingManager;
            this._tokenService = tokenService;
            this._modalService = modalService;
        }

        public virtual async Task RenewLicense(CancellationToken cancellationToken = default)
        {
            await this._settingManager.SetToken(string.Empty);
            this._userDto = null;
            await this.LogoutAsync(cancellationToken);
            await this.LoginAsync(new(), cancellationToken);
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

                    lst.Add(new UserRightDto<int>()
                    {
                        Id = RightConstants.HAS_FIGHTERS,
                        Name = "Has Fighters"
                    });
                }
                else if ((psUser.HasLicense || (psUser.ValidTo > DateTime.Now)) && psUser.UserDevice.IsAllowed)
                {
                    lst.Add(new UserRightDto<int>()
                    {
                        Id = RightConstants.IS_ALLOWED,
                        Name = "Allowed"
                    });
                    if (psUser.UserDevice?.HasFighters ?? false)
                    {
                        lst.Add(new UserRightDto<int>()
                        {
                            Id = RightConstants.HAS_FIGHTERS,
                            Name = "Has Fighters"
                        });
                    }
                }
                else
                {
                    lst.Add(new UserRightDto<int>()
                    {
                        Id = RightConstants.IS_ALLOWED,
                        Name = "Allowed"
                    });
                    if (psUser.UserDevice?.HasFighters ?? false)
                    {
                        lst.Add(new UserRightDto<int>()
                        {
                            Id = RightConstants.HAS_FIGHTERS,
                            Name = "Has Fighters"
                        });
                    }
                }
            }


            perm.UserRightLst = lst;
            return perm;
        }

        protected abstract Task<PSUserDto> SignIn(CancellationToken cancellationToken);
        protected abstract Task<PSDeviceDto> GetDevice(CancellationToken cancellationToken);

        protected override sealed async Task<UserDto<Guid, int>> GetUser(LoginDto<Guid> dto, CancellationToken cancellationToken = default)
        {
            var time = DateTime.Today;
            this._deviceDto = await this.GetDevice(cancellationToken);

            if (this._userDto is null)
            {
                var user = await this.GetUserFromToken();
                if (user is not null && user.UserDevice is not null && user.ValidTo.Date > time)
                {
                    this._userDto = user;
                }
                else
                {
                    user = await this.SignIn(cancellationToken);
                    if (user is not null)
                    {
                        await this.GenerateToken(user, user.ValidTo.Date);
                        this._userDto = user;
                    }
                }
            }
            return this._userDto;
        }

        private async Task<PSUserDto> GetUserFromToken()
        {
            var token = await this._settingManager.GetToken();
            if (string.IsNullOrWhiteSpace(token)) { return null; }
            return this._tokenService.ValidateLicenseToken(token);
        }

        private async Task GenerateToken(PSUserDto user, DateTime expiryDate)
        {
            var token = this._tokenService.GenerateLicenseToken(user, expiryDate);
            await this._settingManager.SetToken(token);
        }


    }
}
