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

namespace chd.Poomsae.Scoring.App.Platforms.iOS.BLE
{
    public class BLEServer : IBroadCastService
    {
        public int ConnectedDevices => 0;

        public event EventHandler<DeviceConnectionChangedEventArgs> DeviceConnectionChanged;

        public Task BroadcastNameChange()
        {
            return Task.CompletedTask;
        }

        public void BroadcastResult(RunDto dto)
        {
        }

        public void ResetScore()
        {
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
