using Plugin.Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.App.Platforms.Android.Data
{
    public class FireStoreDeviceDto : IFirestoreObject
    {
        [FirestoreDocumentId]
        public string UID { get; set; }

        [FirestoreProperty(nameof(Name))]
        public string Name { get; set; }

        [FirestoreProperty(nameof(Model))]
        public string Model { get; set; }

         [FirestoreProperty(nameof(Manufacturer))]
        public string Manufacturer { get; set; }

         [FirestoreProperty(nameof(Platform))]
        public string Platform { get; set; }

        [FirestoreProperty(nameof(CurrentVersion))]
        public string CurrentVersion { get; set; }

         [FirestoreProperty(nameof(Comment))]
        public string Comment { get; set; }  = string.Empty;

         [FirestoreProperty(nameof(LastStart))]
        public DateTimeOffset LastStart { get; set; }
    }
}
