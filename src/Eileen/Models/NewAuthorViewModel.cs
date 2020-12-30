using System.ComponentModel.DataAnnotations;

namespace Eileen.Models
{
    public class NewAuthorViewModel
    {
        [Required]
        public string Name { get; set; }
    }
}