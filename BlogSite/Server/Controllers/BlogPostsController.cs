using WordDaze.Shared;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace WordDaze.Server.Controllers
{
    public class BlogPostsController : Controller
    {
        private readonly BlogPostService _blogPostService;

        public BlogPostsController(BlogPostService blogPostService)
        {
            _blogPostService = blogPostService;
        }

        [HttpGet(Urls.BlogPosts)]
        public async Task<IActionResult> GetBlogPosts()
        {
            return Ok(await _blogPostService.GetBlogPosts());
        }

        [HttpGet(Urls.BlogPost)]
        public async Task<IActionResult> GetBlogPostById(int id)
        {
            var blogPost = await _blogPostService.GetBlogPost(id);

            if (blogPost == null)
                return NotFound();

            return Ok(blogPost);
        }

        [Authorize]
        [HttpPost(Urls.AddBlogPost)]
        public async Task<IActionResult> AddBlogPost([FromBody]BlogPost newBlogPost)
        {
            newBlogPost.Author = Request.HttpContext.User.Identity.Name;
            var savedBlogPost = await _blogPostService.AddBlogPost(newBlogPost);

            return Created(new Uri(Urls.BlogPost.Replace("{id}", savedBlogPost.Id.ToString()), UriKind.Relative), savedBlogPost);
        }

        [Authorize]
        [HttpPut(Urls.UpdateBlogPost)]
        public async Task<IActionResult> UpdateBlogPost(int id, [FromBody]BlogPost updatedBlogPost)
        {
            await _blogPostService.UpdateBlogPost(id, updatedBlogPost.Post, updatedBlogPost.Title);

            return Ok();
        }

        [Authorize]
        [HttpDelete(Urls.DeleteBlogPost)]
        public async Task<IActionResult> DeleteBlogPost(int id)
        {
            await _blogPostService.DeleteBlogPost(id);

            return Ok();
        }
    }
}
