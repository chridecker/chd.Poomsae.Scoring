using Android.Bluetooth.LE;
using Android.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.App.Platforms.Android.BLE
{
    public class BLEAdvertisingCallback : AdvertiseCallback
    {

        public override void OnStartFailure([GeneratedEnum] AdvertiseFailure errorCode)
        {
            base.OnStartFailure(errorCode);
        }

        public override void OnStartSuccess(AdvertiseSettings? settingsInEffect)
        {
            base.OnStartSuccess(settingsInEffect);
        }
    }
}
