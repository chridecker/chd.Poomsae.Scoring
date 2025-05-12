using Android;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.Platforms.Android
{
    public class InternetPermission : Permissions.BasePlatformPermission
    {
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions
        {
            get
            {
                var result = new List<(string androidPermission, bool isRuntime)>();
                result.Add((Manifest.Permission.Internet, true));
                return result.ToArray();
            }
        }
    }
}
