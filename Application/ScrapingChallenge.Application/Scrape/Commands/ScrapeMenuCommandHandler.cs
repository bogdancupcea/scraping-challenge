using System.Collections.Generic;
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

            return new List<MenuItemModel>()
            {
                new MenuItemModel
                {
                    MenuTitle = "test",
                    MenuDescription = "testdesc"
                }
            };
        }
    }
}
