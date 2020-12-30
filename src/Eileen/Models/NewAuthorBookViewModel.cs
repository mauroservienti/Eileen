using System.ComponentModel.DataAnnotations;

namespace Eileen.Models
{
    public class NewAuthorBookRequest
    {
        [Required]
        public string BookTitle { get; set; }
    }

    public class NewAuthorBookViewModel : NewAuthorBookRequest
    {
        public string AuthorName { get; set; }
    }
}