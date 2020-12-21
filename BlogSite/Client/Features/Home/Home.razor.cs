using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using WordDaze.Shared;
using System.Linq;
using System.Net.Http.Json;
using Markdig;

namespace BlogSite.Client.Features.Home
{
    public class HomeModel : ComponentBase
    {
        [Inject] private HttpClient _httpClient { get; set; }

        protected List<BlogPost> blogPosts { get; set; } = new List<BlogPost>();

        protected override async Task OnInitializedAsync()
        {
            await LoadBlogPosts();
        }

        private async Task LoadBlogPosts()
        {
            var blogPostsResponse = await _httpClient.GetFromJsonAsync<List<BlogPost>>(Urls.BlogPosts);
            blogPosts = blogPostsResponse.OrderByDescending(p => p.Posted).ToList();
        }
    }
}