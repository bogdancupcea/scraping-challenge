using System.Threading.Tasks;
using ScrapingChallenge.Domain.Models;

namespace ScrapingChallenge.Application.Scrape.Infrastructure
{
    public interface IMenuItemRepository
    {
        void AddOrUpdate(MenuItem item);
        Task SaveChangesAsync();
    }
}
