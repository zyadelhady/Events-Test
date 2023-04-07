using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;

namespace EventsTest.Interfaces
{
    public interface IDriveService
    {
        public Task<DriveService> GetDriveService();
        public Task<Google.Apis.Drive.v3.Data.File> UploadFile(IFormFile file);
    }
}
