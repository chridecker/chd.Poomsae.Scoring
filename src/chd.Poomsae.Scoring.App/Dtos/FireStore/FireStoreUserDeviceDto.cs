using Plugin.Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.App.Dtos.FireStore
{
    public class FireStoreUserDeviceDto : IFirestoreObject
    {
        [FirestoreDocumentId]
        public string Id { get; set; }

        [FirestoreProperty("Device_UID")]
        public string Device_UID { get; set; }

        [FirestoreProperty("User_UID")]
        public string User_UID { get; set; }

        [FirestoreProperty("IsAllowed")]
        public bool IsAllowed { get; set; }
        
        
        [FirestoreProperty("Created")]
        public DateTimeOffset Created { get; set; }
    }
}
