using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TeamplateHotel.Areas.Administrator.EntityModel
{
    public class ELanguage
    {
        
        [DisplayName("Languages")]
        [MaxLength(10, ErrorMessage = "Up to 10 characters")]
        [Required(ErrorMessage = "Please enter the language symbol")]
        public string ID { get; set; }

        [DisplayName("Name")]
        [MaxLength(50, ErrorMessage = "Up to 50 characters")]
        [Required(ErrorMessage = "Please enter the language symbol")]
        public string Name { get; set; }

        [DisplayName("Icon")]
        [Required(ErrorMessage = "Please enter Icon")]
        public string Icon { get; set; }

        [DisplayName("Default")]
        public bool Default { get; set; }
    }
}