﻿using chd.Poomsae.Scoring.App.Platforms.Android.Data;
using chd.Poomsae.Scoring.Contracts.Dtos;
using chd.Poomsae.Scoring.Contracts.Interfaces;
using chd.UI.Base.Contracts.Interfaces.Update;
using Plugin.Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.App.Platforms.Android.Authentication
{
    public class FireStoreHandler : IDataService
    {
        private readonly IFirebaseFirestore _firebaseFirestore;
        private readonly IDeviceHandler _deviceHandler;
        private readonly IUpdateService _updateService;

        public FireStoreHandler(IFirebaseFirestore firebaseFirestore, IDeviceHandler deviceHandler, IUpdateService updateService)
        {
            this._firebaseFirestore = firebaseFirestore;
            this._deviceHandler = deviceHandler;
            this._updateService = updateService;
        }

        public async Task<PSDeviceDto> GetOrCreateDevice()
        {
            var deviceCollection = this._firebaseFirestore.GetCollection("devices");
            var deviceDocument = deviceCollection.GetDocument(this._deviceHandler.UID);

            var snap = await deviceDocument.GetDocumentSnapshotAsync<FireStoreDeviceDto>(Source.Server);

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
                    LastStart = DateTimeOffset.Now,
                    Comment = string.Empty,
                };
                await snap.Reference.SetDataAsync(deviceDto.ToFSDevice());
                return deviceDto;
            }
            else
            {
                var device = snap.Data;
                device.CurrentVersion = version.ToString();
                device.LastStart = DateTimeOffset.Now;
                await snap.Reference.SetDataAsync(device);
                return device.ToPSDevice();
            }
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
                    Created = DateTimeOffset.Now
                };
                var d = await userDeviceCollection.AddDocumentAsync(deviceDto.ToFSUserDevice());
                deviceDto.Id = d.Id;
                return deviceDto;
            }
            return userDeviceDocumentDatas.Data.ToPSUserDevice();
        }
    }
}
