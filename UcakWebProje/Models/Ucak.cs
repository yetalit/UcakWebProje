using System.ComponentModel.DataAnnotations;

namespace UcakWebProje.Models
{
    public class Ucak : Travel
    {
        [Display(Name = "Price")]
        [Required(ErrorMessage = "This field is required!")]
        [Range(1, int.MaxValue, ErrorMessage = "Enter a positive number!")]
        public int Price { get; set; }

        [Display(Name = "Seat Count")]
        [Required(ErrorMessage = "This field is required!")]
        [Range(1, 500, ErrorMessage = "Enter a number between 1 to 500!")]
        public int seatCount { get; set; }
    }
}
