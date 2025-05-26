using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreBluetooth;
using System.Diagnostics.CodeAnalysis;

namespace chd.Poomsae.Scoring.App.Platforms.iOS
{

    /// <summary>
    /// Checks the Bluetooth Permissions on IOS devices.
    /// </summary>
    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
    public class IosBluetoothPermission : Permissions.BasePlatformPermission
    {
        /// <inheritdoc/>
        public override Task<PermissionStatus> CheckStatusAsync()
        {
            EnsureDeclared();

            return Task.FromResult(GetBluetoothStatus());
        }

        PermissionStatus GetBluetoothStatus()
        {
            var authorization = CBManager.Authorization;
            return authorization switch
            {
                CBManagerAuthorization.NotDetermined => PermissionStatus.Unknown,
                CBManagerAuthorization.Restricted => PermissionStatus.Restricted,
                CBManagerAuthorization.Denied => PermissionStatus.Denied,
                CBManagerAuthorization.AllowedAlways => PermissionStatus.Granted,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        /// <inheritdoc/>
        public override Task<PermissionStatus> RequestAsync()
        {
            /*
             * A request for bluetooth permissions is prompted as soon as the CBManager is used. Therefore, CheckStatus and
             * RequestAsync have the same implementation.
             */
            return CheckStatusAsync();
        }
    }
}
