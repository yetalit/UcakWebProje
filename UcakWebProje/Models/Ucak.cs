using System.ComponentModel.DataAnnotations;

namespace UcakWebProje.Models
{
    public class Ucak : Travel
    {
        [Display(Name = "Seat Count")]
        [Required(ErrorMessage = "This field is required!")]
        [Range(1, 500, ErrorMessage = "Enter a positive number!")]
        public int seatCount { get; set; }
    }
}
