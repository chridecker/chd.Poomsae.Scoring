using System;
using System.Collections.Generic;
using System.Text;

namespace chd.Poomsae.Scoring.Contracts.Constants
{
    public class BLEConstants
    {
        public static readonly Guid Result_Gatt_Service = Guid.Parse("{0FB7A8B9-AA3C-4FE1-83F2-96E32C771C6B}");
        public static readonly Guid Name_Characteristic = Guid.Parse("{A75999A4-87CE-4103-AC44-3712998AFADB}");
        public static readonly Guid Result_Characteristic = Guid.Parse("{D75999A4-87CE-4103-AC44-3712998AFADB}");
        public static readonly Guid Notify_Descriptor = Guid.Parse("{00002902-0000-1000-8000-00805f9b34fb}");
    }
}
