using System.ComponentModel.DataAnnotations;

namespace UcakWebProje.Models
{
    public abstract class Travel
    {
        [Key]
        [Display(Name = "From")]
        [Required(ErrorMessage = "This field is required!")]
        [RegularExpression("^[a-zA-Z]+$")]
        [MaxLength(30)]
        public string departure { get; set; }

        [Key]
        [Display(Name = "To")]
        [Required(ErrorMessage = "This field is required!")]
        [RegularExpression("^[a-zA-Z]+$")]
        [MaxLength(30)]
        public string destination { get; set; }

        [Key]
        [Display(Name = "Date")]
        [Required(ErrorMessage = "This field is required!")]
        public DateTime date { get; set; }

        [Key]
        [Display(Name = "Airlines")]
        [Required(ErrorMessage = "This field is required!")]
        [RegularExpression("^[a-zA-Z0-9]+$")]
        [MaxLength(50)]
        public string AirLine { get; set; }
    }
}
