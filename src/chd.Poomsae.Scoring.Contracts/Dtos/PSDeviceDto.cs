using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.Contracts.Dtos
{
    public class PSDeviceDto
    {
        public string UID { get; set; }
        public string Name { get; set; }
        public string Model { get; set; }
        public string Manufacturer { get; set; }
        public string Platform { get; set; }
        public string CurrentVersion { get; set; }

        public override string ToString() => $"{this.Name} ({this.Model} - {this.Manufacturer})";
    }
}
