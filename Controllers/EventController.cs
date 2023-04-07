using EventsTest.DTOs;
using EventsTest.Interfaces;
using Google.Apis.Auth.AspNetCore3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Gmail.v1;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;

namespace EventsTest.Controllers
{
    [GoogleScopedAuthorize(CalendarService.ScopeConstants.Calendar,DriveService.ScopeConstants.Drive,GmailService.ScopeConstants.GmailInsert,GmailService.ScopeConstants.MailGoogleCom)]
    public class EventController : Controller
    {
        private readonly ICalendarService _googleCalendarService;
        public IList<EventDto> Events { get; set; }
        public EventController(ICalendarService googleCalendarService)
        {
            Events = new List<EventDto>();
            _googleCalendarService = googleCalendarService;
        }

        public async Task<IActionResult> Index(string? ErrorMessage)
        {
            var events = await _googleCalendarService.GetUpcomingEvents(new FilterEventDto("",""));

             
            foreach (var @event in events)
            {
                var e = new EventDto(@event.Id, @event.Summary, @event.Start.DateTime, @event.Description);
                Events.Add(e);
            }
           if(ErrorMessage!= null) ModelState.AddModelError(String.Empty, ErrorMessage);

            return View(Events);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent() 
        {
            string Title = Request.Form["Title"];
            string StartDate = Request.Form["SDate"];
            string EndDate = Request.Form["EDate"];
            string StartTime = Request.Form["STime"];
            string EndTime = Request.Form["ETime"];
            string Description = Request.Form["Desc"];
            IFormFile file = Request.Form.Files["formUpload"];

            string StartDateTime = $"{StartDate} {StartTime}";
            DateTime dateTimeStart = DateTime.Parse(StartDateTime);

            string EndDateTime = $"{EndDate} {EndTime}";
            DateTime dateTimeEnd = DateTime.Parse(EndDateTime);
            if (DateTime.Compare(dateTimeStart,dateTimeEnd) > 0)
            {
                return RedirectToAction("Index", new { ErrorMessage = "The end date must be after the start date" });
            }

            var createdEvent = new CreateEventDto(Title, dateTimeStart, dateTimeEnd, Description,file);

            if (ModelState.IsValid)
            {
                await _googleCalendarService.CreateEvent(createdEvent);
            }


            return Redirect("Index");
        }

        public async Task<IActionResult> DeleteEvent(string Id)
        {
            await _googleCalendarService.DeleteEvent(Id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> EditEvent(string Id,string Title,string Desc)
        {
            return View("Edit",new EditEventDto(Id,Title,Desc));
        }

        [HttpPost]
        public async Task<IActionResult> Edit()
        {
            string Title = Request.Form["Title"];
            string Description = Request.Form["Desc"];
            string Id = Request.Form["Id"];

            var editedEvent = new EditEventDto(Id,Title, Description);

             await _googleCalendarService.EditEvent(editedEvent);
         
            return RedirectToAction("Index");

        }
        public async Task<IActionResult> RemindEmail(string Id)
        {
            await _googleCalendarService.SendEmail(Id);

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> RemindNotifications(string Id)
        {
            await _googleCalendarService.SendNotification(Id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> FilterEvents()
        {
            string Title = Request.Form["TitleFilter"];
            string Date = Request.Form["DateFilter"];

            var filter = new FilterEventDto(Title,Date);

            var events = await _googleCalendarService.GetUpcomingEvents(filter);

            foreach (var @event in events)
            {
                var e = new EventDto(@event.Id, @event.Summary, @event.Start.DateTime, @event.Description);
                Events.Add(e);
            }

            return View("Index", Events);
        }

    }
}