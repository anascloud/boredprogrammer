using System.Diagnostics;
using BoredProgrammer.Data;
using BoredProgrammer.Models;
using BoredProgrammer.Models.ViewModels;
using BoredProgrammer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BoredProgrammer.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;
        private readonly AiUsageTracker _usage;

        public HomeController(AppDbContext db, AiUsageTracker usage)
        {
            _db = db;
            _usage = usage;
        }

        public async Task<IActionResult> Index()
        {
            var posts = await _db.Posts
                .Include(p => p.Category)
                .OrderByDescending(p => p.PublishedDate)
                .ToListAsync();

            var model = new HomeViewModel
            {
                PostCount = posts.Count,
                CategoryCount = await _db.Categories.CountAsync(),
                CommentCount = await _db.Comments.CountAsync(),
                AiResponses = _usage.Snapshot().Total,
                LatestPosts = posts.Take(3).ToList(),
                Categories = await _db.Categories.ToListAsync()
            };

            return View(model);
        }

        public IActionResult Features() => View();

        public IActionResult Resume() => View();

        public async Task<IActionResult> Dashboard()
        {
            var posts = await _db.Posts
                .Include(p => p.Category)
                .OrderByDescending(p => p.PublishedDate)
                .ToListAsync();

            var recentComments = await _db.Comments
                .Include(c => c.Post)
                .OrderByDescending(c => c.CommentDate)
                .Take(6)
                .ToListAsync();

            var usage = _usage.Snapshot();

            var monthly = new List<MonthStat>();
            var month = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(-5);
            for (var i = 0; i < 6; i++)
            {
                var current = month.AddMonths(i);
                monthly.Add(new MonthStat(
                    current.ToString("MMM"),
                    posts.Count(p => p.PublishedDate.Year == current.Year && p.PublishedDate.Month == current.Month)));
            }

            var model = new DashboardViewModel
            {
                PostCount = posts.Count,
                CategoryCount = await _db.Categories.CountAsync(),
                CommentCount = await _db.Comments.CountAsync(),
                AiTotal = usage.Total,
                AiUsage = usage,
                RecentPosts = posts.Take(6).ToList(),
                RecentComments = recentComments,
                CategoryStats = posts
                    .GroupBy(p => p.Category?.Name ?? "Uncategorised")
                    .Select(g => new CategoryStat(g.Key, g.Count()))
                    .OrderByDescending(s => s.Count)
                    .ToList(),
                MonthlyStats = monthly
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
