using System.ComponentModel.DataAnnotations;

namespace UcakWebProje.Models
{
    public class Bilet
    {
        [Key]
        [Display(Name = "From")]
        [Required(ErrorMessage = "This field is required!")]
        [RegularExpression("^[a-zA-Z]+$")]
        public string departure { get; set; }
        [Display(Name = "To")]
        [Required(ErrorMessage = "This field is required!")]
        [RegularExpression("^[a-zA-Z]+$")]
        public string destination { get; set; }
        [Display(Name = "Date")]
        [Required(ErrorMessage = "This field is required!")]
        public DateTime date { get; set; }

        [Display(Name = "Number of Passengers")]
        [Required(ErrorMessage = "This field is required!")]
        [Range(1, 500, ErrorMessage = "Enter a positive number!")]
        public int numberOfPassengers { get; set; }
    }
}
