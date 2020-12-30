using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using WordDaze.Shared;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Headers;
using System.IO;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Collections.Generic;

namespace BlogSite.Client.Features.PostEditor
{
    public class PostEditorModel : ComponentBase
    {
        [Inject] private NavigationManager _uriHelper { get; set; }
        [Inject] private AppState _appState { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }

        [Parameter] public string PostId { get; set; }

        protected string Post { get; set; }
        protected string Title { get; set; }
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

        public async Task UpdateCharacterCount() => CharacterCount = await JSRuntime.InvokeAsync<int>("wordDaze.getCharacterCount", editor);

        public async Task SavePost()
        {
            var content = new MultipartFormDataContent();
            content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");

            if (file != null)
            {
                content.Add(new StreamContent(file.OpenReadStream(), Convert.ToInt32(file.Size)), "file", file.Name);
            }

            content.Add(new StringContent(Title), "Title");
            content.Add(new StringContent(Post), "Post");
            content.Add(new StringContent(DateTime.Now.ToString()), "Posted");

            var response = await _appState._httpClient.PostAsync(Urls.AddBlogPost, content);
            var savedPost = await response.Content.ReadFromJsonAsync<BlogPost>();

            _uriHelper.NavigateTo($"viewpost/{savedPost.Id}");
        }

        public async Task UpdatePost()
        {
            var content = new MultipartFormDataContent();
            content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");

            if (file != null)
            {
                content.Add(new StreamContent(file.OpenReadStream(), Convert.ToInt32(file.Size)), "file", file.Name);
            }

            content.Add(new StringContent(ExistingBlogPost.Title), "Title");
            content.Add(new StringContent(ExistingBlogPost.Post), "Post");
            content.Add(new StringContent(ExistingBlogPost.Posted.ToString()), "Posted");
            content.Add(new StringContent(ExistingBlogPost.Id.ToString()), "PostId");

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
            ThumbnailImage = $"/Images/{ExistingBlogPost.ThumbnailImagePath}";
            CharacterCount = ExistingBlogPost.Post.Length;
        }

        public async Task SendData(InputFileChangeEventArgs eventArgs)
        {
            file = eventArgs.File;
            await LoadImage(file);
        }

        async Task LoadImage(IBrowserFile file)
        {
            long maxFileSize = 1024 * 1024 * 15;
            using var fileStream = file.OpenReadStream(maxFileSize);
            using var memoryStream = new MemoryStream();
            await fileStream.CopyToAsync(memoryStream);
            ThumbnailImage = $"data:png;base64,{Convert.ToBase64String(memoryStream.ToArray())}";
        }
    }
}