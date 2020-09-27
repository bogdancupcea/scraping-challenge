using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ScrapingChallenge.Application.Scrape.Infrastructure;
using ScrapingChallenge.Application.Scrape.Services;
using ScrapingChallenge.Domain.ApiModels;
using ScrapingChallenge.Domain.Models;

namespace ScrapingChallenge.Application.Scrape.Commands
{
    public class ScrapeMenuCommandHandler : IRequestHandler<ScrapeMenuCommand, IEnumerable<MenuItemModel>>
    {
        private readonly IScrapingService _scrapingService;
        private readonly IMenuItemRepository _menuItemRepository;

        public ScrapeMenuCommandHandler(IScrapingService scrapingService, IMenuItemRepository menuItemRepository)
        {
            _scrapingService = scrapingService ?? throw new ArgumentNullException(nameof(scrapingService));
            _menuItemRepository = menuItemRepository ?? throw new ArgumentNullException(nameof(menuItemRepository));
        }

        public async Task<IEnumerable<MenuItemModel>> Handle(ScrapeMenuCommand request, CancellationToken cancellationToken)
        {
            var menuItems = _scrapingService.Scrape(request.MenuUrl);
            await SaveToDb(menuItems);

            var models = new List<MenuItemModel>();
            foreach (var item in menuItems)
            {
                foreach (var section in item.Sections)
                {
                    models.AddRange(section.Dishes.Select(dish => new MenuItemModel
                    {
                        MenuTitle = item.Title,
                        MenuDescription = item.Description,
                        MenuSectionTitle = section.Title.Contains("Section") ? string.Empty : section.Title,
                        DishName = dish.Name,
                        DishDescription = dish.Description
                    }));
                }
            }

            return models;
        }

        private async Task SaveToDb(IEnumerable<MenuItem> menuItems)
        {
            foreach (var item in menuItems)
            {
                _menuItemRepository.AddOrUpdate(item);
            }

            await _menuItemRepository.SaveChangesAsync();
        }
    }
}
