namespace EventsTest.DTOs
{
    public class EditEventDto
    {
        public EditEventDto(string id, string title, string description)
        {
            Id=id;
            Title=title;
            Description=description;
        }

        public String Id { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }


    }
}
