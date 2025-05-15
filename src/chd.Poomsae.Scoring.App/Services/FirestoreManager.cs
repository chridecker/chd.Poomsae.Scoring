using chd.Poomsae.Scoring.App.Extensions;
using chd.Poomsae.Scoring.Contracts.Dtos;
using Firebase.Annotations;
using Firebase.Firestore;
using Plugin.Firebase.Firestore;
using Plugin.Firebase.Firestore.Platforms.Android;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.App.Services
{
    public class FirestoreManager
    {
        private readonly IFirebaseFirestore _firebaseFirestore;

        public FirestoreManager(IFirebaseFirestore firebaseFirestore)
        {
            this._firebaseFirestore = firebaseFirestore;
        }

        public async Task<PSUserDto> GetOrCreateUser(PSUserDto user)
        {
            var userCollection = this._firebaseFirestore.GetCollection("users");
            var userDocument = userCollection.GetDocument(user.UID);

            var snap = await userDocument.GetDocumentSnapshotAsync<FireStoreUserDto>(Plugin.Firebase.Firestore.Source.Server);

            if (snap?.Data is null || string.IsNullOrEmpty(snap.Data.UID))
            {

                user.ValidTo = DateTime.Now.Date;
                await snap.Reference.SetDataAsync(user.ToFSUser());
                return user;
            }
            return snap.Data.ToPSUser();
        }



    }
}
