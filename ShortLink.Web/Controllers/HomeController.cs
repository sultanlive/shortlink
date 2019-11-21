using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShortLink.Web.Data;
using ShortLink.Web.Models;
using System.Linq;
using System.Threading.Tasks;

namespace ShortLink.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public HomeController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var links = await _dbContext.Links.AsNoTracking().OrderByDescending(f => f.CreatedDate)
                .Select(f => new LinkViewModel()
                {
                    Id = f.Id,
                    CreatedDate = f.CreatedDate,
                    LongUrl = f.LongUrl,
                    ShortUrl = $"{HttpContext.Request.Host}/{f.ShortUrl}",
                    CountConversion = f.CountConversion
                }).ToListAsync();

            return View(links);
        }
    }
}
