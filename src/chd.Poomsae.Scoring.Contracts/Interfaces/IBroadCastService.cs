﻿using chd.Poomsae.Scoring.Contracts.Dtos;
using chd.Poomsae.Scoring.Contracts.Dtos.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.Contracts.Interfaces
{
    public interface IBroadCastService
    {
        event EventHandler<string> RedNameReceived;
        event EventHandler<string> BlueNameReceived;
        event EventHandler<DeviceConnectionChangedEventArgs> DeviceConnectionChanged;
        int ConnectedDevices { get; }
        string BlueName { get; set; }
        string RedName { get; set; }

        Task StartAsync(CancellationToken cancellationToken);
        void ResetScore();
        void BroadcastResult(RunDto dto);
        Task BroadcastNameChange();
    }
}
