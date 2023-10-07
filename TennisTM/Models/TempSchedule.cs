using Microsoft.AspNetCore.Mvc.Rendering;

namespace TennisTM.Models
{
    public class TempSchedule
    {
        public Guid Id { get; set; }
        public string EventName { get; set; }
        public DateTime EventTime { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
        public string Location { get; set; }
        public Guid? CoachId { get; set; }
        public List<SelectListItem> CoachList { get; set; } = new List<SelectListItem>() { 
            new SelectListItem() {
                Text = "Select a Coach",
                Value = ""
            }
        };
    }
}
