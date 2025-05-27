using chd.Poomsae.Scoring.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.Contracts.Interfaces
{
    public interface IDataService
    {
        Task<PSDeviceDto> GetOrCreateDevice();
        Task<PSUserDto> GetOrCreateUser(PSUserDto user);
        Task<PSUserDeviceDto> GetOrCreateUserDevice(string userId, string deviceId, bool isAdmin);
    }
}
