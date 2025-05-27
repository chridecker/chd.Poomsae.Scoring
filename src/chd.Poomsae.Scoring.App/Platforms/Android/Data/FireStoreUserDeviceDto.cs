using Plugin.Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.App.Platforms.Android.Data
{
     public class FireStoreUserDeviceDto : IFirestoreObject
    {
        [FirestoreDocumentId]
        public string Id { get; set; }

        [FirestoreProperty(nameof(Device_UID))]
        public string Device_UID { get; set; }

        [FirestoreProperty(nameof(User_UID))]
        public string User_UID { get; set; }

        [FirestoreProperty(nameof(IsAllowed))]
        public bool IsAllowed { get; set; }
        
        
        [FirestoreProperty(nameof(Created))]
        public DateTimeOffset Created { get; set; }
    }
}
