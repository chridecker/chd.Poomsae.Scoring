using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.App.Platforms.iOS.Authentication.Dtos
{
    public class EmailPasswordLoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool ReturnSecureToken { get; set; } = true;
    }
}
