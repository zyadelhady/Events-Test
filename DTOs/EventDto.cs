namespace EventsTest.DTOs
{
    public class EventDto
    {
        public EventDto(string id, string title, DateTime? date, string description)
        {
            Id=id;
            Title=title;
            Date=date;
            Description=description;
        }

        public string Id { get;  set; }
        public string Title { get; set; }
        public DateTime? Date { get; set; }
        public string Description { get; set; }
    }
}
