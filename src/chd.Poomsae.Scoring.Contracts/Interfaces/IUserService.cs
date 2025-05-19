using chd.Poomsae.Scoring.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.Contracts.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<PSUserDto>> GetUsersAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<PSDeviceDto>> GetDevicesAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<PSUserDeviceDto>> GetUserDevicesToUserAsync(string userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<PSUserDeviceDto>> GetUserDevicesToDeviceAsync(string deviceId, CancellationToken cancellationToken = default);

        Task UpdateUserAsync(PSUserDto user, CancellationToken cancellationToken = default);
        Task UpdateDeviceAsync(PSDeviceDto user, CancellationToken cancellationToken = default);
        Task RemoveUserDeviceAsync(string id, CancellationToken cancellationToken = default);
        Task<PSDeviceDto> GetOrCreateDevice();
        Task<PSUserDto> GetOrCreateUser(PSUserDto user);
        Task<PSUserDeviceDto> GetOrCreateUserDevice(string userId, string deviceId, bool isAdmin);
    }
}
