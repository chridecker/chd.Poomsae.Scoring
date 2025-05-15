using Plugin.Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.App.Services
{
    public class FireStoreUserDto : IFirestoreObject
    {
        [FirestoreDocumentId]
        public string UID { get; set; }

        [FirestoreProperty("Email")]
        public string Email { get; set; }

        [FirestoreProperty("Username")]
        public string Username { get; set; }

        [FirestoreProperty("IsAdmin")]
        public bool IsAdmin { get; set; }

        [FirestoreProperty("HasLicense")]
        public bool HasLicense { get; set; }

        [FirestoreProperty("ValidTo")]
        public DateTimeOffset ValidTo { get; set; }
    }
}
