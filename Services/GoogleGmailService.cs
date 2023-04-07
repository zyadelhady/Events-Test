using EventsTest.Interfaces;
using Google.Apis.Auth.AspNetCore3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;

namespace EventsTest.Services
{
    public class GoogleGmailService : IGoogleGmailService
    {
        private readonly IGoogleAuthProvider _auth;
        private readonly BaseClientService.Initializer _baseService;


        public GoogleGmailService(IGoogleAuthProvider auth)
        {
            _auth=auth;
            _baseService = new BaseClientService.Initializer();
        }

        public async Task<GmailService> GetGmailService()
        {

            GoogleCredential cred = await _auth.GetCredentialAsync();
            
            _baseService.HttpClientInitializer = cred;
            return new GmailService(_baseService);
        }

        public async Task SendEmail(string link)
        {
            var service = await GetGmailService();
            var gmailProfile = service.Users.GetProfile("me").Execute();
            string to = gmailProfile.EmailAddress;
            string subject = "created Event";
            string body = "new Event Created and the link is  " + link;
            string message = string.Format("To: {0}\r\nSubject: {1}\r\n\r\n{2}", to, subject, body);

            var newMessage = new Message
            {
                Raw = Base64UrlEncode(message)
            };
            var request = service.Users.Messages.Send(newMessage, "me");
            request.Execute();
        }

        private string Base64UrlEncode(string input)
        {
            var inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(inputBytes).Replace('+', '-').Replace('/', '_').Replace("=", "");
        }

    }
}
