using System.Collections.Generic;

namespace ScrapingChallenge.Domain.Models
{
    public class Section
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public ICollection<Dish> Dishes { get; set; }
    }
}
