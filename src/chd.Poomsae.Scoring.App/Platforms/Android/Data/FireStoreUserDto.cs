using Plugin.Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.App.Platforms.Android.Data
{
    public class FireStoreUserDto : IFirestoreObject
    {
        [FirestoreProperty(nameof(UID))]
        public string UID { get; set; }

        [FirestoreProperty(nameof(Email))]
        public string Email { get; set; }

        [FirestoreProperty(nameof(Username))]
        public string Username { get; set; }

        [FirestoreProperty(nameof(IsAdmin))]
        public bool IsAdmin { get; set; }

        [FirestoreProperty(nameof(HasLicense))]
        public bool HasLicense { get; set; }

        [FirestoreProperty(nameof(ValidTo))]
        public DateTimeOffset ValidTo { get; set; }
    }
}
