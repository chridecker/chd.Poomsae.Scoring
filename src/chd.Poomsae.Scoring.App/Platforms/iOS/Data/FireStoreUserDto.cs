using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.App.Platforms.iOS.Data
{
    public class FireStoreUserDto
    {
        public string UID { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public bool IsAdmin { get; set; }
        public bool HasLicense { get; set; }
        public DateTimeOffset ValidTo { get; set; }
    }
}
