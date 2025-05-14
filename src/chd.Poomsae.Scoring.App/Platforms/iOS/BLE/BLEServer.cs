using chd.Poomsae.Scoring.Contracts.Constants;
using chd.Poomsae.Scoring.Contracts.Dtos;
using chd.Poomsae.Scoring.Contracts.Dtos.Base;
using chd.Poomsae.Scoring.Contracts.Enums;
using chd.Poomsae.Scoring.Contracts.Interfaces;
using CoreBluetooth;
using CoreFoundation;
using Foundation;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.App.Platforms.iOS.BLE
{
    public abstract class BLEServer : CBPeripheralManagerDelegate, IBroadCastService
    {
        private static readonly byte[] DisableNotificationValue = new byte[] { 0, 0 };
        private static readonly byte[] EnableNotificationValue = new byte[] { 0, 0 };

        private static string _restorationIdentifier;

        private CBCentralManager _centralManager;
        private CBPeripheralManager _cBPeripheralManager;
        private IBleCentralManagerDelegate _bleCentralManagerDelegate;
        private ICBPeripheralManagerDelegate _cBPeripheralManagerDelegate;

        private CBMutableService _service;

        private CBMutableCharacteristic _characteristic;
        private CBMutableCharacteristic _characteristicName;

        private CBMutableDescriptor _descNotifyResult;
        private CBMutableDescriptor _descNotifyNameChanged;

        private BluetoothState _state = BluetoothState.Unknown;

        private byte[] _resultNotifyDescValue = DisableNotificationValue;
        private byte[] _nameNotifyDescValue = DisableNotificationValue;
        private byte[] _resultCharacteristicValue = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0];

        private ConcurrentDictionary<Guid, CBCentral> _connectedDevices = [];

        private readonly ISettingManager _settingManager;
        public int ConnectedDevices => this._connectedDevices.Count;
        public event EventHandler<DeviceConnectionChangedEventArgs> DeviceConnectionChanged;

        protected BLEServer(ISettingManager settingManager)
        {
            this._settingManager = settingManager;

            var cmDelegate = new BleCentralManagerDelegate();
            _bleCentralManagerDelegate = cmDelegate;

            var options = CreateInitOptions();

            _centralManager = new CBCentralManager(cmDelegate, DispatchQueue.CurrentQueue, options);
            _bleCentralManagerDelegate.UpdatedState += (s, e) => this._state = GetState();
        }

        public async Task StartAsync(CancellationToken token)
        {
            if (this._state is not BluetoothState.On or BluetoothState.TurningOn)
            {
                //this._centralManager.Sta
            }
            if (this._cBPeripheralManager is not null) { return; }

            this._cBPeripheralManager = new CBPeripheralManager(this, DispatchQueue.CurrentQueue);

            this._cBPeripheralManager.CharacteristicSubscribed += this._cBPeripheralManager_CharacteristicSubscribed;

            var name = await this.GetName();

            this._characteristicName = new CBMutableCharacteristic(CBUUID.FromString(BLEConstants.Name_Characteristic.ToString()), CBCharacteristicProperties.Read | CBCharacteristicProperties.Notify, NSData.FromArray(name.Item2), CBAttributePermissions.Readable | CBAttributePermissions.Writeable);
            this._descNotifyNameChanged = new CBMutableDescriptor(CBUUID.FromString(BLEConstants.Notify_Descriptor.ToString()), NSData.FromArray(this._nameNotifyDescValue));
            this._characteristicName.Descriptors = [this._descNotifyNameChanged];

            this._characteristic = new CBMutableCharacteristic(CBUUID.FromString(BLEConstants.Result_Characteristic.ToString()), CBCharacteristicProperties.Read | CBCharacteristicProperties.Notify, NSData.FromString("Hello BLE!"), CBAttributePermissions.Readable | CBAttributePermissions.Writeable);
            this._descNotifyResult = new CBMutableDescriptor(CBUUID.FromString(BLEConstants.Notify_Descriptor.ToString()), NSData.FromArray(this._resultCharacteristicValue));
            this._characteristic.Descriptors = [this._descNotifyResult];


            this._service = new CBMutableService(CBUUID.FromString(BLEConstants.Result_Gatt_Service.ToString()), true);
            this._service.Characteristics = [this._characteristic, this._characteristicName];

            this._cBPeripheralManager.AddService(this._service);

            this._cBPeripheralManager.StartAdvertising(new StartAdvertisingOptions()
            {
                LocalName = name.Item1,
                ServicesUUID = [CBUUID.FromString(BLEConstants.Result_Gatt_Service.ToString())],
            });

        }

        private void _cBPeripheralManager_CharacteristicSubscribed(object? sender, CBPeripheralManagerSubscriptionEventArgs e)
        {
            if (!this._connectedDevices.ContainsKey(e.Central.ParseDeviceId()))
            {
                this._connectedDevices.TryAdd(e.Central.ParseDeviceId(), e.Central);
                this.DeviceConnectionChanged?.Invoke(this, new DeviceConnectionChangedEventArgs
                {
                    Connected = true,
                    Id = e.Central.ParseDeviceId(),
                    Name = e.Central.Description
                });
            }
        }

        public async Task BroadcastNameChange()
        {
            var name = await this.GetName();
            this.BroadCastToAllDevices(this._characteristicName, name.Item2);
        }
        public void ResetScore()
        {
            this.SetResultValue([0, 0, 0, 0, 0, 0, 0, 0, 0, 0]);
            this.BroadCastToAllDevices(this._characteristic, this._resultCharacteristicValue);
        }

        public void BroadcastResult(RunDto run)
        {
            if (run is EliminationRunDto elimination)
            {
                this.SetResultValue([1, __dataConvert(elimination.ChongScore.Accuracy), __dataConvert(elimination.ChongScore.SpeedAndPower ?? 0m), __dataConvert(elimination.ChongScore.RhythmAndTempo ?? 0m), __dataConvert(elimination.ChongScore.ExpressionAndEnergy ?? 0m), 2, __dataConvert(elimination.HongScore.Accuracy), __dataConvert(elimination.HongScore.SpeedAndPower ?? 0m), __dataConvert(elimination.HongScore.RhythmAndTempo ?? 0m), __dataConvert(elimination.HongScore.ExpressionAndEnergy ?? 0m)]);
                this.BroadCastToAllDevices(this._characteristic, this._resultCharacteristicValue);
            }
            else if (run is SingleRunDto singleRun)
            {
                if (singleRun.Color is EScoringButtonColor.Blue)
                {
                    this.SetResultValue([1, __dataConvert(singleRun.Score.Accuracy), __dataConvert(singleRun.Score.SpeedAndPower ?? 0m), __dataConvert(singleRun.Score.RhythmAndTempo ?? 0m), __dataConvert(singleRun.Score.ExpressionAndEnergy ?? 0m), 0, 0, 0, 0, 0]);
                }
                else if (singleRun.Color is EScoringButtonColor.Red)
                {
                    this.SetResultValue([0, 0, 0, 0, 0, 2, __dataConvert(singleRun.Score.Accuracy), __dataConvert(singleRun.Score.SpeedAndPower ?? 0m), __dataConvert(singleRun.Score.RhythmAndTempo ?? 0m), __dataConvert(singleRun.Score.ExpressionAndEnergy ?? 0m)]);
                }
                this.BroadCastToAllDevices(this._characteristic, this._resultCharacteristicValue);
            }

            byte __dataConvert(decimal d) => (byte)(d * 10);
        }



        public override async void ReadRequestReceived(CBPeripheralManager peripheral, CBATTRequest request)
        {
            base.ReadRequestReceived(peripheral, request);
            if (request.Characteristic.UUID == this._characteristic.UUID)
            {
            }
            else if (request.Characteristic.UUID == this._characteristicName.UUID)
            {
                var name = await this.GetName();
                request.Value = NSData.FromArray(name.Item2);
                peripheral.RespondToRequest(request, CBATTError.Success);
                if (!this._connectedDevices.ContainsKey(request.Central.ParseDeviceId()))
                {
                    this._connectedDevices.TryAdd(request.Central.ParseDeviceId(), request.Central);
                    this.DeviceConnectionChanged?.Invoke(this, new DeviceConnectionChangedEventArgs
                    {
                        Connected = true,
                        Id = request.Central.ParseDeviceId(),
                        Name = request.Central.Description
                    });
                }
            }
        }


        private void BroadCastToAllDevices(CBMutableCharacteristic characteristic, byte[] value)
        {
            this._cBPeripheralManager.UpdateValue(NSData.FromArray(value), characteristic, this._connectedDevices.Values.ToArray());
        }


        private void SetResultValue(byte[] data)
        {
            this._resultCharacteristicValue = data;
        }

        private BluetoothState GetState()
        {
            return _centralManager?.State.ToBluetoothState() ?? BluetoothState.Unavailable;
        }
        private CBCentralInitOptions CreateInitOptions()
        {
            return new CBCentralInitOptions
            {
#if __IOS__
                RestoreIdentifier = _restorationIdentifier,
#endif
                ShowPowerAlert = false
            };
        }

        private async Task<(string, byte[])> GetName()
        {
            var name = await this._settingManager.GetName();
            name = string.IsNullOrWhiteSpace(name) ? DeviceInfo.Current.Name : name;
            return (name, Encoding.ASCII.GetBytes(name));
        }


    }
}
