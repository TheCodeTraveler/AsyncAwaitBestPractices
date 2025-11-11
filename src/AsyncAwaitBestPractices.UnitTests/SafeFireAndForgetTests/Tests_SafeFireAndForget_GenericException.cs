using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AsyncAwaitBestPractices.UnitTests;

class Tests_SafeFireAndForget_GenericException : BaseTest
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
	public async Task SafeFireAndForget_Task_SpecificExceptionType_HandlesCorrectException()
	{
		//Arrange
		ArgumentException? caughtException = null;

		//Act
		ThrowArgumentExceptionTask().SafeFireAndForget<ArgumentException>(ex => caughtException = ex);
		await Task.Delay(1000);

		//Assert
		Assert.That(caughtException, Is.Not.Null);
		Assert.That(caughtException, Is.TypeOf<ArgumentException>());
	}

	[Test]
	public async Task SafeFireAndForget_ValueTask_SpecificExceptionType_HandlesCorrectException()
	{
		//Arrange
		ArgumentException? caughtException = null;

		//Act
		ThrowArgumentExceptionValueTask().SafeFireAndForget<ArgumentException>(ex => caughtException = ex);
		await Task.Delay(1000);

		//Assert
		Assert.That(caughtException, Is.Not.Null);
		Assert.That(caughtException, Is.TypeOf<ArgumentException>());
	}

	static async Task ThrowArgumentExceptionTask()
	{
		await Task.Delay(100);
		throw new ArgumentException("Test exception");
	}

	static async ValueTask ThrowArgumentExceptionValueTask()
	{
		await Task.Delay(100);
		throw new ArgumentException("Test exception");
	}
}
