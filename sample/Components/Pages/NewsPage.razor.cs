using Microsoft.AspNetCore.Components;

namespace HackerNews.Components.Pages;

// Bad and Good share this markup and state, so each page's own file holds only the async code the talk refactors
public abstract partial class NewsPage : ComponentBase, IDisposable
{
	readonly CancellationTokenSource _disposeCancellationTokenSource = new();

	[Inject]
	public required HackerNewsAPIService HackerNewsApiService { get; init; }

	[Inject]
	public required ILogger<NewsPage> Logger { get; init; }

	protected List<StoryModel> TopStoryCollection { get; } = [];
	protected bool IsListRefreshing { get; set; }
	protected string? RefreshErrorMessage { get; set; }
	protected CancellationToken DisposeToken => _disposeCancellationTokenSource.Token;

	protected abstract string PracticesName { get; }

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected static void InsertIntoSortedList<T>(List<T> collection, Comparison<T> comparison, T modelToInsert)
	{
		if (collection.Count is 0)
		{
			collection.Add(modelToInsert);
			return;
		}

		var index = 0;
		foreach (var model in collection)
		{
			if (comparison(model, modelToInsert) >= 0)
			{
				collection.Insert(index, modelToInsert);
				return;
			}

			index++;
		}

		collection.Insert(index, modelToInsert);
	}

	protected abstract Task RefreshAsync();

	protected bool IsDataRecent(TimeSpan timeSpan) => TopStoryCollection.Any()
		&& (DateTimeOffset.UtcNow - TopStoryCollection.Max(x => x.CreatedAt_DateTimeOffset)) < timeSpan;

	protected virtual void Dispose(bool disposing)
	{
		if (!disposing)
		{
			return;
		}

		_disposeCancellationTokenSource.Cancel();
		_disposeCancellationTokenSource.Dispose();
	}
}