using System.ComponentModel.DataAnnotations;

namespace PersonalServiceBus.RSS.Models
{
    public class AddFeedViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public string Url { get; set; }
    }
}