using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TennisTM.Models
{
    public class Coach
    {
        public Guid Id { get; set; }
        [MaxLength(100)]
        public string? Bio { get; set; }
        public ICollection<Schedule>? Schedules { get; set; }
        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}