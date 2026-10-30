using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components;

namespace HackerNews.Components.Pages;

[Route("/good")]
public sealed class NewsPage_GoodAsyncAwaitPractices : NewsPage
{
	protected override string PracticesName => "Good";

	protected override async Task OnInitializedAsync()
	{
		IsListRefreshing = true;
		await RefreshAsync(DisposeToken);
	}

	protected override Task RefreshAsync() => RefreshAsync(DisposeToken);

	async Task RefreshAsync(CancellationToken token)
	{
		IsListRefreshing = true;
		RefreshErrorMessage = null;

		var minimumRefreshTimeTask = Task.Delay(TimeSpan.FromSeconds(2), token);

		try
		{
			var topStoryIds = await GetTopStoryIDs(token).ConfigureAwait(false);

			await InvokeAsync(TopStoryCollection.Clear);

			await foreach (var story in GetTopStories(topStoryIds, StoriesConstants.NumberOfStories, token).ConfigureAwait(false))
			{
				await InvokeAsync(() =>
				{
					if (!TopStoryCollection.Any(x => x.Title.Equals(story.Title, StringComparison.Ordinal)))
					{
						InsertIntoSortedList(TopStoryCollection, (a, b) => b.Score.CompareTo(a.Score), story);
					}

					StateHasChanged();
				});
			}
		}
		catch (OperationCanceledException) when (token.IsCancellationRequested)
		{
			// The page closed before the refresh finished, so there is no one left to show an error to
		}
		catch (Exception e)
		{
			Logger.LogError(e, "Failed to refresh Hacker News top stories.");
			await InvokeAsync(() => RefreshErrorMessage = "Unable to refresh top stories. Check your connection and try again.");
		}
		finally
		{
			// Closing the page cancels this delay, and SuppressThrowing keeps that cancellation from throwing out of the finally block
			await minimumRefreshTimeTask.ConfigureAwait(ConfigureAwaitOptions.None | ConfigureAwaitOptions.SuppressThrowing);

			await InvokeAsync(() =>
			{
				IsListRefreshing = false;

				if (!token.IsCancellationRequested)
				{
					StateHasChanged();
				}
			});
		}
	}

	async IAsyncEnumerable<StoryModel> GetTopStories(IReadOnlyList<long> topStoryIds, int storyCount, [EnumeratorCancellation] CancellationToken token)
	{
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(storyCount);

		// ToList starts every request now, before the loop awaits the first one
		var getTopStoryTaskList = topStoryIds.Take(storyCount).Select(id => GetStory(id, token)).ToList();

		await foreach (var topStoryTask in getTopStoryTaskList.ToAsyncEnumerable().WithCancellation(token).ConfigureAwait(false))
		{
			yield return await topStoryTask.ConfigureAwait(false);
		}
	}

	// async and return await stay: an exception thrown by the call then lands in the returned Task instead of escaping from the call.
	// ConfigureAwait(false) removes the trip back to the page's context, which runtime async keeps.
	async Task<StoryModel> GetStory(long storyId, CancellationToken token)
	{
		return await HackerNewsApiService.GetStory(storyId, token).ConfigureAwait(false);
	}

	// async stays because of the try/catch: without await, the method would return before the call finished, and this catch would never see its exception.
	// ConfigureAwait(false): this method starts on the page's context, and a default await would queue its last step back there before RefreshAsync could go on.
	// ValueTask: the cached path returns without awaiting. Classic async allocates a Task for each of those calls; with runtime async on, ValueTask and Task cost the same here.
	async ValueTask<IReadOnlyList<long>> GetTopStoryIDs(CancellationToken token)
	{
		if (IsDataRecent(TimeSpan.FromHours(1)))
		{
			return TopStoryCollection.Select(x => x.Id).ToList();
		}

		try
		{
			return await HackerNewsApiService.GetTopStoryIDs(token).ConfigureAwait(false);
		}
		catch (Exception e)
		{
			Trace.WriteLine(e.Message);
			throw;
		}
	}
}