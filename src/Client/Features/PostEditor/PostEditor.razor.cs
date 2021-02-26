using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WordDaze.Shared;

namespace BlogSite.Client.Features.PostEditor
{
    public class PostEditorModel : ComponentBase
    {
        [Inject] private NavigationManager _uriHelper { get; set; }
        [Inject] private AppState _appState { get; set; }
        [Inject] private IJSRuntime JSRuntime { get; set; }

        [Parameter] public string PostId { get; set; }

        protected string Post { get; set; }
        protected string Title { get; set; }

        protected string Summary { get; set; }

        protected int CharacterCount { get; set; }
        protected BlogPost ExistingBlogPost { get; set; } = new BlogPost();
        protected bool IsEdit => string.IsNullOrEmpty(PostId) ? false : true;
        protected string ThumbnailImage { get; set; }
        protected IBrowserFile file { get; set; }

        protected ElementReference editor;

        protected override async Task OnInitializedAsync()
        {
            if (!_appState.IsLoggedIn)
            {
                _uriHelper.NavigateTo("/");
            }

            if (!string.IsNullOrEmpty(PostId))
            {
                await LoadPost();
            }
        }

        public async Task UpdateCharacterCount()
        {
            CharacterCount = await JSRuntime.InvokeAsync<int>("wordDaze.getCharacterCount", editor);
        }

        public async Task SavePost()
        {
            MultipartFormDataContent content = new MultipartFormDataContent();
            content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");

            if (file != null)
            {
                content.Add(new StreamContent(file.OpenReadStream(), Convert.ToInt32(file.Size)), "file", file.Name);
            }

            content.Add(new StringContent(Title ?? string.Empty), nameof(Title));
            content.Add(new StringContent(Post ?? string.Empty), nameof(Post));
            content.Add(new StringContent(Summary ?? string.Empty), nameof(BlogPost.PostSummary));

            HttpResponseMessage response = await _appState._httpClient.PostAsync(Urls.AddBlogPost, content);
            BlogPost savedPost = await response.Content.ReadFromJsonAsync<BlogPost>();

            _uriHelper.NavigateTo($"viewpost/{savedPost.Id}");
        }

        public async Task UpdatePost()
        {
            MultipartFormDataContent content = new MultipartFormDataContent();
            content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");

            if (file != null)
            {
                content.Add(new StreamContent(file.OpenReadStream(), Convert.ToInt32(file.Size)), "file", file.Name);
            }

            content.Add(new StringContent(ExistingBlogPost.Title ?? string.Empty), nameof(Title));
            content.Add(new StringContent(ExistingBlogPost.Post ?? string.Empty), nameof(Post));
            content.Add(new StringContent(ExistingBlogPost.PostSummary ?? string.Empty), nameof(BlogPost.PostSummary));
            content.Add(new StringContent(ExistingBlogPost.Id.ToString() ?? string.Empty), nameof(PostId));

            await _appState._httpClient.PutAsync(Urls.UpdateBlogPost.Replace("{id}", PostId), content);
            _uriHelper.NavigateTo($"viewpost/{ExistingBlogPost.Id}");
        }

        public async Task DeletePost()
        {
            await _appState._httpClient.DeleteAsync(Urls.DeleteBlogPost.Replace("{id}", ExistingBlogPost.Id.ToString()));

            _uriHelper.NavigateTo("/");
        }

        private async Task LoadPost()
        {
            ExistingBlogPost = await _appState._httpClient.GetFromJsonAsync<BlogPost>(Urls.BlogPost.Replace("{id}", PostId));

            if (!string.IsNullOrEmpty(ExistingBlogPost.ThumbnailImagePath))
            {
                ThumbnailImage = $"/Images/{ExistingBlogPost.ThumbnailImagePath}";
            }

            CharacterCount = ExistingBlogPost.Post.Length;
        }

        public async Task<string> UploadImage(IBrowserFile newFile)
        {
            MultipartFormDataContent content = new MultipartFormDataContent();
            content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");

            if (newFile != null)
            {
                content.Add(new StreamContent(newFile.OpenReadStream(), Convert.ToInt32(newFile.Size)), "file", newFile.Name);
            }

            HttpResponseMessage response = await _appState._httpClient.PostAsync(Urls.Image, content);
            string image = await response.Content.ReadAsStringAsync();
            return $"[<img src=\"https://crahun.azurewebsites.net/images/{image}\" class=\"img-fluid\" >](image)";
        }

        public async Task SendImage(InputFileChangeEventArgs eventArgs)
        {
            IBrowserFile file = eventArgs.File;
            var image = await UploadImage(file);

            if (IsEdit)
            {
                ExistingBlogPost.Post += image;
            }
            else
            {
                Post += image;
            }
        }

        public async Task SendMainImage(InputFileChangeEventArgs eventArgs)
        {
            file = eventArgs.File;
            await LoadImage(file);
        }

        private async Task LoadImage(IBrowserFile file)
        {
            long maxFileSize = 1024 * 1024 * 15;
            using Stream fileStream = file.OpenReadStream(maxFileSize);
            using MemoryStream memoryStream = new MemoryStream();
            await fileStream.CopyToAsync(memoryStream);
            ThumbnailImage = $"data:png;base64,{Convert.ToBase64String(memoryStream.ToArray())}";
        }
    }
}