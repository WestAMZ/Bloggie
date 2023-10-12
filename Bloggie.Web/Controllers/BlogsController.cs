using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repositories;
using Imagekit.Constant;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bloggie.Web.Controllers
{
    public class BlogsController : Controller
    {
        private readonly IBlogPostRepository blogPostRepository;
        private readonly IBlogPostLikeRepository blogPostLikeRepository;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IBlogPostCommentRepository blogPostCommentRepository;

        public BlogsController(
            IBlogPostRepository blogPostRepository, 
            IBlogPostLikeRepository blogPostLikeRepository, 
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IBlogPostCommentRepository blogPostCommentRepository
            )
        {
            this.blogPostRepository = blogPostRepository;
            this.blogPostLikeRepository = blogPostLikeRepository;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.blogPostCommentRepository = blogPostCommentRepository;
        }
        [HttpGet("Blogs/{urlHandle}")]
        public async Task<IActionResult> Index(string urlHandle)
        {
            var liked = false;
            var blogPost = await blogPostRepository.GetByUrlHandleAsync(urlHandle);
            var blogDetailsViewModel = new BlogDetailsViewModel();

            if (blogPost != null) 
            {
                var totalLikes = await blogPostLikeRepository.GetTotalLikes(blogPost.Id);

                if (signInManager.IsSignedIn(User)) 
                {
                    var likesForBlog = await blogPostLikeRepository.GetLikesForBlog(blogPost.Id);
                    var userId = userManager.GetUserId(User);

                    if (userId != null) 
                    {
                        liked = likesForBlog.Any(x => x.UserId == Guid.Parse(userId));

                    }

                }

                var blogCommentsDomainModel = await blogPostCommentRepository.GetCommentsByBlogIdAsync(blogPost.Id);

                var blogCommetnsForView = new List<BlogCommentViewModel>();

                foreach (var blogComment in blogCommentsDomainModel) 
                {
                    blogCommetnsForView.Add(new BlogCommentViewModel { 
                        Description = blogComment.Description,
                        DateAdded = blogComment.DateAdded,
                        Username = (await userManager.FindByIdAsync(blogComment.UserId.ToString())).UserName
                    });
                }

                blogDetailsViewModel = new BlogDetailsViewModel
                {
                    Id = blogPost.Id,
                    Content = blogPost.Content,
                    PageTitle = blogPost.PageTitle,
                    Author = blogPost.Author,
                    FeaturedImageUrl = blogPost.FeaturedImageUrl,
                    Heading = blogPost.Heading,
                    PublishedDate = blogPost.PublishedDate,
                    ShortDescription = blogPost.ShortDescription,
                    UrlHandle = blogPost.UrlHandle,
                    Visible = blogPost.Visible,
                    Tags = blogPost.Tags,
                    TotalLikes = totalLikes,
                    Liked = liked,
                    Comments = blogCommetnsForView
                };

                
            }

            return View(blogDetailsViewModel);
        }

        [HttpPost("Blogs/{urlHandle}")]
        public async Task<IActionResult> Index(BlogDetailsViewModel blogDetailsViewModel) 
        {
            if (signInManager.IsSignedIn(User)) 
            {
                var userId = userManager.GetUserId(User);
                var domainModel = new BlogPostComment
                {
                    BlogPostId = blogDetailsViewModel.Id,
                    Description = blogDetailsViewModel.CommentDescription,
                    UserId = Guid.Parse(userId),
                    DateAdded = DateTime.Now,
                };
                await blogPostCommentRepository.AddAsync(domainModel);
                return RedirectToAction("Index", "Blogs", 
                    new { urlHandler = blogDetailsViewModel.UrlHandle });
            }

            return Forbid();
        }
    }
}
