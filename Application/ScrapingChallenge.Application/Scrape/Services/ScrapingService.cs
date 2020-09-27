using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using ScrapingChallenge.Domain.Models;

namespace ScrapingChallenge.Application.Scrape.Services
{
    public class ScrapingService : IScrapingService
    {
        private readonly ILogger<ScrapingService> _logger;

        public ScrapingService(ILogger<ScrapingService> logger)
        {
            _logger = logger;
        }

        public IEnumerable<MenuItem> Scrape(string url)
        {
            Log("Starting scraping...");

            var menuItems = new List<MenuItem>();

            using var driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(10);
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl(url);

            var modal = driver.FindElementByCssSelector(".popmake-close");
            modal.Click();
            Log("Closed modal");

            var menuLinks = driver.FindElementsByCssSelector(".nav .submenu li a");
            var linksCount = menuLinks.Count;
            Log($"Found {linksCount} menu items.");

            for (int i = 1; i <= 2; i++)
            {
                var menuLink = driver.FindElementByCssSelector($".nav .submenu li:nth-child({i}) a");
                if (menuLink == null) continue;

                Log($"Start scraping \"{menuLink.Text}\"");

                var item = new MenuItem {Title = menuLink.Text};

                menuLink.Click();

                var menuDescription = driver.FindElementByCssSelector(".main-content .menu-header p");
                item.Description = menuDescription?.Text;

                var sectionHeaders = driver.FindElementsByCssSelector(".main-content h4.menu-title");
                item.Sections = new List<Section>();

                if (sectionHeaders.Any())
                {
                    var headersCount = sectionHeaders.Count;
                    Log($"Item \"{item.Title}\" has {headersCount} sections.");

                    for (var j = 1; j <= headersCount; j++)
                    {
                        var header = driver.FindElementByCssSelector($".main-content h4.menu-title:nth-of-type({j})");
                        var link = header?.FindElement(By.TagName("a"));
                        var dishesDivId = link?.GetAttribute("aria-controls");

                        if(string.IsNullOrEmpty(dishesDivId)) continue;

                        var title = link.FindElement(By.TagName("span"))?.Text;
                        if (string.IsNullOrEmpty(title))
                            title = $"{item.Title}-Section_{j}";
                        var section = new Section
                        {
                            Title = title, 
                            Dishes = new List<Dish>()
                        };

                        Log($"Start scraping section \"{section.Title}\"");

                        PopulateDishes(driver, section, $"#{dishesDivId}");

                        item.Sections.Add(section);

                        Log($"Section \"{section.Title}\" done.");
                    }
                }
                else
                {
                    Log($"Item \"{item.Title}\" has only 1 section.");
                    var section = new Section
                    {
                        Title = $"{item.Title}-Section",
                        Dishes = new List<Dish>()
                    };

                    PopulateDishes(driver, section, ".main-content");

                    item.Sections.Add(section);
                }

                menuItems.Add(item);

                Log($"Item \"{item.Title}\" done.");
            }

            return menuItems;
        }

        private void PopulateDishes(ChromeDriver driver, Section section, string parentSelector)
        {
            Log($"Populating dishes for \"{section.Title}\".");
            var dishLinks = driver.FindElementsByCssSelector($"{parentSelector} .menu-item a");
            var dishesCount = dishLinks.Count;
            Log($"Section \"{section.Title}\" has {dishesCount} dishes.");
            for (int i = 1; i <= dishesCount; i++)
            {
                var now = DateTime.Now;

                WebDriverWait wait  = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                wait.Until(wd => (DateTime.Now - now) - TimeSpan.FromMilliseconds(5) > TimeSpan.Zero);


                var dishLink = driver.FindElementByCssSelector($"{parentSelector} .menu-item:nth-child({i}) a");
                if(dishLink == null) continue;

                if (dishLink.Location.Y > 200)
                {
                    driver.ExecuteScript(
                        $"window.scrollTo({0}, {dishLink.Location.Y - 100})");
                }

                dishLink.Click();
                section.Dishes.Add(GetDish(driver));

                driver.Navigate().Back();
            }
        }

        private Dish GetDish(ChromeDriver driver)
        {
            var text = driver.FindElementByCssSelector(".main-content header h2").Text;
            var descriptions = driver.FindElementsByCssSelector(".main-content header + div p");

            var description = descriptions.Any() 
                ? descriptions.First().Text 
                : driver.FindElementByCssSelector(".main-content header + div .cardBack").Text;

            Log($"Populating dish \"{text}\".");

            return new Dish
            {
                Name = text,
                Description = description
            };
        }

        private void Log(string message)
        {
            _logger.LogInformation(message);
        }
    }
}
