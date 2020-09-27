using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;
using ScrapingChallenge.Domain.Models;

namespace ScrapingChallenge.Application.Scrape.Services
{
    public class ScrapingService : IScrapingService
    {
        public async Task<IEnumerable<MenuItem>> Scrape(string url)
        {
            var chromeDriver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            chromeDriver.Navigate().GoToUrl(url);
            return new List<MenuItem>();
        }
    }
}
