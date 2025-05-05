using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.WPF.Settings
{
    public class SettingDto
    {
        public bool IsServer { get; set; }
        public int ServerPort { get; set; }
        public string ServerAddress{ get; set; }
    }
}
