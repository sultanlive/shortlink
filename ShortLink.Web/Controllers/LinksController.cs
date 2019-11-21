using Microsoft.AspNetCore.Mvc;
using ShortLink.Web.Data;
using ShortLink.Web.Data.Entities;
using ShortLink.Web.Extensions;
using ShortLink.Web.Models;
using ShortLink.Web.Services;
using System;
using System.Linq;

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
        public IActionResult Link(string shortLink)
        {
            int linkId = _urlShortener.Decode(shortLink);
            var link = _dbContext.Links.Find(linkId);

            if (link == null)
                return NotFound(new { shortLink });

            try
            {
                link.CountConversion += 1;
                _dbContext.Links.Update(link);
                _dbContext.SaveChanges();
                
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
        public IActionResult Add([FromForm]LinkViewModel model)
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
                _dbContext.SaveChanges();

                link.ShortUrl = _urlShortener.Encode(link.Id);
                _dbContext.Update(link);
                _dbContext.SaveChanges();

                return RedirectToAction(nameof(HomeController.Index), this.UrlName<HomeController>());
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "На сервере произошла ошибка, попробуйте позже");
                return View(model);
            }
        }

        public IActionResult Edit(int id)
        {
            var link = _dbContext.Links.Find(id);

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
        public IActionResult Edit(int id, [FromForm]LinkViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var link = _dbContext.Links.Find(id);

            if (link == null)
                return NotFound(new { id });

            try
            {
                link.LongUrl = model.LongUrl;
                _dbContext.Links.Update(link);
                _dbContext.SaveChanges();

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
        public IActionResult Delete(int id)
        {
            var link = _dbContext.Links.Find(id);
            if (link == null)
                return NotFound();

            try
            {
                _dbContext.Links.Remove(link);
                _dbContext.SaveChanges();

                return Ok(new { });
            }
            catch
            {
                return BadRequest("На сервере произошла ошибка, попробуйте позже");
            }
        }
    }
}
