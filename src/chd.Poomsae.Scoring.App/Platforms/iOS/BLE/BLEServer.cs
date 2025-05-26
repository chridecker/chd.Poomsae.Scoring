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
using chd.Poomsae.Scoring.App.Extensions;

namespace chd.Poomsae.Scoring.App.Platforms.iOS.BLE
{
    public class BLEServer : BaseBLEServer<CBCentral, CBMutableService, CBMutableCharacteristic, CBMutableDescriptor>, IBroadCastService
    {
        private CBPeripheralManager _cBPeripheralManager;
        private BLEPeripheralManagerDelegate _cBPeripheralManagerDelegate;


        public BLEServer(BLEPeripheralManagerDelegate bLEPeripheralManagerDelegate, ISettingManager settingManager, IModalService modalService)
            : base(settingManager, modalService, new List<byte>() { 0, 0 })
        {
            this._cBPeripheralManagerDelegate = bLEPeripheralManagerDelegate;
            this._cBPeripheralManagerDelegate.ReadRequest += this._cBPeripheralManagerDelegate_ReadRequest;
            this._cBPeripheralManagerDelegate.StateUpdate += this._cb_StateUpdated;
            this._cBPeripheralManagerDelegate.CharacteristicUnsubscribe += this._cBPeripheralManagerDelegate_CharacteristicUnSubscribe;
            this._cBPeripheralManagerDelegate.ServiceAdd += this._cBPeripheralManagerDelegate_ServiceAdd;
        }

        protected override async Task CheckPermissions(CancellationToken cancellationToken)
        {
            var perm = await Permissions.CheckStatusAsync<IosBluetoothPermission>();
            while (perm is not PermissionStatus.Granted)
            {
                _ = await Permissions.RequestAsync<IosBluetoothPermission>();
                _ = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

                perm = await Permissions.CheckStatusAsync<IosBluetoothPermission>();
                await Task.Delay(250, cancellationToken);
                if (cancellationToken.IsCancellationRequested || perm is PermissionStatus.Granted)
                {
                    break;
                }
            }
        }

        protected override void BroadCastToAllDevices(CBMutableCharacteristic characteristic, byte[] value)
        {
            this._cBPeripheralManager.UpdateValue(NSData.FromArray(value), characteristic, this._connectedDevices.Values.ToArray());
        }
        protected override async Task StartNativeAsync(CancellationToken token)
        {
            if (this._cBPeripheralManager is not null) { return; }

            this._cBPeripheralManager = new CBPeripheralManager(this._cBPeripheralManagerDelegate, DispatchQueue.MainQueue);

            if (this._cBPeripheralManager.State is CBManagerState.PoweredOn)
            {
                await this.StartGattServer();
            }
        }

        private async Task StartGattServer()
        {
            try
            {
                this._cBPeripheralManager.StopAdvertising();
                this._cBPeripheralManager.RemoveAllServices();

                var name = await this.GetName();

                this._characteristicName = new CBMutableCharacteristic(CBUUID.FromString(BLEConstants.Result_Characteristic.ToGuidString()), CBCharacteristicProperties.Read | CBCharacteristicProperties.Notify, null, CBAttributePermissions.Readable | CBAttributePermissions.Writeable);

                this._characteristic = new CBMutableCharacteristic(CBUUID.FromString(BLEConstants.Name_Characteristic.ToGuidString()), CBCharacteristicProperties.Notify, null, CBAttributePermissions.Readable | CBAttributePermissions.Writeable);

                this._service = new CBMutableService(CBUUID.FromString(BLEConstants.Result_Gatt_Service.ToString()), true);
                this._service.Characteristics = new[] { this._characteristicName, this._characteristic };

                this._cBPeripheralManager.AddService(this._service);

                this._cBPeripheralManager.StartAdvertising(new StartAdvertisingOptions()
                {
                    LocalName = name.Item1,
                    ServicesUUID = [CBUUID.FromString(BLEConstants.Result_Gatt_Service.ToString())],
                });
            }
            catch (Exception ex)
            {
                await this._modalService.ShowDialog($"Start BLE Gatt with Error '{ex.ToString()}'", EDialogButtons.OK);
            }
        }

        private async void _cb_StateUpdated(object? sender, BLEEventArgs e)
        {
            var peripheralManager = e.Peripheral;
            if (peripheralManager.State is CBManagerState.Unsupported)
            {
                await this._modalService.ShowDialog($"BLE not supported", EDialogButtons.OK);
            }
            else if (peripheralManager.State is CBManagerState.PoweredOn)
            {
                await this.StartGattServer();
            }
            else
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
        }

        private async void _cBPeripheralManagerDelegate_CharacteristicUnSubscribe(object? sender, BLEEventArgs e)
        {
            if (this._connectedDevices.ContainsKey(e.Central.ParseDeviceId())
                && this._connectedDevices.TryRemove(e.Central.ParseDeviceId(), out _))
            {
                this.OnDeviceConnectionChanged(e.Central.ParseDeviceId(), false);
            }
        }

        private async void _cBPeripheralManagerDelegate_ReadRequest(object? sender, BLEEventArgs e)
        {
            try
            {
                var request = e.Request;
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
                        this.OnDeviceConnectionChanged(request.Central.ParseDeviceId(), true);
                    }
                }
            }
            catch (Exception ex)
            {
                await this._modalService.ShowDialog($"Read BLE Request with Error '{ex.ToString()}'", EDialogButtons.OK);
            }
        }


        private async void _cBPeripheralManagerDelegate_ServiceAdd(object? sender, BLEEventArgs e)
        {
            var errorMessgae = string.Empty;
            if (e.Error is not null)
            {
                var sb = new StringBuilder();
                sb.AppendLine("Fehler aufgetreten:");
                sb.AppendLine($"Beschreibung     : {e.Error.LocalizedDescription}");
                sb.AppendLine($"Fehlercode       : {e.Error.Code}");
                sb.AppendLine($"Fehlerdomain     : {e.Error.Domain}");
                sb.AppendLine($"Fehlerursache    : {e.Error.LocalizedFailureReason ?? "Keine Angabe"}");
                sb.AppendLine($"Vorschlag        : {e.Error.LocalizedRecoverySuggestion ?? "Keine Angabe"}");

                if (e.Error.UserInfo is not null && e.Error.UserInfo.Any())
                {
                    sb.AppendLine("Zusätzliche Informationen (UserInfo):");
                    foreach (var key in e.Error.UserInfo.Keys)
                    {
                        var value = e.Error.UserInfo.ObjectForKey(key);
                        sb.AppendLine($"  {key} = {value}");
                    }
                }
                errorMessgae = sb.ToString();
                await this._modalService.ShowDialog($"BLE Service Add'{e.Service.UUID}', {errorMessgae}", EDialogButtons.OK);

            }
        }
    }
}
