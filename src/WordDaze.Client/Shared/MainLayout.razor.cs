using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace WordDaze.Client
{
    public class MainLayoutModel : LayoutComponentBase
    {
        [Inject] protected AppState AppState { get; set; }

        protected async Task Logout()
        {
            await AppState.Logout();
        }
    }
}