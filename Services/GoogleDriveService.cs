using EventsTest.Interfaces;
using Google.Apis.Auth.AspNetCore3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Drive.v3;
using Google.Apis.Services;

namespace EventsTest.Services
{
    public class GoogleDriveService : IDriveService
    {
        private readonly IGoogleAuthProvider _auth;
        private readonly BaseClientService.Initializer _baseService;


        public GoogleDriveService(IGoogleAuthProvider auth)
        {
            _auth=auth;
            _baseService = new BaseClientService.Initializer();
        }

        public async Task<DriveService> GetDriveService()
        {

            GoogleCredential cred = await _auth.GetCredentialAsync();
            _baseService.HttpClientInitializer = cred;
            return new DriveService(_baseService);
        }

        public async Task<Google.Apis.Drive.v3.Data.File> UploadFile(IFormFile file)
        {
            var service = await GetDriveService();
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = file.FileName
            };
            FilesResource.CreateMediaUpload request;

            using (var stream = file.OpenReadStream())
            {
                // Create a new file, with metadata and stream.
                request = service.Files.Create(
                    fileMetadata, stream, file.ContentType);
                request.Fields = "id,webViewLink";
                await request.UploadAsync();
            }

            var fileUploaded = request.ResponseBody;
            return fileUploaded;
        }
    }
    }

