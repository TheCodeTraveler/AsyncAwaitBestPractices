using Refit;

namespace HackerNews;

public interface IHackerNewsAPI
{
	[Get("/topstories.json?print=pretty")]
	Task<IReadOnlyList<long>> GetTopStoryIDs(CancellationToken token);

	[Get("/item/{storyId}.json?print=pretty")]
	Task<StoryModel> GetStory(long storyId, CancellationToken token);
}