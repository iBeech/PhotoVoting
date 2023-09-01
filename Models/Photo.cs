using System.ComponentModel.DataAnnotations;

namespace PhotoVotingApp.Models
{
    public class Photo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please provide a title.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Please provide your name so we know who uploaded the photo!")]
        public string Author { get; set; }

        public int Votes { get; set; }

        public string ImageFileName { get; set; } = string.Empty;
    }

}