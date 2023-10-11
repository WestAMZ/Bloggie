using Bloggie.Web.Models.Domain;

namespace Bloggie.Web.Repositories
{
    public interface IBlogPostLikeRepository
    {
        public Task<int> GetTotalLikes(Guid blogPostId);
        public Task<IEnumerable<BlogPostLike>> GetLikesForBlog(Guid blogPostId);
        public Task<BlogPostLike> AddLikeForBlog(BlogPostLike blogPostLike);
    }
}
