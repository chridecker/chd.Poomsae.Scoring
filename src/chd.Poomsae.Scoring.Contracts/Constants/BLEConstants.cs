﻿using System;
using System.Collections.Generic;
using System.Text;

namespace chd.Poomsae.Scoring.Contracts.Constants
{
    public class BLEConstants
    {
        //public static readonly string Result_Gatt_Service = "A8B9";
        public static readonly Guid Result_Gatt_Service = Guid.Parse("{0000a8b9-0000-1000-8000-00805f9b34fb}");
        public static readonly string Result_Characteristic = "99A4";
        //public static readonly Guid Result_Characteristic = Guid.Parse("{000099A4-0000-1000-8000-00805f9b34fb}");
        public static readonly string Name_Characteristic = "88A5";
        //public static readonly Guid Name_Characteristic = Guid.Parse("{A75999A4-87CE-4103-AC44-3712998AFADB}");
        //public static readonly Guid Notify_Descriptor = Guid.Parse("{00002902-0000-1000-8000-00805f9b34fb}");
        public static readonly string Notify_Descriptor = "2902";
    }
}
