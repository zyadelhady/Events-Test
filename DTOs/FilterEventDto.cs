namespace EventsTest.DTOs
{
    public class FilterEventDto
    {
        public FilterEventDto(String title, String date)
        {
            Title=title;
            Date=date;
        }

        public String Title { get; set; }
        public String Date { get; set; }
    }
}
