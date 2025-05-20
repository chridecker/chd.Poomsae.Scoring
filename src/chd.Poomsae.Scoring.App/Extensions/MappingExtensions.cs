using chd.Poomsae.Scoring.App.Services;
using chd.Poomsae.Scoring.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.App.Extensions
{
    public static class MappingExtensions
    {
        public static PSUserDto ToPSUser(this FireStoreUserDto dto)
            => new PSUserDto()
            {
                Email = dto.Email,
                UID = dto.UID,
                IsAdmin = dto.IsAdmin,
                HasLicense = dto.HasLicense,
                ValidTo = dto.ValidTo,
                Username = dto.Username,
                FirstName = dto.Username.Split(' ')[0],
                LastName = dto.Username.Split(' ').Length > 1 ? dto.Username.Split(" ")[1] : string.Empty,
            };

        public static FireStoreUserDto ToFSUser(this PSUserDto dto)
            => new FireStoreUserDto()
            {
                Email = dto.Email,
                UID = dto.UID,
                IsAdmin = dto.IsAdmin,
                HasLicense = dto.HasLicense,
                ValidTo = dto.ValidTo,
                Username = dto.Username ?? string.Empty
            };

        public static PSUserDeviceDto ToPSUserDevice(this FireStoreUserDeviceDto dto)
           => new PSUserDeviceDto()
           {
               Id = dto.Id,
               Device_UID = dto.Device_UID,
               IsAllowed = dto.IsAllowed,
               User_UID = dto.User_UID
           };
        public static FireStoreUserDeviceDto ToFSUserDevice(this PSUserDeviceDto dto)
           => new FireStoreUserDeviceDto()
           {
               Id = dto.Id,
               Device_UID = dto.Device_UID,
               IsAllowed = dto.IsAllowed,
               User_UID = dto.User_UID
           };

        public static PSDeviceDto ToPSDevice(this FireStoreDeviceDto dto)
           => new PSDeviceDto()
           {
               UID = dto.UID,
               CurrentVersion = dto.CurrentVersion,
               Manufacturer = dto.Manufacturer,
               Model = dto.Model,
               Name = dto.Name,
               Platform = dto.Platform,
           };
        public static FireStoreDeviceDto ToFSDevice(this PSDeviceDto dto)
           => new FireStoreDeviceDto()
           {
               UID = dto.UID,
               CurrentVersion = dto.CurrentVersion,
               Manufacturer = dto.Manufacturer,
               Model = dto.Model,
               Name = dto.Name,
               Platform = dto.Platform,
           };
    }
}
