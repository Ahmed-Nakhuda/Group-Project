using System;
using System.ComponentModel.DataAnnotations;

namespace SSD_Lab1.Models
{
    public class Company : IAuditable
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Company name is required")]
        public string Name { get; set; }

        [Display(Name = "Years in Business")]
        [Required(ErrorMessage = "Years in business is required")]
        [Range(0, 500, ErrorMessage = "Years in business must be between 0 and 500")]
        public int YearsInBusiness { get; set; }

        [Required(ErrorMessage = "Website is required")]
        [Url(ErrorMessage = "Please enter a valid website URL")]
        public string Website { get; set; }

        public string? Province { get; set; }

        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Default value
        public string CreatedBy { get; set; } = "System"; // Default value
        public DateTime? ModifiedAt { get; set; }
        public string? ModifiedBy { get; set; }
    }
}