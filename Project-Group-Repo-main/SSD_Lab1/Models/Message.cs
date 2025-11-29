using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SSD_Lab1.Models
{
    public class Message
    {
        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Content { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        [Display(Name = "Sent At")]
        public DateTime SentAt { get; set; } = DateTime.Now;
    }

}
