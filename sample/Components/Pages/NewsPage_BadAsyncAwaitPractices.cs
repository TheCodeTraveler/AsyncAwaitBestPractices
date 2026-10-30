using System.Diagnostics;
using System.Runtime.CompilerServices;
using AsyncAwaitBestPractices;
using Microsoft.AspNetCore.Components;

namespace HackerNews.Components.Pages;

[Route("/")]
public sealed class NewsPage_BadAsyncAwaitPractices : NewsPage
{
	protected override string PracticesName => "Bad";

	protected override void OnInitialized()
	{
		IsListRefreshing = true;

		// ToDo Refactor
		RefreshAsync(DisposeToken);
	}

	protected override Task RefreshAsync() => RefreshAsync(DisposeToken);

	async Task RefreshAsync(CancellationToken token)
	{
		IsListRefreshing = true;
		RefreshErrorMessage = null;

		// ToDo Refactor
		var minimumRefreshTimeTask = Task.Delay(TimeSpan.FromSeconds(2));

		try
		{
			// ToDo Refactor
			var topStoriesList = await GetTopStories(token, StoriesConstants.NumberOfStories);

			await InvokeAsync(() =>
			{
				TopStoryCollection.Clear();

				foreach (var story in topStoriesList)
				{
					if (!TopStoryCollection.Any(x => x.Title.Equals(story.Title, StringComparison.Ordinal)))
					{
						InsertIntoSortedList(TopStoryCollection, (a, b) => b.Score.CompareTo(a.Score), story);
					}
				}
			});
		}
		catch (Exception e)
		{
			Logger.LogError(e, "Failed to refresh Hacker News top stories.");
			await InvokeAsync(() => RefreshErrorMessage = "Unable to refresh top stories. Check your connection and try again.");
		}
		finally
		{
			// ToDo Refactor
			minimumRefreshTimeTask.Wait();

			await InvokeAsync(() =>
			{
				IsListRefreshing = false;
				StateHasChanged();
			});
		}
	}

	// ToDo Refactor
	async Task<IReadOnlyList<StoryModel>> GetTopStories(CancellationToken token, int storyCount = int.MaxValue)
	{
		List<StoryModel> topStoryList = [];

		var topStoryIds = await GetTopStoryIDs(token).ConfigureAwait(false);

		foreach (var topStoryId in topStoryIds)
		{
			var story = await GetStory(topStoryId, token).ConfigureAwait(false);
			topStoryList.Add(story);

			if (topStoryList.Count >= storyCount)
			{
				break;
			}
		}

		return topStoryList.OrderByDescending(x => x.Score).ToList();
	}

	// ToDo Refactor
	async Task<StoryModel> GetStory(long storyId, CancellationToken token)
	{
		return await HackerNewsApiService.GetStory(storyId, token);
	}

	// ToDo Refactor
	async Task<IReadOnlyList<long>> GetTopStoryIDs(CancellationToken token)
	{
		if (IsDataRecent(TimeSpan.FromHours(1)))
		{
			return TopStoryCollection.Select(x => x.Id).ToList();
		}

		try
		{
			return await HackerNewsApiService.GetTopStoryIDs(token);
		}
		catch (Exception e)
		{
			Trace.WriteLine(e.Message);
			throw;
		}
	}
}