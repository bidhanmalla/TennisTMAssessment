using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TennisTM.Models
{
    public class Coach
    {
        public Guid Id { get; set; }
        [MaxLength(100)]
        public string Bio { get; set; } = "No bio written yet.";
        public string Image { get; set; } = "https://img.freepik.com/free-vector/illustration-businessman_53876-5856.jpg?w=740&t=st=1696523364~exp=1696523964~hmac=7dea5c82b135d0676f325bf1b85472d0f7e46a8fcc12a898ebb4212d02d38903";
        public ICollection<Schedule>? Schedules { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}