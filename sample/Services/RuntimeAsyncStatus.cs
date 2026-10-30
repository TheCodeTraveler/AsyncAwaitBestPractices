using System.Reflection;

namespace HackerNews;

// The runtime-async switch fails silently: a value with the wrong case builds without a warning, and an incremental build keeps the old code when the value changes only on the command line (-p:Features).
// So the page reports what the compiler produced, read from one of this assembly's own async methods.
static class RuntimeAsyncStatus
{
	public static bool IsEnabled { get; } = typeof(RuntimeAsyncStatus).GetMethod(nameof(SampleMethodAsync), BindingFlags.NonPublic | BindingFlags.Static) is MethodInfo sampleMethod
		&& sampleMethod.MethodImplementationFlags.HasFlag(MethodImplAttributes.Async);

	// With runtime-async=on, the compiler marks async methods that return Task or ValueTask with MethodImplAttributes.Async (0x2000)
	static async Task SampleMethodAsync() => await Task.Yield();
}