using Microsoft.AspNetCore.Mvc;
using ShortLink.Web.Data;
using ShortLink.Web.Data.Entities;
using ShortLink.Web.Extensions;
using ShortLink.Web.Models;
using ShortLink.Web.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ShortLink.Web.Controllers
{
    public class LinksController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IUrlShortener _urlShortener;

        public LinksController(ApplicationDbContext dbContext, IUrlShortener urlShortener)
        {
            _dbContext = dbContext;
            _urlShortener = urlShortener;
        }

        [Route("{shortLink}")]
        public async Task<IActionResult> Link(string shortLink)
        {
            int linkId = _urlShortener.Decode(shortLink);
            var link = await _dbContext.Links.FindAsync(linkId);

            if (link == null)
                return NotFound(new { shortLink });

            try
            {
                link.CountConversion += 1;
                _dbContext.Links.Update(link);
                await _dbContext.SaveChangesAsync();
                
                return Redirect(link.LongUrl);
            }
            catch
            {
                return RedirectToAction(nameof(HomeController.Index), this.UrlName<HomeController>());
            }
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromForm]LinkViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var lastLink = _dbContext.Links.LastOrDefault();
                var link = new Link()
                {
                    LongUrl = model.LongUrl,
                    CreatedDate = DateTime.UtcNow,
                };

                if (lastLink != null)
                    link.Id = lastLink.Id + new Random().Next(1, 100);

                _dbContext.Links.Add(link);
                await _dbContext.SaveChangesAsync();

                link.ShortUrl = _urlShortener.Encode(link.Id);
                _dbContext.Update(link);
                await _dbContext.SaveChangesAsync();

                return RedirectToAction(nameof(HomeController.Index), this.UrlName<HomeController>());
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "На сервере произошла ошибка, попробуйте позже");
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var link = await _dbContext.Links.FindAsync(id);

            if (link == null)
                return NotFound(new { id });

            return View(new LinkViewModel()
            {
                Id = link.Id,
                CountConversion = link.CountConversion,
                CreatedDate = link.CreatedDate,
                LongUrl = link.LongUrl,
                ShortUrl = $"{HttpContext.Request.Host}/{link.ShortUrl}"
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, [FromForm]LinkViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var link = await _dbContext.Links.FindAsync(id);

            if (link == null)
                return NotFound(new { id });

            try
            {
                link.LongUrl = model.LongUrl;
                _dbContext.Links.Update(link);
                await _dbContext.SaveChangesAsync();

                TempData["Message"] = "Изменения сохранены";

                return View(new LinkViewModel()
                {
                    Id = link.Id,
                    CountConversion = link.CountConversion,
                    CreatedDate = link.CreatedDate,
                    LongUrl = link.LongUrl,
                    ShortUrl = $"{HttpContext.Request.Host}/{link.ShortUrl}"
                });
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "На сервере произошла ошибка, попробуйте позже");
                return View(model);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var link = await _dbContext.Links.FindAsync(id);
            if (link == null)
                return NotFound();

            try
            {
                _dbContext.Links.Remove(link);
                await _dbContext.SaveChangesAsync();

                return Ok(new { });
            }
            catch
            {
                return BadRequest("На сервере произошла ошибка, попробуйте позже");
            }
        }
    }
}
