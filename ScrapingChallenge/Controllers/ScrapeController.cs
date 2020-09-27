using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ScrapingChallenge.Application.Scrape.Commands;
using ScrapingChallenge.Domain.ApiModels;

namespace ScrapingChallenge.Controllers
{
    /// <summary>
    /// Scrape Controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ScrapeController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of <see cref="ScrapeController"/>
        /// </summary>
        /// <param name="mediator"></param>
        public ScrapeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get scraped data async
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<IEnumerable<MenuItemModel>>> Post(ScrapeRequestModel request)
        {
            if (string.IsNullOrEmpty(request.MenuUrl))
                return BadRequest("Please enter a url.");
            if (!request.MenuUrl.Equals("https://www.pure.co.uk/menus/breakfast", StringComparison.InvariantCultureIgnoreCase))
                return BadRequest(
                    "Invalid url. This application only accepts \"https://www.pure.co.uk/menus/breakfast\" as input.");

            var items = await _mediator.Send(new ScrapeMenuCommand(request.MenuUrl));
            return Ok(items);
        }
    }
}
