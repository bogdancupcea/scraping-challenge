using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ScrapingChallenge.Application.Scrape.Infrastructure;
using ScrapingChallenge.Domain.Models;
using ScrapingChallenge.Infrastructure.Context;

namespace ScrapingChallenge.Infrastructure.Repositories
{
    public class MenuItemRepository : IMenuItemRepository
    {
        private readonly ScrapingDbContext _context;

        public MenuItemRepository(ScrapingDbContext context)
        {
            _context = context;
        }

        public void AddOrUpdate(MenuItem item)
        {
            var existingItem = _context.MenuItems.Include(m => m.Sections).ThenInclude(s => s.Dishes)
                .FirstOrDefault(i => i.Title == item.Title);

            if (existingItem != null)
            {
                existingItem.Title = item.Title;
                existingItem.Description = item.Description;
                UpdateSections(existingItem, item);
            }
            else
            {
                _context.MenuItems.Add(item);
            }
        }

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

        private void UpdateSections(MenuItem existingItem, MenuItem item)
        {
            foreach (var existingSection in existingItem.Sections)
            {
                var section = item.Sections.FirstOrDefault(s => s.Title == existingSection.Title);
                if (section == null)
                    _context.Sections.Remove(existingSection);
                else
                {
                    UpdateDishes(existingSection, section);
                }
            }

            foreach (var section in item.Sections)
            {
                if (existingItem.Sections.All(s => s.Title != section.Title))
                    existingItem.Sections.Add(section);
            }
        }

        private void UpdateDishes(Section existingSection, Section section)
        {
            foreach (var existingDish in existingSection.Dishes)
            {
                var dish = section.Dishes.FirstOrDefault(s => s.Name == existingDish.Name);
                if (dish == null)
                    _context.Dishes.Remove(existingDish);
                else
                    existingDish.Description = dish.Description;
            }

            foreach (var dish in section.Dishes)
            {
                if (existingSection.Dishes.All(s => s.Name != dish.Name))
                    existingSection.Dishes.Add(dish);
            }
        }
    }
}
