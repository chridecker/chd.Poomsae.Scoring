using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.Contracts.Dtos
{
    public  class DeviceConnectionChangedEventArgs : EventArgs
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Connected { get; set; }
    }
}
