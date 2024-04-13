using Doan_Web_CK.Models;
using Doan_Web_CK.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Doan_Web_CK.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAccountRepository _accountRepository;
        private readonly IBlogRepository _blogRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly ILikeRepository _likeRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IFriendShipRepository _friendShipRepository;
        private readonly INotifiticationRepository _notifiticationRepository;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public AccountController(
            UserManager<ApplicationUser> userManager,
            IAccountRepository accountRepository,
            IBlogRepository blogRepository,
            ICommentRepository commentRepository,
            ILikeRepository likeRepository,
            ICategoryRepository categoryRepository,
            IFriendShipRepository friendShipRepository,
            INotifiticationRepository noticeRepository,
            SignInManager<ApplicationUser> signInManager
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
            _signInManager = signInManager;
        }

        [HttpGet]
        [Route("/Admin/Account/GetAllAccountAsync")]
        public async Task<IActionResult> GetAllAccountAsync(string search)
        {
            var accounts = await _accountRepository.GetAllAsync();
            var currentUser = await _userManager.GetUserAsync(User);
            var filterd = accounts.Where(p => p.Email != currentUser.Email).ToList();
            if (search == null)
            {
                return Json(new
                {
                    message = "Found",
                    accounts = filterd
                });
            }
            var matched = filterd.Where(p => p.UserName.ToLower().Contains(search.ToLower()) || p.Email.ToLower().Contains(search.ToLower())).ToList();

            return Json(new
            {
                message = "Found",
                accounts = matched
            });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var account = await _accountRepository.GetByIdAsync(id);
            await _friendShipRepository.RemoveFriendsByUserId(account.Id);
            await _blogRepository.RemoveBlogsByUserId(account.Id);
            await _commentRepository.RemoveCommentsByUserId(account.Id);
            await _likeRepository.RemoveLikesByUserId(account.Id);
            await _notifiticationRepository.RemoveNofsByUserId(account.Id);
            if (account == null)
            {
                return NotFound();
            }
            await _accountRepository.DeleteAsync(account.Id);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Index()
        {
            var accounts = await _accountRepository.GetAllAsync();
            var user = await _userManager.GetUserAsync(User);
            var adminId = "";
            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                // Người dùng hiện tại là admin
                adminId = user.Id;
            }
            var accountList = accounts.Where(p => p.Id != adminId).ToList();
            return View(accountList);
        }

        public async Task<IActionResult> Details(string id)
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
            ViewBag.IsFriend = new Func<string, string, bool>(IsFriend);
            ViewBag.IsBeingRequested = new Func<string, string, bool>(IsBeingRequested);
            return View();
        }
        public async Task<bool> IsBeingRequestedAsync(string currentUserId, string accountId)
        {
            var friendships = await _friendShipRepository.GetAllAsync();
            var finded = friendships.SingleOrDefault(p => p.FriendId == currentUserId && p.UserId == accountId);
            if (finded != null)
            {
                return true;
            }
            return false;
        }
        public bool IsBeingRequested(string currentUserId, string accountId)
        {
            var task = IsBeingRequestedAsync(currentUserId, accountId);
            task.Wait();
            return task.Result;
        }
        public async Task<bool> IsFriendAsync(string userId, string friendId)
        {
            var friendship = await _friendShipRepository.GetAllAsync();
            var finded = friendship.SingleOrDefault(p => p.UserId == userId && p.FriendId == friendId || p.UserId == friendId && p.FriendId == userId);
            if (finded != null && finded.IsConfirmed == true)
            {
                return true;
            }
            return false;
        }
        public bool IsFriend(string userId, string friendId)
        {
            var task = IsFriendAsync(userId, friendId);
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
        public async Task<bool> IsRequestedAsync(string userId, string friendId)
        {
            var friendships = await _friendShipRepository.GetAllAsync();
            var finded = friendships.SingleOrDefault(p => p.UserId == userId && p.FriendId == friendId || p.UserId == friendId && p.FriendId == userId && p.IsConfirmed == false);
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
        public async Task<IEnumerable<Comment>> GetAllBlogCommentsAsync(int blogId)
        {
            var comments = await _commentRepository.GetAllComments();
            if (comments != null)
            {
                var filered = comments.Where(p => p.BlogId == blogId).OrderByDescending(p => p.CommentDate).ToList().Take(3);
                return filered;
            }
            return comments;
        }
        public IEnumerable<Comment> GetAllBlogComments(int blogId)
        {
            var task = GetAllBlogCommentsAsync(blogId);
            task.Wait();
            return task.Result;

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
