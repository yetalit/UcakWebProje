using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UcakWebProje.Models
{
    public class Bilet : Travel
    {
        [Display(Name = "Number of Passengers")]
        [Required(ErrorMessage = "This field is required!")]
        [Range(1, 500, ErrorMessage = "Enter a number between 1 to 500!")]
        public int numberOfPassengers { get; set; }

        [Key, ForeignKey("userName")]
        [Display(Name = "Passenger Username")]
        [Required(ErrorMessage = "This field is required!")]
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Only letters and numbers are allowed!")]
        [MaxLength(20, ErrorMessage = "Maximum length is {1} characters!")]
        public string passengerUN { get; set; }

        [Key]
        [Display(Name = "Order Time")]
        [Required(ErrorMessage = "This field is required!")]
        public DateTime orderTime { get; set; }
    }
}
