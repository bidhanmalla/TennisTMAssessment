namespace TennisTM.Models
{
    public class Schedule
    {
        public Guid Id { get; set; }
        public string EventName { get; set; }
        public string? EventTime { get; set; }
        public string Location { get; set; }
        public Coach? Coach { get; set; }
        public ICollection<User> Users { get; set; }
    }
}
