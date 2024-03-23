﻿using Doan_Web_CK.Models;
using Doan_Web_CK.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;

namespace Doan_Web_CK.Controllers
{
    public class BlogController : Controller
    {
        private readonly IBlogRepository _blogRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly INotifiticationRepository _notifiticationRepository;
        private readonly ILogger<BlogController> _logger;
        private readonly IAccountRepository _accountRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public BlogController(UserManager<ApplicationUser> userManager, IBlogRepository blogRepository, ICategoryRepository categoryRepository, ILogger<BlogController> logger, INotifiticationRepository notifiticationRepository, IAccountRepository accountRepository, ICommentRepository commentRepository)
        {
            _blogRepository = blogRepository;
            _categoryRepository = categoryRepository;
            _logger = logger;
            _notifiticationRepository = notifiticationRepository;
            _userManager = userManager;
            _accountRepository = accountRepository;
            _commentRepository = commentRepository;
        }
        [HttpPost]
        public async Task<IActionResult> Search(string blog_title, DateTime? blog_date, string blog_newest, string cate_filter)
        {
            var blogs = await _blogRepository.GetAllAsync();
            var filteredBlogs = blogs.Where(p => p.IsAccepted == true);
            if (cate_filter != null)
            {
                filteredBlogs = filteredBlogs.Where(p => p.CategoryId.ToString() == cate_filter).ToList();
            }
            if (blog_title != null)
            {
                filteredBlogs = filteredBlogs.Where(p => p.Title.ToLower().Contains(blog_title.ToLower())).ToList();
            }

            if (blog_date.HasValue)
            {
                filteredBlogs = filteredBlogs.Where(p => p.PublishDate.Date <= blog_date.Value.Date).ToList();
            }

            if (blog_newest != null)
            {
                if (blog_newest.ToLower() == "true" || blog_newest.ToLower() == "on")
                {
                    filteredBlogs = filteredBlogs.OrderByDescending(p => p.PublishDate).ToList();
                }
            }
            var categories = await _categoryRepository.GetAllAsync();
            var currentUser = await _userManager.GetUserAsync(User);
            ViewBag.GetUserName = new Func<string, string>(GetUserName);
            ViewBag.BlogList = filteredBlogs;
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            ViewBag.GetPhotoById = new Func<string, string>(GetPhotoById);
            ViewBag.GetAllBlogComments = new Func<int, IEnumerable<Comment>>(GetAllBlogComments);
            if (currentUser != null)
            {
                ViewBag.MyBlogs = blogs.Where(p => p.AccountId == currentUser.Id);
                ViewBag.CurrentUser = currentUser;
            }
            return View("Index");
        }
        //public async Task<IActionResult> AddComment(int comment_blogid, string comment_accountid, DateTime comment_commentdate, string comment_content)
        //{
        //    var blog = await _blogRepository.GetByIdAsync(comment_blogid);
        //    var account = await _accountRepository.GetByIdAsync(comment_accountid);
        //    if (blog == null || account == null)
        //    {
        //        return NotFound();
        //    }

        //    var newComment = new Comment
        //    {
        //        Content = comment_content

        //    }


        //}
        public async Task<string> GetUserNameByIdAsync(string id)
        {
            var user = await _accountRepository.GetByIdAsync(id);
            return "@" + user.UserName;
        }
        public string GetUserName(string id)
        {
            var task = GetUserNameByIdAsync(id);
            task.Wait();
            return task.Result;
        }
        public async Task<IEnumerable<Comment>?> GetAllBlogCommentsAsync(int blogId)
        {
            var comments = await _commentRepository.GetAllComments();
            if (comments != null)
            {
                var filered = comments.Where(p => p.BlogId == blogId).OrderByDescending(p => p.CommentDate).ToList().Take(3);
                return filered;
            }
            return comments;
        }
        public IEnumerable<Comment>? GetAllBlogComments(int blogId)
        {
            var task = GetAllBlogCommentsAsync(blogId);
            task.Wait();
            return task.Result;

        }
        public async Task<IActionResult> Index()
        {
            var blogs = await _blogRepository.GetAllAsync();
            var blogList = blogs.Where(p => p.IsAccepted == true).ToList();
            var categories = await _categoryRepository.GetAllAsync();
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser != null)
            {
                ViewBag.CurrentUser = currentUser;
            }
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            ViewBag.BlogList = blogList;
            ViewBag.GetUserName = new Func<string, string>(GetUserName);
            ViewBag.GetPhotoById = new Func<string, string>(GetPhotoById);
            ViewBag.GetAllBlogComments = new Func<int, IEnumerable<Comment>>(GetAllBlogComments);
            if (currentUser != null)
            {
                ViewBag.MyBlogs = blogList.Where(p => p.AccountId == currentUser.Id);
            }
            return View();
        }
        public async Task<IActionResult> Display(int id)
        {
            var blog = await _blogRepository.GetByIdAsync(id);
            ViewBag.Blog = blog;
            return View(blog);
        }
        public async Task<string> GetPhotoByIdAsync(string id)
        {
            var user = await _accountRepository.GetByIdAsync(id);
            return user.ImageUrl;
        }
        public string GetPhotoById(string id)
        {
            var task = GetPhotoByIdAsync(id);
            task.Wait();
            return task.Result;
        }
        //[HttpPost]
        //public async Task<IActionResult> AddComment(int comment_blogid, string comment_accountid, string comment_content)
        //{
        //    var blog = await _blogRepository.GetByIdAsync(comment_blogid);
        //    if (blog != null || comment_content != "")
        //    {

