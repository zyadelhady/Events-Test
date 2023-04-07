namespace EventsTest.DTOs
{


    public class CreateEventDto
    {
        public CreateEventDto(string title, DateTime startDate, DateTime endDate, string description, IFormFile uploadedFile)
        {
            Title=title;
            StartDate=startDate;
            EndDate=endDate;
            Description=description;
            UploadedFile = uploadedFile;
        }

        public string Title { get; set; }
        public DateTime StartDate { get;set; }
        public DateTime EndDate { get;set; }
   
        public IFormFile UploadedFile { get; set; }
        public string Description { get;set; }

    }
}
