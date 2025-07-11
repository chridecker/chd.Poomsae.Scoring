﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.Contracts.Interfaces
{
    public interface IDeviceHandler
    {
        bool IsiOS{get; }
        string UID { get; }
        string CurrentAppName { get; }
        string CurrentAppPackageName { get; }
        Version CurrentVersion { get; }
        int PlatformVersionId { get; }
        string Platform { get; }
        string Manufacturer { get; }
        string Model { get; }
        string Name { get; }
        void CloseApp();

        Task RequestLandscape();
        Task ResetOrientation();
        Task ShowToast(string message, CancellationToken cancellationToken = default);
    }
}
