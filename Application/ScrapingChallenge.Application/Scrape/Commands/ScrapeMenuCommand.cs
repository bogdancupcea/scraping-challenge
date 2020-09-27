using System.Collections.Generic;
using MediatR;
using ScrapingChallenge.Domain.ApiModels;

namespace ScrapingChallenge.Application.Scrape.Commands
{
    public class ScrapeMenuCommand : IRequest<IEnumerable<MenuItemModel>>
    {
        public ScrapeMenuCommand(string menuUrl)
        {
            MenuUrl = menuUrl;
        }

        public string MenuUrl { get; set; }
    }
}
