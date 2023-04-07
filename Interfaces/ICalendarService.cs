using EventsTest.DTOs;
using Google.Apis.Calendar.v3.Data;

namespace EventsTest.Interfaces
{
    public interface ICalendarService
    {
        public Task<IList<Event>> GetUpcomingEvents(FilterEventDto filter);
        public Task CreateEvent(CreateEventDto Event);
        public Task EditEvent(EditEventDto Event);
        public Task DeleteEvent(string eventId);

        public Task SendEmail(string eventId);
        public Task SendNotification(string eventId);
    }
}
