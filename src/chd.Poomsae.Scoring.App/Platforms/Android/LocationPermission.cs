﻿using Android;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.Platforms.Android
{
    public class LocationPermission : Permissions.BasePlatformPermission
    {
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions
        {
            get
            {
                var result = new List<(string androidPermission, bool isRuntime)>();
                if (OperatingSystem.IsAndroidVersionAtLeast(33))
                    result.Add((Manifest.Permission.AccessCoarseLocation, true));
                result.Add((Manifest.Permission.AccessFineLocation, true));
                result.Add((Manifest.Permission.WakeLock, true));
                return result.ToArray();
            }
        }
    }
}
