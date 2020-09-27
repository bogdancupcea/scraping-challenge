using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using ScrapingChallenge.Domain.Models;

namespace ScrapingChallenge.Application.Scrape.Services
{
    public class ScrapingService : IScrapingService
    {
        private Uri _url;

        public async Task<IEnumerable<MenuItem>> Scrape(string url)
        {
            _url = new Uri(url);

            var menuItems = new List<MenuItem>();

            var driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            driver.Navigate().GoToUrl(url);

            var menuLinks = driver.FindElementsByCssSelector(".nav .submenu li a");
            WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0,0,0,5));

            foreach (var menuLink in menuLinks)
            {
                var item = new MenuItem();
                var menuUrl = menuLink.GetAttribute("href");
                item.Title = menuLink.Text;

                menuLink.Click();

                var menuDescription = driver.FindElementByCssSelector(".main-content .menu-header p");
                item.Description = menuDescription?.Text; //todo remove html

                var sectionHeaders = driver.FindElementsByCssSelector(".main-content h4.menu-title");
                item.Sections = new List<Section>();


                if (sectionHeaders.Any())
                {
                    foreach (var header in sectionHeaders)
                    {
                        var link = header.FindElement(By.TagName("a"));
                        var section = new Section();
                        section.Title = link.FindElement(By.TagName("span"))?.Text;
                        section.Dishes = new List<Dish>();

                        var dishesDiv = driver.FindElementById(link.GetAttribute("aria-controls"));

                        var dishLinks = dishesDiv.FindElements(By.CssSelector(".menu-item a"));
                        PopulateDishes(driver, section, dishLinks);

                        item.Sections.Add(section);
                    }
                }
                else
                {
                    var section = new Section
                    {
                        Dishes = new List<Dish>()
                    };

                    var dishLinks = driver.FindElementsByCssSelector("div.row .menu-item a");
                    PopulateDishes(driver, section, dishLinks);

                    item.Sections.Add(section);
                }

                menuItems.Add(item);
            }

            return menuItems;
        }

        private void PopulateDishes(ChromeDriver driver, Section section, IEnumerable<IWebElement> dishLinks)
        {
            foreach (var dishLink in dishLinks)
            {
                dishLink.Click();

                var dish = new Dish
                {
                    Name = driver.FindElementByCssSelector(".main-content header h2")?.Text,
                    Description = driver.FindElementByCssSelector(".main-content header + div > p")?.Text
                };
                section.Dishes.Add(dish);

                driver.Navigate().Back();
            }
        }
    }
}
