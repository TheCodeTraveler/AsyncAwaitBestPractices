using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AsyncAwaitBestPractices.UnitTests;

class Tests_SafeFireAndForget_EdgeCases : BaseTest
{
	[SetUp]
	public void BeforeEachTest()
	{
		SafeFireAndForgetExtensions.Initialize(false);
		SafeFireAndForgetExtensions.RemoveDefaultExceptionHandling();
	}

	[TearDown]
	public void AfterEachTest()
	{
		SafeFireAndForgetExtensions.Initialize(false);
		SafeFireAndForgetExtensions.RemoveDefaultExceptionHandling();
	}

	[Test]
	public void SafeFireAndForget_Initialize_ShouldAlwaysRethrowException()
	{
		//Arrange & Act
		SafeFireAndForgetExtensions.Initialize(shouldAlwaysRethrowException: true);

		//Assert - No exception should be thrown during initialization
		Assert.Pass();
	}

	[Test]
	public async Task SafeFireAndForget_CompletedTask_NoException()
	{
		//Arrange
		var completedTask = Task.CompletedTask;
		Exception? caughtException = null;

		//Act
		completedTask.SafeFireAndForget(ex => caughtException = ex);
		await Task.Delay(100); // Small delay to ensure any potential exception handling

		//Assert
		Assert.That(caughtException, Is.Null);
	}

	[Test]
	public async Task SafeFireAndForget_CompletedValueTask_NoException()
	{
		//Arrange
		var completedValueTask = ValueTask.CompletedTask;
		Exception? caughtException = null;

		//Act
		completedValueTask.SafeFireAndForget(ex => caughtException = ex);
		await Task.Delay(100); // Small delay to ensure any potential exception handling

		//Assert
		Assert.That(caughtException, Is.Null);
	}

	[Test]
	public async Task SafeFireAndForget_TaskWithResult_CompletedSuccessfully()
	{
		//Arrange
		var taskWithResult = Task.FromResult(42);
		Exception? caughtException = null;

		//Act
		taskWithResult.SafeFireAndForget(ex => caughtException = ex);
		await Task.Delay(100); // Small delay to ensure any potential exception handling

		//Assert
		Assert.That(caughtException, Is.Null);
	}

	[Test]
	public async Task SafeFireAndForget_ValueTaskWithResult_CompletedSuccessfully()
	{
		//Arrange
		var valueTaskWithResult = ValueTask.FromResult(42);
		Exception? caughtException = null;

		//Act
		valueTaskWithResult.SafeFireAndForget(ex => caughtException = ex);
		await Task.Delay(100); // Small delay to ensure any potential exception handling

		//Assert
		Assert.That(caughtException, Is.Null);
	}

	[Test]
	public void SafeFireAndForget_RemoveDefaultExceptionHandling_NoException()
	{
		//Arrange
		SafeFireAndForgetExtensions.SetDefaultExceptionHandling(ex => { });

		//Act & Assert
		Assert.DoesNotThrow(() => SafeFireAndForgetExtensions.RemoveDefaultExceptionHandling());
	}

	[Test]
	public void SafeFireAndForget_SetDefaultExceptionHandling_NullHandler()
	{
		//Act & Assert
		Assert.DoesNotThrow(() => SafeFireAndForgetExtensions.SetDefaultExceptionHandling(null!));
	}
}
