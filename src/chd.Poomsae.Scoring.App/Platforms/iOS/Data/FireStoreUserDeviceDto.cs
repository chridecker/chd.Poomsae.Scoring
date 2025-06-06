using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.App.Platforms.iOS.Data
{
    public class FireStoreUserDeviceDto
    {
        public string Device_UID { get; set; }
        public string User_UID { get; set; }
        public bool IsAllowed { get; set; }
        public bool HasFighters { get; set; }
        public DateTimeOffset Created { get; set; }
    }
}
