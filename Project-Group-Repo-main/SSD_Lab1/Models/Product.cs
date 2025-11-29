using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SSD_Lab1.Models
{
    public class Product
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters")]
        [RegularExpression(@"^[A-Za-z0-9\s\-\.]+$", ErrorMessage = "Product name contains invalid characters")]
        public string Name { get; set; }

        [StringLength(300, ErrorMessage = "Description cannot exceed 300 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 100000, ErrorMessage = "Price must be between 0.01 and 100,000")]
        public decimal Price { get; set; }

        [Display(Name = "In Stock")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative")]
        public int StockQuantity { get; set; }

        [Required(ErrorMessage = "Company is required")]
        public Guid CompanyId { get; set; }

        [ForeignKey("CompanyId")]
        public Company? Company { get; set; }
    }
}