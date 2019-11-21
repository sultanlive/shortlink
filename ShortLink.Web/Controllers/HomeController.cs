using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShortLink.Web.Data;
using ShortLink.Web.Models;
using System.Linq;

namespace ShortLink.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public HomeController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var links = _dbContext.Links.AsNoTracking().OrderByDescending(f => f.CreatedDate)
                .Select(f => new LinkViewModel()
                {
                    Id = f.Id,
                    CreatedDate = f.CreatedDate,
                    LongUrl = f.LongUrl,
                    ShortUrl = $"{HttpContext.Request.Host}/{f.ShortUrl}",
                    CountConversion = f.CountConversion
                }).ToList();

            return View(links);
        }
    }
}
