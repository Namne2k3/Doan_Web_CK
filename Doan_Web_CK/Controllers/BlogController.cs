using Doan_Web_CK.Models;
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
        private readonly ILikeRepository _likeRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public BlogController(
            UserManager<ApplicationUser> userManager,
            IBlogRepository blogRepository,
            ICategoryRepository categoryRepository,
            ILogger<BlogController> logger,
            INotifiticationRepository notifiticationRepository,
            IAccountRepository accountRepository,
            ICommentRepository commentRepository,
            ILikeRepository likeRepository
        )
        {
            _blogRepository = blogRepository;
            _categoryRepository = categoryRepository;
            _logger = logger;
            _notifiticationRepository = notifiticationRepository;
            _userManager = userManager;
            _accountRepository = accountRepository;
            _commentRepository = commentRepository;
            _likeRepository = likeRepository;
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
            ViewBag.IsCurrentUserLiked = new Func<int, string, bool>(IsCurrentUserLiked);
            if (currentUser != null)
            {
                ViewBag.MyBlogs = blogs.Where(p => p.AccountId == currentUser.Id);
                ViewBag.CurrentUser = currentUser;
            }
            return View("Index");
        }
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
            ViewBag.IsCurrentUserLiked = new Func<int, string, bool>(IsCurrentUserLiked);
            if (currentUser != null)
            {
                ViewBag.MyBlogs = blogList.Where(p => p.AccountId == currentUser.Id);
            }
            return View();
        }
        public bool IsCurrentUserLiked(int blogId, string userId)
        {
            var task = IsCurrentUserLikedAsync(blogId, userId);
            task.Wait();
            return task.Result;
        }
        public async Task<bool> IsCurrentUserLikedAsync(int blogId, string userId)
        {
            var blog = await _blogRepository.GetByIdAsync(blogId);
            var like = await _likeRepository.GetAllLikeAsync();
            var userLike = like.SingleOrDefault(p => p.ApplicationUserId == userId && p.BlogId == blogId);
            if (blog != null || blog.Likes != null)
            {
                if (userLike != null)
                {
                    return true;
                }
                return false;
            }
            return false;
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
        public async Task<IActionResult> UnLike(string like_accountId, int like_blogId)
        {
            var blog = await _blogRepository.GetByIdAsync(like_blogId);
            var likes = await _likeRepository.GetAllLikeAsync();
            var like = likes.SingleOrDefault(p => p.ApplicationUserId == like_accountId && p.BlogId == like_blogId);
            if (blog != null)
            {
                await _blogRepository.DeleteLikeAsync(blog, like);
                return Json(new
                {
                    message = "DeleteLike  Successfully"
                });
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> AddLike(string like_accountId, int like_blogId)
        {
            var blog = await _blogRepository.GetByIdAsync(like_blogId);
            if (blog != null)
            {
                // chay lan dau tien
                if (blog.Likes == null)
                {
                    blog.Likes = new List<Like>();
                }

                var newLike = new Like
                {
                    BlogId = like_blogId,
                    ApplicationUserId = like_accountId,
                };

                await _blogRepository.AddLikeAsync(blog, newLike);
                return Json(new
                {
                    message = "Add Like Successfully"
                });
            }
            return NotFound();
        }


        [HttpPost]
        public async Task<IActionResult> UpdateComment(int edit_cmt_id, string edit_cmt_accountid, int edit_cmt_blogid, string edit_cmt_content)
        {
            var finded = await _commentRepository.GetByIdAsync(edit_cmt_id);
            var currentUser = await _userManager.GetUserAsync(User);
            StringBuilder newCommentsHtml = new StringBuilder();
            if (finded != null)
            {
                finded.CommentDate = DateTime.Now;

                finded.Content = edit_cmt_content;


                await _commentRepository.UpdateAsync(finded);

                var comments = await _commentRepository.GetAllComments();
                var filterd = comments.Where(p => p.BlogId == edit_cmt_blogid).OrderByDescending(p => p.CommentDate).ToList().Take(3);

                foreach (var comment in filterd)
                {

                    newCommentsHtml.Append("<div class=\"comment_card\">");
                    newCommentsHtml.Append("<div class=\"comment_card_img_container\">");
                    newCommentsHtml.Append("<img src=\"" + await GetPhotoByIdAsync(comment.AccountId) + "\" class=\"comment_card_img\" />");
                    newCommentsHtml.Append("</div>");
                    newCommentsHtml.Append("<div class=\"comment_card_content\">");
                    newCommentsHtml.Append("<p class=\"fw-bold\">" + await GetUserNameByIdAsync(comment.AccountId) + " <span class=\"comment_card_date\">" + comment.CommentDate + "</span> </p>");
                    newCommentsHtml.Append("<p id=\"p_content_" + comment.Id + "\" class=\"fw-normal\">" + comment.Content + "</p>");
                    newCommentsHtml.Append("<form method=\"post\" id=\"edit_form_cmt_" + comment.Id + "\">");
                    newCommentsHtml.Append("<input type=\"number\" name=\"edit_cmt_id\" hidden value=\"" + comment.Id + "\" />");
                    newCommentsHtml.Append("<input type=\"text\" name=\"edit_cmt_accountid\" value=\"" + comment.AccountId + "\" hidden />");
                    newCommentsHtml.Append("<input type=\"number\" name=\"edit_cmt_blogid\" value=\"" + comment.BlogId + "\" hidden />");
                    newCommentsHtml.Append("<input onkeypress=\"handleKeyPressInputEditComment(event, " + edit_cmt_blogid + "," + comment.Id + ")" + "\" type=\"text\" name=\"edit_cmt_content\" value=\"" + comment.Content + "\" id=\"comment_form_" + comment.Id + "\" class=\"text-white hidden comments_inputs p-2\" style=\"border: none;\" />");
                    newCommentsHtml.Append("</form>");
                    newCommentsHtml.Append("</div>");
                    newCommentsHtml.Append("<div class=\"comment_card_actions position-relative\">");
                    if (currentUser.Id == comment.AccountId)
                    {
                        newCommentsHtml.Append("<a onclick=\"toggleActionComment(" + comment.Id + ")\" href=\"#\" class=\"text-white\">");
                        newCommentsHtml.Append("<i class=\"bi bi-three-dots-vertical\"></i>");
                        newCommentsHtml.Append("</a>");
                    }
                    newCommentsHtml.Append("<div id=\"comments_actions_" + comment.Id + "\" class=\"comments_actions hidden\">");
                    newCommentsHtml.Append("<a onclick=\"handleEditToggleComment(" + comment.Id + ")\" class=\"btn btn-outline-light\">Edit</a>");
                    newCommentsHtml.Append("<a class=\"btn btn-outline-light\">Delete</a>");
                    newCommentsHtml.Append("</div>");
                    newCommentsHtml.Append("</div>");
                    newCommentsHtml.Append("</div>");
                }

                string newCommentsHtmlString = newCommentsHtml.ToString();
                return Json(new { commentHtml = newCommentsHtmlString });
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> AddComment(int comment_blogid, string comment_accountid, string comment_content)
        {
            var blog = await _blogRepository.GetByIdAsync(comment_blogid);
            var currentUser = await _userManager.GetUserAsync(User);
            StringBuilder newCommentsHtml = new StringBuilder();
            var newComment = new Comment
            {
                Content = comment_content,
                AccountId = comment_accountid,
                BlogId = comment_blogid,
                CommentDate = DateTime.Now,
            };

            await _commentRepository.AddAsync(newComment);

            var nofitication = new Nofitication
            {
                BlogId = comment_blogid,
                SenderAccountId = comment_accountid,
                RecieveAccountId = blog.AccountId,
                Type = "Comment",
                Date = DateTime.Now,
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
                newCommentsHtml.Append("<p class=\"fw-bold\">" + await GetUserNameByIdAsync(comment.AccountId) + " <span class=\"comment_card_date\">" + comment.CommentDate + "</span> </p>");
                newCommentsHtml.Append("<p id=\"p_content_" + comment.Id + "\" class=\"fw-normal\">" + comment.Content + "</p>");
                newCommentsHtml.Append("<form action=\"/UpdateComment\" method=\"post\" id=\"edit_form_cmt_" + comment.Id + "\">");
                newCommentsHtml.Append("<input type=\"number\" name=\"edit_cmt_id\" hidden value=\"" + comment.Id + "\" />");
                newCommentsHtml.Append("<input type=\"text\" name=\"edit_cmt_accountid\" value=\"" + comment.AccountId + "\" hidden />");
                newCommentsHtml.Append("<input type=\"number\" name=\"edit_cmt_blogid\" value=\"" + comment.BlogId + "\" hidden />");
                newCommentsHtml.Append("<input onkeypress=\"handleKeyPressInputEditComment(event, " + comment_blogid + "," + comment.Id + ")" + "\" type=\"text\" name=\"edit_cmt_content\" value=\"" + comment.Content + "\" id=\"comment_form_" + comment.Id + "\" class=\"text-white hidden comments_inputs p-2\" style=\"border: none;\" />");
                newCommentsHtml.Append("</form>");
                newCommentsHtml.Append("</div>");
                newCommentsHtml.Append("<div class=\"comment_card_actions position-relative\">");
                if (currentUser.Id == comment.AccountId)
                {
                    newCommentsHtml.Append("<a onclick=\"toggleActionComment(" + comment.Id + ")\" href=\"#\" class=\"text-white\">");
                    newCommentsHtml.Append("<i class=\"bi bi-three-dots-vertical\"></i>");
                    newCommentsHtml.Append("</a>");
                }
                newCommentsHtml.Append("<div id=\"comments_actions_" + comment.Id + "\" class=\"comments_actions hidden\">");
                newCommentsHtml.Append("<a onclick=\"handleEditToggleComment(" + comment.Id + ")\" class=\"btn btn-outline-light\">Edit</a>");
                newCommentsHtml.Append("<a class=\"btn btn-outline-light\">Delete</a>");
                newCommentsHtml.Append("</div>");
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
                PublishDate = DateTime.Now,
                AccountId = user.Id,
            };
            if (BlogImageUrl == null)
            {
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
