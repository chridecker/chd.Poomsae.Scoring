using chd.Poomsae.Scoring.Contracts.Dtos;
using chd.Poomsae.Scoring.Contracts.Settings;
using chd.Poomsae.Scoring.UI.Services;
using chd.UI.Base.Client.Implementations.Authorization;
using chd.UI.Base.Contracts.Dtos.Authentication;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.WPF.Services
{
    public class WindowsProfileService : LicenseTokenProfileService
    {
        public WindowsProfileService(IOptionsMonitor<LicenseSettings> optionsMonitor) : base(optionsMonitor)
        {
        }

        protected override Task<UserPermissionDto<int>> GetPermissions(UserDto<Guid, int> dto, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new UserPermissionDto<int>());
        }

        protected override async Task<UserDto<Guid, int>> GetUser(LoginDto<Guid> dto, CancellationToken cancellationToken = default)
        {
            return new PSUserDto
            {
                Email = "c.decker@metalldeutsch.com",
                Username = "decker",
            };
        }
    }
}
