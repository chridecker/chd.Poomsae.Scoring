using System;
using System.Collections.Generic;
using System.Text;

namespace chd.Poomsae.Scoring.Contracts.Dtos
{
    public class DeviceFoundEventArgs : EventArgs
    {
        public Guid Id { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
    }
}
