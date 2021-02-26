using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using WordDaze.Shared;

namespace BlogSite.Server.Controllers
{
    public class UploadImage : ControllerBase
    {
        [Authorize]
        [HttpPost(Urls.Image)]
        public async Task<IActionResult> AddImage(IFormFile file, [FromServices] IWebHostEnvironment _hostingEnvironment)
        {
            if (file != null)
            {
                string path = Path.Combine(_hostingEnvironment.WebRootPath, "Images", file.FileName);
                using FileStream stream = System.IO.File.Create(path);
                await file.CopyToAsync(stream);
            }
            return Ok(file.FileName);
        }
    }
}
