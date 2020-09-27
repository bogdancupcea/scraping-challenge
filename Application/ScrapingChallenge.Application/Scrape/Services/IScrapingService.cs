using System.Collections.Generic;
using System.Threading.Tasks;
using ScrapingChallenge.Domain.Models;

namespace ScrapingChallenge.Application.Scrape.Services
{
    public interface IScrapingService
    {
        Task<IEnumerable<MenuItem>> Scrape(string url);
    }
}