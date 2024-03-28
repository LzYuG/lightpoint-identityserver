using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Blazor
{
    public class ToolsJsInterop
    {
        private readonly Lazy<Task<IJSObjectReference>> moduleTask;

        public ToolsJsInterop(IJSRuntime jsRuntime)
        {
            
            moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./_content/LightPoint.IdentityServer.Blazor/tools.js").AsTask());
        }

        public async ValueTask<string> TriggerAnimation(string id, string animationName = "fade-in")
        {
            var module = await moduleTask.Value;
            return await module.InvokeAsync<string>("triggerAnimation", id, animationName);
        }

        public async ValueTask<string> PostForm(string requestAddr, object? model, string? antiForgeryToken)
        {
            var module = await moduleTask.Value;
            return await module.InvokeAsync<string>("postForm", requestAddr, model, antiForgeryToken);
        }

        public async ValueTask<string> Redirect(string addr)
        {
            var module = await moduleTask.Value;
            return await module.InvokeAsync<string>("redirect", addr);
        }




        public async ValueTask DisposeAsync()
        {
            if (moduleTask.IsValueCreated)
            {
                var module = await moduleTask.Value;
                await module.DisposeAsync();
            }
        }
    }
}
