using Plugin.Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.App.Services
{
    public class FireStoreDeviceDto : IFirestoreObject
    {
        [FirestoreDocumentId]
        public string UID { get; set; }

        [FirestoreProperty("Name")]
        public string Name { get; set; }

        [FirestoreProperty("Model")]
        public string Model { get; set; }

        [FirestoreProperty("Manufacturer")]
        public string Manufacturer { get; set; }

        [FirestoreProperty("Platform")]
        public string Platform { get; set; }

        [FirestoreProperty("CurrentVersion")]
        public string CurrentVersion { get; set; }

        [FirestoreProperty("Comment")]
        public string Comment { get; set; }

        [FirestoreProperty("LastStart")]
        public DateTimeOffset LastStart { get; set; }
    }
}
