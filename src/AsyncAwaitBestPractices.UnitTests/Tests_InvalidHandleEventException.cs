using System;
using System.Reflection;
using NUnit.Framework;

namespace AsyncAwaitBestPractices.UnitTests;

class Tests_InvalidHandleEventException : BaseTest
{
	[Test]
	public void InvalidHandleEventException_Constructor_ValidParameters()
	{
		//Arrange
		const string message = "Test exception message";
		var targetParameterCountException = new TargetParameterCountException("Parameter count mismatch");

		//Act
		var exception = new InvalidHandleEventException(message, targetParameterCountException);

		//Assert
		using (Assert.EnterMultipleScope())
		{
			Assert.That(exception.Message, Is.EqualTo(message));
			Assert.That(exception.InnerException, Is.EqualTo(targetParameterCountException));
			Assert.That(exception.InnerException, Is.TypeOf<TargetParameterCountException>());
		}
	}

	[Test]
	public void InvalidHandleEventException_Constructor_NullMessage()
	{
		//Arrange
		var targetParameterCountException = new TargetParameterCountException("Parameter count mismatch");

		//Act & Assert
		Assert.DoesNotThrow(() => new InvalidHandleEventException(null!, targetParameterCountException));
	}

	[Test]
	public void InvalidHandleEventException_Constructor_NullInnerException()
	{
		//Arrange
		const string message = "Test exception message";

		//Act & Assert
		Assert.DoesNotThrow(() => new InvalidHandleEventException(message, null!));
	}

	[Test]
	public void InvalidHandleEventException_InheritsFromException()
	{
		//Arrange
		const string message = "Test exception message";
		var targetParameterCountException = new TargetParameterCountException("Parameter count mismatch");

		//Act
		var exception = new InvalidHandleEventException(message, targetParameterCountException);

		//Assert
		Assert.That(exception, Is.InstanceOf<Exception>());
	}
}
