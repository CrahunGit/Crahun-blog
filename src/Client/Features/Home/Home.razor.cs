using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using WordDaze.Shared;
using System.Linq;
using System.Net.Http.Json;
using Markdig;
using Append.Blazor.Notifications;
using Blazored.LocalStorage;

namespace BlogSite.Client.Features.Home
{
    public class HomeModel : ComponentBase
    {
        [Inject] private HttpClient _httpClient { get; set; }

        [Inject] private INotificationService _notificationService { get; set; }

        [Inject] private ILocalStorageService _localStorageService { get; set; }

        protected List<BlogPost> blogPosts { get; set; }

        protected override async Task OnInitializedAsync()
        {     
            await LoadBlogPosts();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                return;
            }

            var hasWebPush = await _notificationService.IsSupportedByBrowserAsync();
            var isAllowed = await _notificationService.RequestPermissionAsync() == PermissionType.Granted;
            var isNotified = await _localStorageService.GetItemAsync<bool>("isNotified") == true;

            if (hasWebPush && isAllowed && !isNotified)
            {
                await _localStorageService.SetItemAsync<bool>("isNotified", true);
                NotificationOptions options = new NotificationOptions { Lang = "es", Body = "Te iremos informando", Icon = "/img/foto.jpg" };
                await _notificationService.CreateAsync("Gracias", options);
            }
        }

        private async Task LoadBlogPosts()
        {
            var blogPostsResponse = await _httpClient.GetFromJsonAsync<List<BlogPost>>(Urls.BlogPosts);
            blogPosts = blogPostsResponse.OrderByDescending(p => p.Posted).ToList();
        }
    }
}