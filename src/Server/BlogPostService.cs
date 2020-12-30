using Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WordDaze.Shared;

namespace WordDaze.Server
{
    public class BlogPostService
    {
        private readonly BlogContext _context;

        public BlogPostService(BlogContext context)
        {
            _context = context;
        }

        public async Task<List<BlogPost>> GetBlogPosts() 
        {
            return await _context.Posts.AsNoTracking().ToListAsync();
        }

        public async Task<BlogPost> GetBlogPost(int id) 
        {
            return await _context.Posts.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<BlogPost> AddBlogPost(BlogPost newBlogPost)
        {
            _context.Posts.Add(newBlogPost);
            await _context.SaveChangesAsync();
            return newBlogPost;
        }

        public async Task UpdateBlogPost(int postId, string updatedPost, string updateTitle, string thumb)
        {
            var originalBlogPost = _context.Posts.Find(postId);            
            originalBlogPost.Post = updatedPost;
            originalBlogPost.Title = updateTitle;

            if (!string.IsNullOrEmpty(thumb))
            {
                originalBlogPost.ThumbnailImagePath = thumb;
            }
            await _context.SaveChangesAsync();

        }

        public async Task DeleteBlogPost(int postId) 
        {
            var blogPost = _context.Posts.Find(postId);
            _context.Posts.Remove(blogPost);
            await _context.SaveChangesAsync();
        }
    }
}