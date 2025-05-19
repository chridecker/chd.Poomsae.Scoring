using chd.Poomsae.Scoring.Contracts.Dtos;
using chd.Poomsae.Scoring.Contracts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.WPF.Services
{
    public class UserService : IUserService
    {
        public Task<IEnumerable<PSDeviceDto>> GetDevicesAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<PSDeviceDto> GetOrCreateDevice()
        {
            throw new NotImplementedException();
        }

        public Task<PSUserDto> GetOrCreateUser(PSUserDto user)
        {
            throw new NotImplementedException();
        }

        public Task<PSUserDeviceDto> GetOrCreateUserDevice(string userId, string deviceId, bool isAdmin)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PSUserDeviceDto>> GetUserDevicesToDeviceAsync(string deviceId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PSUserDeviceDto>> GetUserDevicesToUserAsync(string userId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PSUserDto>> GetUsersAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task RemoveUserDeviceAsync(string id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task UpdateDeviceAsync(PSDeviceDto user, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task UpdateUserAsync(PSUserDto user, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
