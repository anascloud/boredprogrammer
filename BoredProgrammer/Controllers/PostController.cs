using BoredProgrammer.Data;
using BoredProgrammer.Models;
using BoredProgrammer.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BoredProgrammer.Controllers
{
    public class PostController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string[] _allowedExtension = { ".jpg", ".jpeg", "png" };
        public PostController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        public IActionResult Index(int? categoryId)
        {
            var postQuery = _context.Posts.Include(p => p.Category).AsQueryable();
            if(categoryId.HasValue)
            {
                postQuery = postQuery.Where(p => p.CategoryId == categoryId);
            }
            var posts = postQuery.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            return View(posts);
        }

        public async Task<IActionResult> Detail(int id)
        {
            if(id== null)
            {
                return NotFound();
            }
            var post = _context.Posts.Include(p => p.Category).Include(p => p.Comments)
                .FirstOrDefault(p => p.Id == id);

            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var postViewModel = new PostViewModel();
            postViewModel.Categories = _context.Categories.Select(c =>
                new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }
            ).ToList();
            return View(postViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Create(PostViewModel postViewModel)
        {
            if(ModelState.IsValid)
            {
                var inputFileExtension = Path.GetExtension(postViewModel.FeatureImage.FileName).ToLower();
                bool isAllowed = _allowedExtension.Contains(inputFileExtension);
                if(!isAllowed)
                {
                    ModelState.AddModelError("", "Invalid Image Format. Allowed formats are .jpeg, .png .and jpg");
                    return View(postViewModel);
                }

                postViewModel.Post.FeatureImagePath =  await UploadFileToFolder(postViewModel.FeatureImage);
                await _context.Posts.AddAsync(postViewModel.Post);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(postViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var postFromDb = await _context.Posts.FirstOrDefaultAsync(p=>p.Id == id);
            EditViewModel editViewModel = new EditViewModel
            {
                Post = postFromDb,
                Categories = _context.Categories.Select(c =>
                new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }
            ).ToList()
            };
            return View(editViewModel);
        }

        public JsonResult AddComment([FromBody]Comment comment)
        {
            comment.CommentDate = DateTime.Now;
            _context.Comments.Add(comment);
            _context.SaveChanges();

            return Json(new
            { 
                username = comment.UserName,
                commentDate = comment.CommentDate.ToString("MMMM dd, yyyy"),
                content = comment.Content
            });
        }

        private async Task<String> UploadFileToFolder(IFormFile file)
        {
            var inputFileExtension = Path.GetExtension(file.FileName);
            var fileName = Guid.NewGuid().ToString() + inputFileExtension;
            var wwwRootPath = _webHostEnvironment.WebRootPath;
            var imagesFolderPath = Path.Combine(wwwRootPath, "images");
            if(!Directory.Exists(imagesFolderPath))
            {
                Directory.CreateDirectory(imagesFolderPath);
            }
            var filePath = Path.Combine(imagesFolderPath, fileName);
            try
            {
                await using(var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
            }
            catch (Exception ex)
            {
                return "Error Uploading Images:" + ex.Message;
            }
            return "/images/" + fileName;
        }
    }
}
