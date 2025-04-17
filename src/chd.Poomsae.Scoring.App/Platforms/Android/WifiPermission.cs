using Android;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.Platforms.Android
{
    public class WifiPermission : Permissions.BasePlatformPermission
    {
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions
        {
            get
            {
                var result = new List<(string androidPermission, bool isRuntime)>();
                if (OperatingSystem.IsAndroidVersionAtLeast(33))
                    result.Add((Manifest.Permission.AccessWifiState, true));
                result.Add((Manifest.Permission.ChangeWifiState, true));
                result.Add((Manifest.Permission.AccessNetworkState, true));
                return result.ToArray();
            }
        }
    }
}
