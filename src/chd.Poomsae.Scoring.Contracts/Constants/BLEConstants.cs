using System;
using System.Collections.Generic;
using System.Text;

namespace chd.Poomsae.Scoring.Contracts.Constants
{
    public class BLEConstants
    {
        //public static readonly string Result_Gatt_Service = "A8B9";
        public static readonly Guid Result_Gatt_Service = Guid.Parse("{0000a8b9-0000-1000-8000-00805f9b34fb}");
        public static readonly string Result_Characteristic = "99A4";
        public static readonly string Name_Characteristic = "88A5";

        public static readonly string BlueName_Characteristic = "88A3";
        public static readonly string RedName_Characteristic = "88A2";

        public static readonly string Notify_Descriptor = "2902";
    }
}
