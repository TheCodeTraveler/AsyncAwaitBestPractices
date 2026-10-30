namespace HackerNews;

public class HackerNewsAPIService(IHackerNewsAPI hackerNewsClient)
{
	readonly IHackerNewsAPI _hackerNewsClient = hackerNewsClient;

	public Task<StoryModel> GetStory(long storyId, CancellationToken token) => _hackerNewsClient.GetStory(storyId, token);
	public Task<IReadOnlyList<long>> GetTopStoryIDs(CancellationToken token) => _hackerNewsClient.GetTopStoryIDs(token);
}