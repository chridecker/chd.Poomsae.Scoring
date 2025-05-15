using chd.Poomsae.Scoring.Contracts.Interfaces;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using chd.Poomsae.Scoring.Contracts.Dtos;
using chd.Poomsae.Scoring.Contracts.Dtos.Base;
using chd.Poomsae.Scoring.Contracts.Enums;
using Blazored.Modal.Services;

namespace chd.Poomsae.Scoring.App.Services.BLE.Base
{
    public abstract class BaseBLEServer<TDevice, TService, TCharacteristic, TDescriptor>
    {
        protected readonly ISettingManager _settingManager;
        protected readonly IModalService _modalService;

        protected IBluetoothLE _bluetoothLE => CrossBluetoothLE.Current;

        protected byte[] _resultNotifyDescValue;
        protected byte[] _nameNotifyDescValue;
        protected byte[] _resultCharacteristicValue = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0];


        protected TService _service;
        protected TCharacteristic _characteristic;
        protected TCharacteristic _characteristicName;

        protected TDescriptor _descNotifyResult;
        protected TDescriptor _descNotifyNameChanged;

        protected ConcurrentDictionary<Guid, TDevice> _connectedDevices = [];

        public int ConnectedDevices => this._connectedDevices.Count;
        public event EventHandler<DeviceConnectionChangedEventArgs> DeviceConnectionChanged;

        protected abstract void BroadCastToAllDevices(TCharacteristic characteristic, byte[] data);
        protected abstract Task StartNativeAsync(CancellationToken cancellationToken);


        protected BaseBLEServer(ISettingManager settingManager,IModalService modalService, IList<byte> disableNotificationValue)
        {
            this._settingManager = settingManager;
            this._modalService = modalService;
            this._resultNotifyDescValue = disableNotificationValue.ToArray();
            this._nameNotifyDescValue = disableNotificationValue.ToArray();
        }

        public async Task StartAsync(CancellationToken token)
        {
            var perm = await Permissions.CheckStatusAsync<Permissions.Bluetooth>();
            while (perm is not PermissionStatus.Granted)
            {
                _ = await Permissions.RequestAsync<Permissions.Bluetooth>();
                _ = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

                perm = await Permissions.CheckStatusAsync<Permissions.Bluetooth>();
                await Task.Delay(250, token);
                if (token.IsCancellationRequested || perm is PermissionStatus.Granted)
                {
                    break;
                }
            }
            await this.StartNativeAsync(token);
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

        protected void OnDeviceConnectionChanged(Guid id, string name, bool connected)
        {
            this.DeviceConnectionChanged?.Invoke(this, new DeviceConnectionChangedEventArgs
            {
                Connected = connected,
                Id = id,
                Name = name
            });
        }

        protected async Task<(string, byte[])> GetName()
        {
            var name = await this._settingManager.GetName();
            name = string.IsNullOrWhiteSpace(name) ? DeviceInfo.Current.Name : name;
            return (name, Encoding.ASCII.GetBytes(name));
        }

        protected void SetResultValue(byte[] data)
        {
            this._resultCharacteristicValue = data;
        }
    }
}
