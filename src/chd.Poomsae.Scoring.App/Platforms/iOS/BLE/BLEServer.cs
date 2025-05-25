using chd.Poomsae.Scoring.Contracts.Constants;
using chd.Poomsae.Scoring.Contracts.Dtos;
using chd.Poomsae.Scoring.Contracts.Dtos.Base;
using chd.Poomsae.Scoring.Contracts.Enums;
using chd.Poomsae.Scoring.Contracts.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blazored.Modal.Services;
using chd.UI.Base.Components.Extensions;
using chd.UI.Base.Contracts.Enum;
using System.Threading;
using Intents;
using chd.Poomsae.Scoring.App.Services.Base;
using CoreBluetooth;
using Plugin.BLE.Abstractions.Contracts;
using UIKit;
using Foundation;
using CoreFoundation;

namespace chd.Poomsae.Scoring.App.Platforms.iOS.BLE
{
    public class BLEServer : BaseBLEServer<CBCentral, CBMutableService, CBMutableCharacteristic, CBMutableDescriptor>, IBroadCastService
    {
        private static readonly byte[] DisableNotificationValue = new byte[] { 0, 0 };
        private static readonly byte[] EnableNotificationValue = new byte[] { 1, 0 };


        private CBPeripheralManager _cBPeripheralManager;
        private BLEPeripheralManagerDelegate _cBPeripheralManagerDelegate;


        public BLEServer(BLEPeripheralManagerDelegate bLEPeripheralManagerDelegate, ISettingManager settingManager, IModalService modalService)
            : base(settingManager, modalService, DisableNotificationValue)
        {
            this._cBPeripheralManagerDelegate = bLEPeripheralManagerDelegate;
            this._cBPeripheralManagerDelegate.ReadRequest += this._cBPeripheralManagerDelegate_ReadRequest;
            this._cBPeripheralManagerDelegate.StateUpdate += this._cb_StateUpdated;
        }


        protected override async Task StartNativeAsync(CancellationToken token)
        {
            if (this._cBPeripheralManager is not null) { return; }

            this._cBPeripheralManager = new CBPeripheralManager(this._cBPeripheralManagerDelegate, DispatchQueue.MainQueue);

        }

        private async void _cb_StateUpdated(object? sender, CBPeripheralManager peripheralManager)
        {
            if (this._cBPeripheralManager.State is CBManagerState.Unsupported)
            {
                await this._modalService.ShowDialog($"BLE not supporter", EDialogButtons.OK);
            }
            else if (peripheralManager.State is not CBManagerState.PoweredOn)
            {
                var res = await this._modalService.ShowDialog("Bluetooth ist nicht aktiviert! Um alle Funktionen nutzen zu können muss der Bluetooth-Dienst aktiviert sein! Jetzt aktivieren?", EDialogButtons.YesNo);
                if (res is not EDialogResult.Yes)
                {
                    return;
                }
                var isSucceded = false;
                UIApplication.SharedApplication.OpenUrl(new NSUrl(UIApplication.OpenSettingsUrlString), new UIApplicationOpenUrlOptions(), (success) =>
                {
                });
            }
            else if (peripheralManager.State is CBManagerState.PoweredOn)
            {
#if DEBUG
                await this._modalService.ShowDialog($"BLE Running", EDialogButtons.OK);
#endif
                this._cBPeripheralManager.CharacteristicSubscribed += this._cBPeripheralManager_CharacteristicSubscribed;
                this._cBPeripheralManager.CharacteristicUnsubscribed += this._cBPeripheralManager_CharacteristicUnSubscribed;

                var name = await this.GetName();

                this._characteristicName = new CBMutableCharacteristic(CBUUID.FromString("99A4"), CBCharacteristicProperties.Read | CBCharacteristicProperties.Notify, null, CBAttributePermissions.Readable | CBAttributePermissions.Writeable);
                this._descNotifyNameChanged = new CBMutableDescriptor(CBUUID.FromString("2902"), NSData.FromArray(this._nameNotifyDescValue));
                this._characteristicName.Descriptors = [this._descNotifyNameChanged];

                this._characteristic = new CBMutableCharacteristic(CBUUID.FromString("99A4"), CBCharacteristicProperties.Read | CBCharacteristicProperties.Notify, null, CBAttributePermissions.Readable | CBAttributePermissions.Writeable);
                this._descNotifyResult = new CBMutableDescriptor(CBUUID.FromString("2902"), NSData.FromArray(this._resultCharacteristicValue));
                this._characteristic.Descriptors = [this._descNotifyResult];

                this._service = new CBMutableService(CBUUID.FromString("A8B9"), true);
                this._service.Characteristics = [this._characteristic, this._characteristicName];

                this._cBPeripheralManager.AddService(this._service);

                this._cBPeripheralManager.StartAdvertising(new StartAdvertisingOptions()
                {
                    LocalName = name.Item1,
                    ServicesUUID = [CBUUID.FromString("A8B9")],
                });

            }
        }

        private async void _cBPeripheralManagerDelegate_ReadRequest(object? sender, CBATTRequest request)
        {
            if (request.Characteristic.UUID == this._characteristic.UUID)
            {
            }
            else if (request.Characteristic.UUID == this._characteristicName.UUID)
            {
                var name = await this.GetName();
                request.Value = NSData.FromArray(name.Item2);
                this._cBPeripheralManager.RespondToRequest(request, CBATTError.Success);
                if (!this._connectedDevices.ContainsKey(request.Central.ParseDeviceId()))
                {
                    this._connectedDevices.TryAdd(request.Central.ParseDeviceId(), request.Central);
                    this.OnDeviceConnectionChanged(request.Central.ParseDeviceId(), request.Central.Description, true);
                }
            }
        }

        private void _cBPeripheralManager_CharacteristicSubscribed(object? sender, CBPeripheralManagerSubscriptionEventArgs e)
        {
            if (!this._connectedDevices.ContainsKey(e.Central.ParseDeviceId()))
            {
                this._connectedDevices.TryAdd(e.Central.ParseDeviceId(), e.Central);
                this.OnDeviceConnectionChanged(e.Central.ParseDeviceId(), e.Central.Description, true);
            }
        }

        private void _cBPeripheralManager_CharacteristicUnSubscribed(object? sender, CBPeripheralManagerSubscriptionEventArgs e)
        {
            if (this._connectedDevices.ContainsKey(e.Central.ParseDeviceId())
                && this._connectedDevices.TryRemove(e.Central.ParseDeviceId(), out _))
            {
                this.OnDeviceConnectionChanged(e.Central.ParseDeviceId(), e.Central.Description, false);
            }
        }

        protected override void BroadCastToAllDevices(CBMutableCharacteristic characteristic, byte[] value)
        {
            this._cBPeripheralManager.UpdateValue(NSData.FromArray(value), characteristic, this._connectedDevices.Values.ToArray());
        }
    }
}
