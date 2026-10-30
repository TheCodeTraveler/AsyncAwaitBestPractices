using HackerNews.Components;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using Refit;

namespace HackerNews;

public static class Program
{
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		builder.Services.AddRazorComponents()
			.AddInteractiveServerComponents();

		builder.Services.AddSingleton<HackerNewsAPIService>();

		builder.Services.AddRefitGeneratedClient<IHackerNewsAPI>()
			.ConfigureHttpClient(static client => client.BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0"))
			.AddStandardResilienceHandler(static options => options.Retry = new WebHttpRetryStrategyOptions());

		var app = builder.Build();

		if (!app.Environment.IsDevelopment())
		{
			app.UseExceptionHandler("/Error");
			app.UseHsts();
			app.UseHttpsRedirection();
		}

		app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
		app.UseAntiforgery();

		app.MapStaticAssets();
		app.MapRazorComponents<App>()
			.AddInteractiveServerRenderMode();

		app.Run();
	}

	sealed class WebHttpRetryStrategyOptions : HttpRetryStrategyOptions
	{
		public WebHttpRetryStrategyOptions()
		{
			BackoffType = DelayBackoffType.Exponential;
			MaxRetryAttempts = 3;
			UseJitter = true;
			Delay = TimeSpan.FromSeconds(2);
		}
	}
}