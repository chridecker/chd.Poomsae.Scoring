﻿using Android;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.Platforms.Android
{
    public class BluetoothPermission : Permissions.BasePlatformPermission
    {
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions
        {
            get
            {
                var result = new List<(string androidPermission, bool isRuntime)>();
                if (OperatingSystem.IsAndroidVersionAtLeast(33))
                    result.Add((Manifest.Permission.Bluetooth, true));
                result.Add((Manifest.Permission.BluetoothAdmin, true));
                result.Add((Manifest.Permission.BluetoothAdvertise, true));
                result.Add((Manifest.Permission.BluetoothConnect, true));
                result.Add((Manifest.Permission.BluetoothScan, true));
                return result.ToArray();
            }
        }
    }
}
