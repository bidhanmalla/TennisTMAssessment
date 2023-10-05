using Microsoft.AspNetCore.Mvc.Rendering;

namespace TennisTM.Models
{
    public class TempSchedule
    {
        public Guid Id { get; set; }
        public string EventName { get; set; }
        public string EventTime { get; set; }
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
