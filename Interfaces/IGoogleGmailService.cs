
using Google.Apis.Gmail.v1;

namespace EventsTest.Interfaces
{
    public interface IGoogleGmailService
    {
        public Task<GmailService> GetGmailService();
        public Task SendEmail(string link);
    }
}
