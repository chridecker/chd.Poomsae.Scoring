using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.App.Platforms.iOS.Data
{
    public class FireStoreDeviceDto
    {
        public string Name { get; set; }
        public string Model { get; set; }
        public string Manufacturer { get; set; }
        public string Platform { get; set; }
        public string CurrentVersion { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTimeOffset LastStart { get; set; }
    }
}
