using System.ComponentModel.DataAnnotations;

namespace Eileen.Models
{
    public class NewBookViewModel
    {
        [Required]
        public string Title { get; set; }
        public bool IsAuthorSelected { get; set; }
        
        [Required]
        public int? SelectedAuthorId { get; set; }
        public string SelectedAuthorName { get; set; }
    }
}