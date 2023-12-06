using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UcakWebProje.Models
{
    public class User
    {
        [Key]
        [Display(Name = "Username")]
        [Required(ErrorMessage = "This field is required!")]
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Only letters and numbers are allowed!")]
        [MaxLength(20, ErrorMessage = "Maximum length is {1} characters!")]
        public string UserName { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "This field is required!")]
        [RegularExpression("^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z])", ErrorMessage = "Please enter a valid password!")]
        [DataType(DataType.Password)]
        [MaxLength(50, ErrorMessage = "Maximum length is {1} characters!")]
        public string Password { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "This field is required!")]
        [RegularExpression("/^(?![ .]+$)[a-zA-Z .]*$", ErrorMessage = "Please enter a valid name!")]
        [MaxLength(50, ErrorMessage = "Maximum length is {1} characters!")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "This field is required!")]
        [RegularExpression("/^(?![ .]+$)[a-zA-Z .]*$", ErrorMessage = "Please enter a valid name!")]
        [MaxLength(50, ErrorMessage = "Maximum length is {1} characters!")]
        public string LastName { get; set; }

        [Display(Name = "E-Mail")]
        [Required(ErrorMessage = "This field is required!")]
        [EmailAddress(ErrorMessage = "Please enter a valid mail address!")]
        [MaxLength(80, ErrorMessage = "Maximum length is {1} characters!")]
        public string Mail { get; set; }

        [Display(Name = "Phone")]
        [Required(ErrorMessage = "This field is required!")]
        [Phone(ErrorMessage = "Please enter a valid phone number!")]
        [MaxLength(20, ErrorMessage = "Maximum length is {1} characters!")]
        public string phoneNum { get; set; }
    }
}
