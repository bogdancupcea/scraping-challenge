﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ScrapingChallenge.Application.Scrape.Services;
using ScrapingChallenge.Domain.ApiModels;

namespace ScrapingChallenge.Application.Scrape.Commands
{
    public class ScrapeMenuCommandHandler : IRequestHandler<ScrapeMenuCommand, IEnumerable<MenuItemModel>>
    {
        private readonly IScrapingService _scrapingService;

        public ScrapeMenuCommandHandler(IScrapingService scrapingService)
        {
            _scrapingService = scrapingService;
        }

        public async Task<IEnumerable<MenuItemModel>> Handle(ScrapeMenuCommand request, CancellationToken cancellationToken)
        {
            var menuItems = await _scrapingService.Scrape(request.MenuUrl);
            var models = new List<MenuItemModel>();

            foreach (var item in menuItems)
            {
                foreach (var section in item.Sections)
                {
                    models.AddRange(section.Dishes.Select(dish => new MenuItemModel
                    {
                        MenuTitle = item.Title,
                        MenuDescription = item.Description,
                        MenuSectionTitle = section.Title,
                        DishName = dish.Name,
                        DishDescription = dish.Description
                    }));
                }
            }

            return models;
        }
    }
}
