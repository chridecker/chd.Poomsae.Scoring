using Blazored.Modal.Services;
using chd.Poomsae.Scoring.App.Services;
using chd.Poomsae.Scoring.Contracts.Dtos;
using chd.Poomsae.Scoring.Contracts.Interfaces;
using chd.Poomsae.Scoring.UI.Services;
using chd.UI.Base.Components.Extensions;
using chd.UI.Base.Contracts.Dtos.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.App.Platforms.iOS.Authentication
{
    public class AppleSignInManager : LicenseTokenProfileService
    {
        public AppleSignInManager(IModalService modalService, ISettingManager settingManager, ITokenService tokenService) : base(settingManager, tokenService)
        {
        }

        protected override async Task<PSDeviceDto> GetDevice(CancellationToken cancellationToken)
        {
            return new PSDeviceDto()
            {
                UID = "test"
            };
        }
        protected override async Task<PSUserDto> SignIn(CancellationToken cancellationToken)
        {
            return new PSUserDto()
            {
                Email = "test",
                UID = "test",
                Username = "test",
                FirstName = "test",
                LastName = "test",
            };

        }
    }
}
