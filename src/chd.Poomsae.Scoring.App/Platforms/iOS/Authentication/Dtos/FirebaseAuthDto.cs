using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.App.Platforms.iOS.Authentication.Dtos
{
    public class FirebaseAuthDto
    {
        public string IdToken { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string Kind { get; set; }
        public string LocalId { get; set; }
        public bool Registered { get; set; }
    }
}