        //        var newComment = new Comment
        //        {
        //            Content = comment_content,
        //            AccountId = comment_accountid,
        //            BlogId = comment_blogid,
        //            CommentDate = DateTime.UtcNow,
        //        };

        //        await _commentRepository.AddAsync(newComment);

        //        var nofitication = new Nofitication
        //        {
        //            BlogId = comment_blogid,
        //            SenderAccountId = comment_accountid,
        //            RecieveAccountId = blog.AccountId,
        //            Type = "Comment",
        //            Date = DateTime.UtcNow,
        //        };
        //        await _notifiticationRepository.AddAsync(nofitication);

        //        return RedirectToAction("Index");
        //    }
        //    return NotFound();
        //}
        [HttpPost]
        public async Task<IActionResult> AddComment(int comment_blogid, string comment_accountid, string comment_content)
        {
            var blog = await _blogRepository.GetByIdAsync(comment_blogid);
            StringBuilder newCommentsHtml = new StringBuilder();
            var newComment = new Comment
            {
                Content = comment_content,
                AccountId = comment_accountid,
                BlogId = comment_blogid,
                CommentDate = DateTime.UtcNow,
            };

            await _commentRepository.AddAsync(newComment);

            var nofitication = new Nofitication
            {
                BlogId = comment_blogid,
                SenderAccountId = comment_accountid,
                RecieveAccountId = blog.AccountId,
                Type = "Comment",
                Date = DateTime.UtcNow,
            };
            await _notifiticationRepository.AddAsync(nofitication);

            var comments = await _commentRepository.GetAllComments();
            var filterd = comments.Where(p => p.BlogId == comment_blogid).OrderByDescending(p => p.CommentDate).ToList().Take(3);
            foreach (var comment in filterd)
            {
                newCommentsHtml.Append("<div class=\"comment_card\">");
                newCommentsHtml.Append("<div class=\"comment_card_img_container\">");
                newCommentsHtml.Append("<img src=\"" + await GetPhotoByIdAsync(comment.AccountId) + "\" class=\"comment_card_img\" />");

                newCommentsHtml.Append("</div>");
                newCommentsHtml.Append("<div class=\"comment_card_content\">");
                newCommentsHtml.Append("<p class=\"fw-bold\">" + await GetUserNameByIdAsync(comment.AccountId) + "</p>");
                newCommentsHtml.Append("<p class=\"fw-normal\">" + comment.Content + "</p>");
                newCommentsHtml.Append("</div>");
                newCommentsHtml.Append("<div class=\"comment_card_actions\">");
                newCommentsHtml.Append("<a href = \"#\" class=\"text-white\">");
                newCommentsHtml.Append("<i class=\"bi bi-three-dots-vertical\"></i>");
                newCommentsHtml.Append("</a>");
                newCommentsHtml.Append("</div>");
                newCommentsHtml.Append("</div>");

            }

            string newCommentsHtmlString = newCommentsHtml.ToString();
            return Json(new { commentHtml = newCommentsHtmlString });
        }
        public async Task<IActionResult> Edit(int id)
        {
            var blog = await _blogRepository.GetByIdAsync(id);
            if (blog == null)
            {
                return NotFound();
            }

            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            var user = _userManager.GetUserAsync(User);
            ViewBag.User = user;
            return View(blog);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Blog blog, IFormFile BlogImageUrl)
        {
            if (BlogImageUrl == null)
            {
                var categories = await _categoryRepository.GetAllAsync();
                ViewBag.Categories = new SelectList(categories, "Id", "Name");
                return View(blog);
            }
            else
            {
                var user = await _userManager.GetUserAsync(User);
                blog.IsAccepted = true;
                blog.AccountId = user.Id;
                blog.BlogImageUrl = await SaveImage(BlogImageUrl);
            }

            await _blogRepository.UpdateAsync(blog);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Details(int id)
        {
            var Blog = await _blogRepository.GetByIdAsync(id);
            if (Blog == null)
            {
                return NotFound();
            }
            return View(Blog);
        }
        public async Task<IActionResult> Add()
        {
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
        }

        [Authorize(Roles = "Admin, Member")]
        [HttpPost]
        public async Task<IActionResult> Add(Blog blog, IFormFile BlogImageUrl)
        {

            var user = await _userManager.GetUserAsync(User);
            var newBlog = new Blog
            {
                Title = blog.Title,
                Description = blog.Description,
                Content = blog.Content,
                CategoryId = blog.CategoryId,
                PublishDate = DateTime.UtcNow,
                AccountId = user.Id,
            };
            if (BlogImageUrl == null)
            {
                var categories = await _categoryRepository.GetAllAsync();
                ViewBag.Categories = new SelectList(categories, "Id", "Name");
                TempData["ShowModal"] = true;
                return RedirectToAction("Index");
            }
            else
            {
                newBlog.BlogImageUrl = await SaveImage(BlogImageUrl);
            }
            await _blogRepository.AddAsync(newBlog);
            return RedirectToAction("Index");


        }
        private async Task<string> SaveImage(IFormFile imageFile)
        {
            var savePath = Path.Combine("wwwroot/images", imageFile.FileName);
            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }
            Console.WriteLine("/images/" + imageFile.FileName);
            return "/images/" + imageFile.FileName;
        }
    }
}