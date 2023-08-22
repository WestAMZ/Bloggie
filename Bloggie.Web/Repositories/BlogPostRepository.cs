using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Repositories
{
    public class BlogPostRepository: IBlogPostRepository
    {
        private readonly BloggieDbContext bloggieDbContext;
        public BlogPostRepository(BloggieDbContext bloggieDbContext)
        {
            this.bloggieDbContext = bloggieDbContext;
        }

        public async Task<BlogPost> AddAsync(BlogPost blogPost)
        {
            await this.bloggieDbContext.BlogPosts.AddAsync(blogPost);
            await this.bloggieDbContext.SaveChangesAsync();
            return blogPost;
        }

        public async Task<BlogPost?> DeleteAsync(Guid id)
        {
            var blogPost = await this.bloggieDbContext.BlogPosts.FirstOrDefaultAsync(x => x.Id == id);
            if (blogPost != null)
            {
                this.bloggieDbContext.Remove(blogPost);
                await this.bloggieDbContext.SaveChangesAsync();
                return blogPost;
            }
            return null;
        }

        public async Task<IEnumerable<BlogPost>> GetAllAsync()
        {
            return await bloggieDbContext.BlogPosts.Include(x=> x.Tags).ToListAsync();
        }

        public async Task<BlogPost?> GetAsync(Guid id)
        {
            return await this.bloggieDbContext.BlogPosts.Include(x=> x.Tags)
                .FirstOrDefaultAsync(x=>x.Id == id);
        }

        public async Task<BlogPost?> GetByUrlHandleAsync(string urlHandle)
        {
            return await this.bloggieDbContext.BlogPosts.Include(x => x.Tags)
                .FirstOrDefaultAsync(x => x.UrlHandle == urlHandle);
        }

        public async Task<BlogPost?> UpdateAsync(BlogPost blogPost)
        {
            var existingBlogPost = await this.bloggieDbContext.BlogPosts.Include(x=>x.Tags)
                .FirstOrDefaultAsync(x => x.Id == blogPost.Id);

            if (existingBlogPost != null) 
            {
                existingBlogPost.Heading = blogPost.Heading;
                existingBlogPost.PageTitle = blogPost.PageTitle;
                existingBlogPost.Content = blogPost.Content;
                existingBlogPost.ShortDescription = blogPost.ShortDescription;
                existingBlogPost.FeaturedImageUrl = blogPost.FeaturedImageUrl;
                existingBlogPost.UrlHandle = blogPost.UrlHandle;
                existingBlogPost.Author = blogPost.Author;
                existingBlogPost.Visible = blogPost.Visible;
                existingBlogPost.PublishedDate = blogPost.PublishedDate;
                existingBlogPost.Tags = blogPost.Tags;

                await this.bloggieDbContext.SaveChangesAsync();
                return existingBlogPost;
            }

            return null;
        }
    }
}
