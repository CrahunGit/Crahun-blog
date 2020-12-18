using System;
using System.Net.Http;
using System.Threading.Tasks;
using Markdig;
using Microsoft.AspNetCore.Components;
using WordDaze.Shared;
using System.Net.Http.Json;

namespace BlogSite.Client.Features.ViewPost
{
    public class ViewPostModel : ComponentBase
    {
        [Inject] private HttpClient _httpClient { get; set; }

        [Parameter] public string PostId { get; set; }

        protected BlogPost BlogPost { get; set; } = new BlogPost();

        protected override async Task OnInitializedAsync()
        {
            await LoadBlogPost();
        }

        private async Task LoadBlogPost()
        {
            BlogPost = await _httpClient.GetFromJsonAsync<BlogPost>(Urls.BlogPost.Replace("{id}", PostId));
            BlogPost.Post = Markdown.ToHtml(BlogPost.Post);
        }
    }
}