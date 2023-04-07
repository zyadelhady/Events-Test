using EventsTest.DTOs;
using EventsTest.Interfaces;
using Google.Apis.Auth.AspNetCore3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;


namespace EventsTest.Services
{
    public class GoogleCalendarService : ICalendarService
    {
        private readonly IGoogleAuthProvider _auth;
        private readonly BaseClientService.Initializer _baseService;
        private readonly IDriveService _driveService;
        private readonly IGoogleGmailService _googleGmailService;


        public GoogleCalendarService(IGoogleAuthProvider auth, IDriveService driveService,IGoogleGmailService googleGmailService)
        {
            _auth = auth;
            _baseService = new BaseClientService.Initializer();
            _driveService= driveService;
            _googleGmailService = googleGmailService;
        }

        public async Task<CalendarService> GetCalenderService()
        {

            GoogleCredential cred = await _auth.GetCredentialAsync();
            _baseService.HttpClientInitializer = cred;
            return new CalendarService(_baseService);
        }
        public async Task<IList<Event>> GetUpcomingEvents(FilterEventDto filter)
        {
            
            var _calendarService = await GetCalenderService();
            var request = _calendarService.Events.List("primary");
            request.MaxResults = 100;
            request.TimeMin = DateTime.Now;
            if (filter.Date != "")
            {
                request.TimeMin = DateTime.Parse(filter.Date);
                request.TimeMax = DateTime.Parse(filter.Date).AddHours(24);
            }
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
            request.SingleEvents = true;
            if (filter.Title != "")
            {
                request.Q = filter.Title;
            }
            var events = await request.ExecuteAsync();

            return events.Items;
        }

        public async Task CreateEvent(CreateEventDto Event)
        {
            var attachments = new List<EventAttachment>();

            if (Event.UploadedFile != null)
            {
                var fileUploaded = await _driveService.UploadFile(Event.UploadedFile);

                var attach = new EventAttachment
                {
                    FileId = fileUploaded.Id,
                    FileUrl = fileUploaded.WebViewLink,
                    MimeType = fileUploaded.MimeType,
                    Title = "Alo"
                };

                attachments.Add(attach);

            }

            var _calendarService = await GetCalenderService();

            var newEvent = new Event
            {

                Summary = Event.Title,
                Description = Event.Description,
                Start = new EventDateTime
                {
                    DateTime = Event.StartDate,
                    TimeZone = TimeZoneInfo.Local.ToString()
                },
                End = new EventDateTime
                {
                    DateTime = Event.EndDate,
                    TimeZone = TimeZoneInfo.Local.ToString()

                },
                Attachments = attachments,


            };

            var request = _calendarService.Events.Insert(newEvent, "primary");
            request.SupportsAttachments = true;
            request.SendUpdates = EventsResource.InsertRequest.SendUpdatesEnum.All;
            var e = request.Execute();
            await _googleGmailService.SendEmail(e.HtmlLink);
 
        }

        public async Task DeleteEvent(string eventId)
        {
            var _calendarService = await GetCalenderService();
            var request = _calendarService.Events.Delete("primary", eventId);
            request.Execute();
        }

        public async Task EditEvent(EditEventDto Event)
        {
            var newEvent = new Event { Summary = Event.Title, Description = Event.Description };
            var _calendarService = await GetCalenderService();
            var request = _calendarService.Events.Patch(newEvent, "primary", Event.Id);
            request.Execute();
        }

        public async Task SendEmail(string eventId)
        {
            var editedEvent = new Event
            {
                Reminders = new Event.RemindersData
                {
                    UseDefault = false,
                    Overrides = new[]
                    {
                        new EventReminder
                        {
                            Method = "email",
                            Minutes = 5
                        }
                    }

                }
            };

            var _calendarService = await GetCalenderService();
            var request = _calendarService.Events.Patch(editedEvent, "primary", eventId);
            request.Execute();

        }

        public async Task SendNotification(string eventId)
        {
            var editedEvent = new Event
            {
                Reminders = new Event.RemindersData
                {
                    UseDefault = false,
                    Overrides = new[]
                   {
                        new EventReminder
                        {
                            Method = "popup",
                            Minutes = 5
                        }
                    }

                }
            };

            var _calendarService = await GetCalenderService();
            var request = _calendarService.Events.Patch(editedEvent, "primary", eventId);
            request.Execute();
        }
    }
}
