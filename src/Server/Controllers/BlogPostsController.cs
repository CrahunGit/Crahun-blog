using WordDaze.Shared;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace WordDaze.Server.Controllers
{
    [ApiController]
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
        public async Task<IActionResult> AddBlogPost([FromForm] BlogPost newBlogPost, IFormFile file, [FromServices] IWebHostEnvironment _hostingEnvironment)
        {
            newBlogPost.Author = Request.HttpContext.User.Identity.Name;

            if (file != null)
            {
                var path = Path.Combine(_hostingEnvironment.WebRootPath, "Images", file.FileName);
                using var stream = System.IO.File.Create(path);
                await file.CopyToAsync(stream);
                newBlogPost.ThumbnailImagePath = file.FileName;
            }

            var savedBlogPost = await _blogPostService.AddBlogPost(newBlogPost);
            return Created(new Uri(Urls.BlogPost.Replace("{id}", savedBlogPost.Id.ToString()), UriKind.Relative), savedBlogPost);
        }

        [Authorize]
        [HttpPut(Urls.UpdateBlogPost)]
        public async Task<IActionResult> UpdateBlogPost(int id, [FromForm] BlogPost updatedBlogPost, IFormFile file, [FromServices] IWebHostEnvironment _hostingEnvironment)
        {
            if (file != null)
            {
                var path = Path.Combine(_hostingEnvironment.WebRootPath, "Images", file.FileName);
                using var stream = System.IO.File.Create(path);
                await file.CopyToAsync(stream);
            }

            await _blogPostService.UpdateBlogPost(id, updatedBlogPost.Post, updatedBlogPost.Title, file?.FileName);

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
