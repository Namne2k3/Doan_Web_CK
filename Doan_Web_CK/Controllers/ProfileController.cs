using Doan_Web_CK.Models;
using Doan_Web_CK.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Doan_Web_CK.Controllers
{
    [Authorize(Roles = "Member, Admin")]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAccountRepository _accountRepository;
        private readonly IBlogRepository _blogRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly ILikeRepository _likeRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IFriendShipRepository _friendShipRepository;
        private readonly INotifiticationRepository _notifiticationRepository;
        public ProfileController(
            UserManager<ApplicationUser> userManager,
            IAccountRepository accountRepository,
            IBlogRepository blogRepository,
            ICommentRepository commentRepository,
            ILikeRepository likeRepository,
            ICategoryRepository categoryRepository,
            IFriendShipRepository friendShipRepository,
            INotifiticationRepository noticeRepository
        )
        {
            _userManager = userManager;
            _accountRepository = accountRepository;
            _blogRepository = blogRepository;
            _commentRepository = commentRepository;
            _likeRepository = likeRepository;
            _categoryRepository = categoryRepository;
            _friendShipRepository = friendShipRepository;
            _notifiticationRepository = noticeRepository;
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(string profile_email, string profile_cur_password, string profile_new_password, string profile_phone_num, string profile_confirm_password)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser.Email != null)
                currentUser.Email = profile_email;

            currentUser.PhoneNumber = profile_phone_num;
            await _userManager.UpdateAsync(currentUser);

            if (profile_cur_password != null && profile_new_password != null && profile_confirm_password != null)
            {
                if (await _userManager.CheckPasswordAsync(currentUser, profile_cur_password) == true)
                {
                    if (profile_new_password == profile_confirm_password)
                    {
                        await _userManager.ChangePasswordAsync(currentUser, profile_cur_password, profile_confirm_password);
                    }
                    else
                    {
                        return Json(new
                        {
                            message = "failed"
                        });
                    }
                }
                else
                {
                    return Json(new
                    {
                        message = "failed"
                    });
                }
            }

            return Json(new
            {
                message = "success"
            });
        }
        public async Task<IActionResult> UpdateProfile(string id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null)
            {
                if (currentUser.Id != id)
                {
                    return NotFound();
                }
            }

            var account = await _accountRepository.GetByIdAsync(currentUser?.Id);

            if (id != null)
            {
                account = await _accountRepository.GetByIdAsync(id);
            }
            ViewBag.currentUser = currentUser;
            ViewBag.GetAllNofOfUser = new Func<string, IEnumerable<Nofitication>>(GetAllNofOfUser);
            ViewBag.GetUserName = new Func<string, string>(GetUserName);
            ViewBag.IsRequested = new Func<string, string, bool>(IsRequested);
            ViewBag.GetPhotoById = new Func<string, string>(GetPhotoById);
            ViewBag.Account = account;
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
        public async Task<string> GetUserNameByBlogIdAsync(int blogId)
        {
            var blog = await _blogRepository.GetByIdAsync(blogId);
            if (blog != null)
            {
                var author = await _accountRepository.GetByIdAsync(blog.AccountId);
                return author.UserName;
            }
            return "No Blog was Found";
        }
        public string GetUserNameByBlogId(int blogId)
        {
            var task = GetUserNameByBlogIdAsync(blogId);
            task.Wait();
            return task.Result;
        }

        [HttpGet]
        public async Task<IEnumerable<Nofitication>> GetAllNofOfUserAsync(string userId)
        {
            var user = await _accountRepository.GetByIdAsync(userId);

            var nofitications = await _notifiticationRepository.GetAllNotifitions();
            var filtered = nofitications.Where(p => p.RecieveAccountId == userId).ToList();
            return filtered;
        }
        public IEnumerable<Nofitication> GetAllNofOfUser(string userId)
        {
            var task = GetAllNofOfUserAsync(userId);
            task.Wait();
            return task.Result;
        }
        [HttpGet]
        public async Task<IActionResult> DenyFriendRequest(string userId, int nofId)
        {
            var friendShip = await _friendShipRepository.GetAllAsync();
            var finded = friendShip.SingleOrDefault(p => p.FriendId == userId && p.IsConfirmed == false);
            StringBuilder newHtml = new StringBuilder();
            if (finded != null)
            {
                await _friendShipRepository.DeleteAsync(finded);
                //< a onclick = "handleAccept(' @currentUser?.Id ', @nof.Id)" class="btn btn-outline-dark">Accept</a>
                //<a class="btn btn-outline-dark">Deny</a>
                newHtml.Append("<a class=\"disabled btn btn-outline-dark\">Denied</a>");
                return Json(new
                {
                    message = "Denied friendship successfully",
                    newHtml = newHtml.ToString()
                });
            }
            return Json(new
            {
                message = "Not found user"
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetReloadNofs(string userId)
        {
            var user = await _accountRepository.GetByIdAsync(userId);

            var nofitications = await _notifiticationRepository.GetAllNotifitions();
            var filtered = nofitications.Where(p => p.RecieveAccountId == userId).ToList();
            StringBuilder sb = new StringBuilder();

            foreach (var nof in filtered)
            {
                switch (nof.Type)
                {
                    case "Addfriend":
                        if (IsRequested(nof.SenderAccountId, nof.RecieveAccountId) == true)
                        {
                            sb.Append("<div class=\"nofi_card\">");
                            sb.Append("<p class=\"nofi_card_content\">");

                            // Use string formatting for clarity and potential data validation
                            sb.Append("<a href=\"/Profile/Index/" + nof.SenderAccountId + "\">" + GetUserName(nof.SenderAccountId) + "</a> " + nof.Content);
                            sb.Append("<span class=\"nofi_card_date\"> ");
                            sb.Append(nof.Date);
                            sb.AppendLine("</span>");  // Add newline for proper formatting

                            sb.Append("<div id=\"nofi_card_actions_");
                            sb.Append(nof.Id);
                            sb.Append("\" class=\"nofi_card_actions\">");

                            sb.Append("<a onclick=\"handleAccept(");
                            sb.Append(user?.Id ?? "null");  // Handle null case for currentUser
                            sb.Append(", ");
                            sb.Append(nof.Id);
                            sb.Append(")\" class=\"btn btn-outline-dark\">Accept</a>");

                            sb.Append("<a onclick=\"handleDeny(");
                            sb.Append(user?.Id ?? "null");  // Handle null case for currentUser
                            sb.Append(", ");
                            sb.Append(nof.Id);
                            sb.Append(")\" class=\"btn btn-outline-dark\">Deny</a>");

                            sb.AppendLine("</div>");

                            sb.Append("<div>");
                            sb.Append("<a onclick = \"handleDeleteNofitication(" + @nof.Id + ")\">");
                            sb.Append("<i class=\"close_icon bi bi-x\"></i>");
                            sb.Append("</a>");
                            sb.Append("</div>");
                            sb.AppendLine("</div>");
                        }
                        break;
                    case "Like":
                        sb.Append("<div class=\"nofi_card\">");
                        sb.Append("<p class=\"nofi_card_content\">");

                        // Use string formatting for clarity and potential data validation
                        sb.Append("<a href=\"/Profile/Index/" + nof.SenderAccountId + "\">" + GetUserName(nof.SenderAccountId) + "</a> " + nof.Content);

                        // Append blog link with string formatting
                        sb.AppendFormat(" <a asp-route-id=\"{0}\" asp-action=\"Details\" asp-controller=\"Blog\">Link to blog</a>", nof.BlogId);

                        sb.Append("<span class=\"nofi_card_date\"> ");
                        sb.Append(nof.Date);
                        sb.AppendLine("</span>");  // Add newline for proper formatting

                        sb.AppendLine("</p>");

                        sb.Append("<div>");
                        sb.Append("<a onclick = \"handleDeleteNofitication(" + @nof.Id + ")\">");
                        sb.Append("<i class=\"close_icon bi bi-x\"></i>");
                        sb.Append("</a>");
                        sb.Append("</div>");
                        sb.AppendLine("</div>");

                        break;
                    case "Comment":
                        sb.Append("<div class=\"nofi_card\">");
                        sb.Append("<p class=\"nofi_card_content\">");

                        // Use string formatting for clarity and potential data validation

                        sb.Append("<a href=\"/Profile/Index/" + nof.SenderAccountId + "\">" + GetUserName(nof.SenderAccountId) + "</a> " + nof.Content);

                        // Append blog link with string formatting
                        sb.AppendFormat(" <a asp-route-id=\"{0}\" asp-action=\"Details\" asp-controller=\"Blog\">Link to blog</a>", nof.BlogId);

                        sb.Append("<span class=\"nofi_card_date\"> ");
                        sb.Append(nof.Date);
                        sb.AppendLine("</span>");  // Add newline for proper formatting

                        sb.AppendLine("</p>");

                        sb.Append("<div>");
                        sb.Append("<a onclick = \"handleDeleteNofitication(" + @nof.Id + ")\">");
                        sb.Append("<i class=\"close_icon bi bi-x\"></i>");
                        sb.Append("</a>");
                        sb.Append("</div>");
                        sb.AppendLine("</div>");
                        break;
                    default:
                        break;
                }
            }
            string finalHtml = sb.ToString();
            return Json(new
            {
                message = "Not found user",
                newHtml = finalHtml,
            });
        }
        [HttpGet]
        public async Task<IActionResult> AcceptFriendRequest(string userId, int nofId)
        {
            var friendShip = await _friendShipRepository.GetAllAsync();
            var finded = friendShip.SingleOrDefault(p => p.FriendId == userId);
            StringBuilder newHtml = new StringBuilder();
            if (finded != null)
            {
                finded.IsConfirmed = true;
                await _friendShipRepository.UpdateAsync(finded);

                //< a onclick = "handleAccept(' @currentUser?.Id ', @nof.Id)" class="btn btn-outline-dark">Accept</a>
                //<a class="btn btn-outline-dark">Deny</a>
                newHtml.Append("<a class=\"disabled btn btn-outline-dark\">Accepted</a>");
                return Json(new
                {
                    friendShip = friendShip.ToString(),
                    message = "Accepted friendship successfully",
                    newHtml = newHtml.ToString()
                });
            }
            return Json(new
            {
                message = "Not found user"
            });
        }
        public async Task<IActionResult> Index(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            var account = await _accountRepository.GetByIdAsync(user.Id);

            if (id != null)
            {
                account = await _accountRepository.GetByIdAsync(id);
            }

            var blogs = await _blogRepository.GetAllAsync();

            var accountBlogs = blogs.Where(p => p.AccountId == account.Id).ToList();
            ViewBag.Account = account;

            ViewBag.blogList = accountBlogs;
            ViewBag.currentUser = user;
            ViewBag.GetPhotoById = new Func<string, string>(GetPhotoById);
            ViewBag.GetUserName = new Func<string, string>(GetUserName);
            ViewBag.GetAllBlogComments = new Func<int, IEnumerable<Comment>>(GetAllBlogComments);
            ViewBag.IsCurrentUserLiked = new Func<int, string, bool>(IsCurrentUserLiked);
            ViewBag.GetUserNameByBlogId = new Func<int, string>(GetUserNameByBlogId);
            ViewBag.GetBlogLikesCount = new Func<int, int>(GetBlogLikesCount);
            ViewBag.GetBlogCommentsCount = new Func<int, int>(GetBlogCommentsCount);
            ViewBag.IsRequested = new Func<string, string, bool>(IsRequested);
            ViewBag.GetAllNofOfUser = new Func<string, IEnumerable<Nofitication>>(GetAllNofOfUser);
            return View();
        }
        public async Task<bool> IsRequestedAsync(string userId, string friendId)
        {
            var friendships = await _friendShipRepository.GetAllAsync();
            var finded = friendships.SingleOrDefault(p => p.UserId == userId && p.FriendId == friendId && p.IsConfirmed == false);
            if (finded != null)
            {
                return true;
            }
            return false;
        }
        public bool IsRequested(string userId, string friendId)
        {
            var task = IsRequestedAsync(userId, friendId);
            task.Wait();
            return task.Result;
        }
        public int GetBlogCommentsCount(int blogId)
        {
            var task = GetBlogCommentsCountAsync(blogId);
            task.Wait();
            return task.Result; ;
        }
        public async Task<int> GetBlogCommentsCountAsync(int blogId)
        {
            var blog = await _blogRepository.GetByIdAsync(blogId);
            var comments = await _commentRepository.GetAllComments();
            int count = comments.Where(p => p.BlogId == blogId).Count();
            if (blog.Comments == null)
            {
                blog.Comments = new List<Comment>();
            }
            return count;
        }
        public int GetBlogLikesCount(int blogId)
        {
            var task = GetBlogLikesCountAsync(blogId);
            task.Wait();
            return task.Result; ;
        }
        public async Task<int> GetBlogLikesCountAsync(int blogId)
        {
            var blog = await _blogRepository.GetByIdAsync(blogId);
            if (blog.Likes == null)
            {
                blog.Likes = new List<Like>();
            }
            return blog.Likes.Count();
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
        [HttpPost]
        public async Task<IActionResult> Edit(IFormFile profile_photo, string user_name)
        {
            var user = await _userManager.GetUserAsync(User);
            if (profile_photo != null)
            {
                user.ImageUrl = await SaveImage(profile_photo);
            }
            user.UserName = user_name;
            await _accountRepository.UpdateAsync(user);

            var account = await _accountRepository.GetByIdAsync(user.Id);
            ViewBag.Account = account;
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
    }
}
