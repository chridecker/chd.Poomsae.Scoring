using Android.DeviceLock;
using chd.Poomsae.Scoring.App.Extensions;
using chd.Poomsae.Scoring.Contracts.Dtos;
using chd.Poomsae.Scoring.Contracts.Interfaces;
using chd.Poomsae.Scoring.UI.Components.Pages;
using chd.UI.Base.Contracts.Interfaces.Update;
using chd.UI.Base.Extensions;
using DocumentFormat.OpenXml.Spreadsheet;
using Firebase.Annotations;
using Firebase.Firestore;
using Firebase.Firestore.Auth;
using Plugin.Firebase.Firestore;
using Plugin.Firebase.Firestore.Platforms.Android;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.App.Services
{
    public class FirestoreManager : IUserService
    {
        private readonly IFirebaseFirestore _firebaseFirestore;
        private readonly IDeviceHandler _deviceHandler;
        private readonly IUpdateService _updateService;

        public FirestoreManager(IFirebaseFirestore firebaseFirestore, IDeviceHandler deviceHandler, IUpdateService updateService)
        {
            this._firebaseFirestore = firebaseFirestore;
            this._deviceHandler = deviceHandler;
            this._updateService = updateService;
        }

        public async Task<IEnumerable<PSDeviceDto>> GetDevicesAsync(CancellationToken cancellationToken = default)
        {
            var deviceCollection = this._firebaseFirestore.GetCollection("devices");
            var devices = await deviceCollection.GetDocumentsAsync<FireStoreDeviceDto>(Plugin.Firebase.Firestore.Source.Server);
            return devices.Documents.Select(s => s.Data.ToPSDevice());
        }

        public async Task<PSDeviceDto> GetOrCreateDevice()
        {
            var deviceCollection = this._firebaseFirestore.GetCollection("devices");
            var deviceDocument = deviceCollection.GetDocument(this._deviceHandler.UID);

            var snap = await deviceDocument.GetDocumentSnapshotAsync<FireStoreDeviceDto>(Plugin.Firebase.Firestore.Source.Server);

            var version = await this._updateService.CurrentVersion();

            if (snap?.Data is null || string.IsNullOrEmpty(snap.Data.UID))
            {
                var deviceDto = new PSDeviceDto()
                {
                    UID = this._deviceHandler.UID,
                    CurrentVersion = version.ToString(),
                    Manufacturer = this._deviceHandler.Manufacturer,
                    Model = this._deviceHandler.Model,
                    Name = this._deviceHandler.Name,
                    Platform = this._deviceHandler.Platform,
                };
                await snap.Reference.SetDataAsync(deviceDto.ToFSDevice());
                return deviceDto;
            }
            else if (snap.Data.CurrentVersion != version.ToString())
            {
                snap.Data.CurrentVersion = version.ToString();
                await snap.Reference.SetDataAsync(snap.Data);
            }
            return snap.Data.ToPSDevice();
        }


        public async Task<PSUserDto> GetOrCreateUser(PSUserDto user)
        {
            var userCollection = this._firebaseFirestore.GetCollection("users");
            var userDocument = userCollection.GetDocument(user.UID);

            var snap = await userDocument.GetDocumentSnapshotAsync<FireStoreUserDto>(Plugin.Firebase.Firestore.Source.Server);

            if (snap?.Data is null || string.IsNullOrEmpty(snap.Data.UID))
            {
                await snap.Reference.SetDataAsync(user.ToFSUser());
                return user;
            }
            return snap.Data.ToPSUser();
        }


        public async Task<PSUserDeviceDto> GetOrCreateUserDevice(string userId, string deviceId, bool isAdmin)
        {
            var userDeviceCollection = this._firebaseFirestore.GetCollection("user_devices");
            var userDeviceDocuments = await userDeviceCollection.GetDocumentsAsync<FireStoreUserDeviceDto>(Plugin.Firebase.Firestore.Source.Server);

            var userDeviceDocumentDatas = userDeviceDocuments.Documents.FirstOrDefault(x => x.Data is not null && x.Data.Device_UID == this._deviceHandler.UID && x.Data.User_UID == userId);

            if (userDeviceDocumentDatas is null)
            {
                var deviceDto = new PSUserDeviceDto()
                {
                    Device_UID = this._deviceHandler.UID,
                    IsAllowed = isAdmin,
                    User_UID = userId,
                };
                var d = await userDeviceCollection.AddDocumentAsync(deviceDto.ToFSUserDevice());
                deviceDto.Id = d.Id;
                return deviceDto;
            }
            return userDeviceDocumentDatas.Data.ToPSUserDevice();
        }

        public async Task<IEnumerable<PSUserDeviceDto>> GetUserDevicesToDeviceAsync(string deviceId, CancellationToken cancellationToken = default)
        {
            var userDeviceCollection = this._firebaseFirestore.GetCollection("user_devices");
            var userDeviceDocuments = await userDeviceCollection.GetDocumentsAsync<FireStoreUserDeviceDto>(Plugin.Firebase.Firestore.Source.Server);

            var userDeviceDocumentDatas = userDeviceDocuments.Documents.Where(x => x.Data is not null && x.Data.Device_UID == deviceId);
            return userDeviceDocumentDatas.Select(s => s.Data.ToPSUserDevice());
        }

        public async Task<IEnumerable<PSUserDeviceDto>> GetUserDevicesToUserAsync(string userId, CancellationToken cancellationToken = default)
        {
            var userDeviceCollection = this._firebaseFirestore.GetCollection("user_devices");
            var userDeviceDocuments = await userDeviceCollection.GetDocumentsAsync<FireStoreUserDeviceDto>(Plugin.Firebase.Firestore.Source.Server);

            var userDeviceDocumentDatas = userDeviceDocuments.Documents.Where(x => x.Data is not null && x.Data.User_UID == userId);
            return userDeviceDocumentDatas.Select(s => s.Data.ToPSUserDevice());
        }

        public async Task<IEnumerable<PSUserDto>> GetUsersAsync(CancellationToken cancellationToken = default)
        {
            var deviceCollection = this._firebaseFirestore.GetCollection("users");
            var devices = await deviceCollection.GetDocumentsAsync<FireStoreUserDto>(Plugin.Firebase.Firestore.Source.Server);
            return devices.Documents.Select(s => s.Data.ToPSUser());
        }

        public async Task RemoveUserDeviceAsync(string id, CancellationToken cancellationToken = default)
        {
            var userCollection = this._firebaseFirestore.GetCollection("user_devices");
            var userDocument = userCollection.GetDocument(id);
            await userDocument.DeleteDocumentAsync();
        }

        public async Task UpdateDeviceAsync(PSDeviceDto device, CancellationToken cancellationToken = default)
        {

            var userCollection = this._firebaseFirestore.GetCollection("devices");
            var userDocument = userCollection.GetDocument(device.UID);
            var snap = await userDocument.GetDocumentSnapshotAsync<FireStoreDeviceDto>(Plugin.Firebase.Firestore.Source.Server);
            await snap.Reference.SetDataAsync(device.ToFSDevice());
        }

        public async Task UpdateUserAsync(PSUserDto user, CancellationToken cancellationToken = default)
        {
            var userCollection = this._firebaseFirestore.GetCollection("users");
            var userDocument = userCollection.GetDocument(user.UID);
            var snap = await userDocument.GetDocumentSnapshotAsync<FireStoreUserDto>(Plugin.Firebase.Firestore.Source.Server);
            await snap.Reference.SetDataAsync(user.ToFSUser());
        }
    }
}
