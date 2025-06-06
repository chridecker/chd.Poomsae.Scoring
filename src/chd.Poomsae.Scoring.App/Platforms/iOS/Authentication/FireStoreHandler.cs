using chd.Poomsae.Scoring.App.Platforms.iOS.Data;
using chd.Poomsae.Scoring.Contracts.Dtos;
using chd.Poomsae.Scoring.Contracts.Interfaces;
using chd.UI.Base.Contracts.Interfaces.Update;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.App.Platforms.iOS.Authentication
{
    public class FireStoreHandler : IDataService
    {
        private readonly IDeviceHandler _deviceHandler;
        private readonly IUpdateService _updateService;

        const string PROJECTID = "chdpoomsaescoring";
        const string BaseUrl = $"https://firestore.googleapis.com/v1/projects/{PROJECTID}/databases/(default)/documents";

        public FireStoreHandler(IDeviceHandler deviceHandler, IUpdateService updateService)
        {
            this._deviceHandler = deviceHandler;
            this._updateService = updateService;
        }

        public async Task<PSDeviceDto> GetOrCreateDevice()
        {
            var version = await this._updateService.CurrentVersion();
            var fsDevice = await this.GetDocumentAsync<FireStoreDeviceDto>("devices", this._deviceHandler.UID);

            if (fsDevice is null)
            {
                var deviceDto = new PSDeviceDto()
                {
                    UID = this._deviceHandler.UID,
                    CurrentVersion = version.ToString(),
                    Manufacturer = this._deviceHandler.Manufacturer,
                    Model = this._deviceHandler.Model,
                    Name = this._deviceHandler.Name,
                    Platform = this._deviceHandler.Platform,
                    LastStart = DateTimeOffset.Now,
                    Comment = string.Empty,
                };
                await this.CreateDocumentAsync("devices", this._deviceHandler.UID, deviceDto.ToFSDevice());
                return deviceDto;
            }
            else
            {
                fsDevice.CurrentVersion = version.ToString();
                fsDevice.LastStart = DateTimeOffset.Now;
                await this.UpdateDocumentAsync("devices", this._deviceHandler.UID, fsDevice);
                return fsDevice.ToPSDevice(this._deviceHandler.UID);
            }
        }


        public async Task<PSUserDto> GetOrCreateUser(PSUserDto user)
        {
            var fsUser = await this.GetDocumentAsync<FireStoreUserDto>("users", user.UID);
            if (fsUser is null)
            {
                await this.CreateDocumentAsync("users", user.UID, user.ToFSUser());
                return user;
            }
            return fsUser.ToPSUser();
        }


        public async Task<PSUserDeviceDto> GetOrCreateUserDevice(string userId, string deviceId, bool isAdmin)
        {
            var lst = await this.FindByQuery<FireStoreUserDeviceDto>("user_devices", new
            {
                op = "AND",
                filters = new object[]
                            {
                                new {
                                    fieldFilter = new {
                                        field = new { fieldPath = "Device_UID" },
                                        op = "EQUAL",
                                        value = new { stringValue = this._deviceHandler.UID  }
                                    }
                                },
                                new {
                                    fieldFilter = new {
                                        field = new { fieldPath = "User_UID" },
                                        op = "EQUAL",
                                        value = new { stringValue = userId }
                                    }
                                }
                            }
            }, 2);

            var userDeviceDocumentDatas = lst.FirstOrDefault(x => x is not null && x.Device_UID == this._deviceHandler.UID && x.User_UID == userId);

            if (userDeviceDocumentDatas is null)
            {
                var deviceDto = new PSUserDeviceDto()
                {
                    Device_UID = this._deviceHandler.UID,
                    IsAllowed = isAdmin,
                    User_UID = userId,
                    Created = DateTimeOffset.Now,
                };
                var id = Guid.NewGuid().ToString();
                await this.CreateDocumentAsync<FireStoreUserDeviceDto>("user_devices", id, deviceDto.ToFSUserDevice());
                deviceDto.Id = id;
                return deviceDto;
            }
            return userDeviceDocumentDatas.ToPSUserDevice();
        }

        private async Task<List<T>> FindByQuery<T>(string collection, dynamic compositeFilter, int limit)
            where T : new()
        {
            var url = $"{BaseUrl}:runQuery";
            var query = new
            {
                structuredQuery = new
                {
                    from = new[] { new { collectionId = collection } },
                    where = new
                    {
                        compositeFilter = compositeFilter
                    },
                    limit = limit
                }
            };
            using var httpClient = new HttpClient();
            var response = await httpClient.PostAsJsonAsync(url, query);
            var content = await response.Content.ReadAsStringAsync();

            var lst = new List<T>();
            try
            {
                var jsonArray = JsonNode.Parse(content)?.AsArray();
                if (jsonArray != null)
                {
                    foreach (var item in jsonArray)
                    {
                        lst.Add(this.FromFirestoreFormat<T>(item[0].ToString()));
                    }
                }
            }
            catch { }


            return lst;
        }

        private async Task<bool> CreateDocumentAsync<T>(string collection, string documentId, T data)
           where T : new()
        {
            var url = $"{BaseUrl}/{collection}?documentId={documentId}";
            using var httpClient = new HttpClient();
            var firestoreFields = ToFirestoreFormat(data);

            var body = new
            {
                fields = firestoreFields
            };

            var response = await httpClient.PostAsJsonAsync(url, body);
            return response.IsSuccessStatusCode;
        }

        private async Task<bool> UpdateDocumentAsync<T>(string collection, string documentId, T data)
            where T : new()
        {
            var url = $"{BaseUrl}/{collection}/{documentId}";

            var firestoreFields = ToFirestoreFormat(data);
            using var httpClient = new HttpClient();
            var body = new
            {
                fields = firestoreFields
            };
            var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Patch, url)
            {
                Content = content
            };
            var response = await httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        private async Task<T> GetDocumentAsync<T>(string collection, string documentId)
            where T : new()
        {
            using var httpClient = new HttpClient();
            var url = $"{BaseUrl}/{collection}/{documentId}";
            var response = await httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            return this.FromFirestoreFormat<T>(content);
        }

        private T FromFirestoreFormat<T>(string json) where T : new()
        {
            var doc = JsonNode.Parse(json);
            var fields = doc?["fields"]?.AsObject();

            if (fields == null) { return default!; }

            var result = new Dictionary<string, object?>();

            foreach (var kvp in fields)
            {
                var fieldValue = kvp.Value?.AsObject();

                if (fieldValue == null) { continue; }

                // Firestore speichert Werte als {"stringValue":"...", "integerValue":"...", ...}
                foreach (var prop in fieldValue)
                {
                    var value = prop.Value;
                    // Typen können unterschiedlich sein, hier einfache Zuordnung (string, int, bool)
                    switch (prop.Key)
                    {
                        case "stringValue":
                            result[kvp.Key] = value?.GetValue<string>();
                            break;
                        case "integerValue":
                            result[kvp.Key] = int.Parse(value?.GetValue<string>() ?? "0");
                            break;
                        case "booleanValue":
                            result[kvp.Key] = value?.GetValue<bool>();
                            break;
                        case "doubleValue":
                            result[kvp.Key] = value?.GetValue<double>();
                            break;
                        case "timestampValue":
                            result[kvp.Key] = value?.GetValue<DateTimeOffset>();
                            break;
                        default:
                            break;
                    }
                }
            }

            var resultJson = JsonSerializer.Serialize(result);
            return JsonSerializer.Deserialize<T>(resultJson)!;
        }

        private Dictionary<string, object> ToFirestoreFormat<T>(T data)
        {
            var result = new Dictionary<string, object>();
            var properties = data.GetType().GetProperties();
            foreach (var prop in properties)
            {
                var value = prop.GetValue(data);
                if (value is string strVal)
                {
                    result[prop.Name] = new { stringValue = strVal };
                }
                else if (value is int intVal)
                {
                    result[prop.Name] = new { integerValue = intVal.ToString() };
                }
                else if (value is bool boolVal)
                {
                    result[prop.Name] = new { booleanValue = boolVal };
                }
                else if (value is DateTimeOffset dtOff)
                {
                    result[prop.Name] = new { timestampValue = dtOff };
                }
            }
            return result;
        }
    }
}
