using Microsoft.JSInterop;

namespace learnUkk.Helper
{
    public class LocalStorageHelper
    {
        private static IJSRuntime JSRuntime { set; get; }

        public LocalStorageHelper(IJSRuntime jSRuntime)
        {
            JSRuntime = jSRuntime;
        }

        public async Task SetItemAsync(string key, string value)
        {
            await JSRuntime.InvokeVoidAsync("localStorage.setItem", key, value);
        }

        public async Task<string> getItemAsync(string key)
        {
            var result = await JSRuntime.InvokeAsync<string>("localStorage.getItem", key);
            return result;
        }
    }
}
