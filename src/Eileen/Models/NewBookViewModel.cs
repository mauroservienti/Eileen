using System.ComponentModel.DataAnnotations;

namespace Eileen.Models
{
    public class NewBookViewModel
    {
        public string Title { get; set; }
        public bool IsAuthorSelected { get; set; }
        
        [Required]
        public int SelectedAutorId { get; set; }
        public string SelectedAutorName { get; set; }
    }
}