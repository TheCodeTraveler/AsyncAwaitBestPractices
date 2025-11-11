using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AsyncAwaitBestPractices.UnitTests;

class Tests_SafeFireAndForget_Initialize : BaseTest
{
	[TearDown]
	public void AfterEachTest()
	{
		SafeFireAndForgetExtensions.Initialize(false);
		SafeFireAndForgetExtensions.RemoveDefaultExceptionHandling();
	}

	[Test]
	public void SafeFireAndForget_Initialize_ShouldAlwaysRethrowException_True()
	{
		//Act & Assert
		Assert.DoesNotThrow(() => SafeFireAndForgetExtensions.Initialize(shouldAlwaysRethrowException: true));
	}

	[Test]
	public void SafeFireAndForget_Initialize_ShouldAlwaysRethrowException_False()
	{
		//Act & Assert
		Assert.DoesNotThrow(() => SafeFireAndForgetExtensions.Initialize(shouldAlwaysRethrowException: false));
	}

	[Test]
	public void SafeFireAndForget_SetDefaultExceptionHandling_ValidHandler()
	{
		//Arrange
		Exception? caughtException = null;

		//Act & Assert
		Assert.DoesNotThrow(() => SafeFireAndForgetExtensions.SetDefaultExceptionHandling(ex => caughtException = ex));
	}

	[Test]
	public void SafeFireAndForget_SetDefaultExceptionHandling_NullHandler()
	{
		//Act & Assert
		Assert.DoesNotThrow(() => SafeFireAndForgetExtensions.SetDefaultExceptionHandling(null!));
	}

	[Test]
	public void SafeFireAndForget_RemoveDefaultExceptionHandling()
	{
		//Arrange
		SafeFireAndForgetExtensions.SetDefaultExceptionHandling(ex => { });

		//Act & Assert
		Assert.DoesNotThrow(() => SafeFireAndForgetExtensions.RemoveDefaultExceptionHandling());
	}

	[Test]
	public async Task SafeFireAndForget_DefaultExceptionHandling_HandlesException()
	{
		//Arrange
		Exception? defaultException = null;
		SafeFireAndForgetExtensions.SetDefaultExceptionHandling(ex => defaultException = ex);

		//Act
		ThrowExceptionTask().SafeFireAndForget();
		await Task.Delay(500);

		//Assert
		Assert.That(defaultException, Is.Not.Null);
	}

	static async Task ThrowExceptionTask()
	{
		await Task.Delay(100);
		throw new InvalidOperationException("Test exception");
	}
}
