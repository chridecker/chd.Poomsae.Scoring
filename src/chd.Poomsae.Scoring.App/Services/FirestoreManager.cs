using chd.Poomsae.Scoring.Contracts.Dtos;
using Firebase.Annotations;
using Plugin.Firebase.Firestore;
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
            try
            {
                var d = await userDocument.GetDocumentSnapshotAsync<PSUserDto>();
                if (d.Data is null || string.IsNullOrEmpty(d.Data.UID))
                {
                    await userDocument.SetDataAsync(new Dictionary<object, object>() {
                        {nameof(PSUserDto.UID), user.UID },
                        { nameof(PSUserDto.Email), user.Email},
                        {nameof(PSUserDto.Username), user.Username },
                        {nameof(PSUserDto.IsAdmin), false },
                        {nameof(PSUserDto.HasLicense), false },
                        { nameof(PSUserDto.ValidTo), DateTime.Now.AddDays(10) }
                    });
                    return user;
                }
                return d.Data;
            }
            catch (Exception ex)
            {

            }
            return user;
        }

    }
}
