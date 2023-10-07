namespace TennisTM.Models
{
    public class Schedule
    {
        public Guid Id { get; set; }
        public string EventName { get; set; }
        public DateTime EventTime { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
        public string Location { get; set; }
        public Coach? Coach { get; set; }
        public ICollection<User> Users { get; set; }
    }
}
