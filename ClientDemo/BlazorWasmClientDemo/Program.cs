using BlazorWasmClientDemo;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddOidcAuthentication(options =>
{
    if (!builder.HostEnvironment.IsDevelopment())
    {
        builder.Configuration.Bind("Product", options.ProviderOptions);
    }
    else
    {
        builder.Configuration.Bind("Local", options.ProviderOptions);
    }
});
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
