using System.ComponentModel.DataAnnotations;
namespace Tech_Desk.Models
{
    public class Device
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "The asset tag is required.")]
        [StringLength(10, ErrorMessage = "The asset tag can be at most 10 characters.")]
        [Display(Name = "Asset tag")]
        public string AssetTag { get; set; } = string.Empty;

        [Required(ErrorMessage = "The device name is required.")]
        [StringLength(50, ErrorMessage = "The name can be at most 50 characters.")]
        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "The category is required.")]
        [StringLength(20, ErrorMessage = "The category can be at most 20 characters.")]
        [Display(Name = "Category")]
        public string Category { get; set; } = string.Empty;

        [Range(2015, 2026, ErrorMessage = "The year bought must be from 2015 to 2026.")]
        [Display(Name = "Year bought")]
        public int PurchaseYear { get; set; }

        [Display(Name = "On loan?")]
        public bool IsOnLoan { get; set; }
    }
}
