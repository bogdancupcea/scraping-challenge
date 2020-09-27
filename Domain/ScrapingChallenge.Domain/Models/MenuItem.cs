using System.Collections.Generic;

namespace ScrapingChallenge.Domain.Models
{
    public class MenuItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public ICollection<Section> Sections { get; set; }
    }
}
