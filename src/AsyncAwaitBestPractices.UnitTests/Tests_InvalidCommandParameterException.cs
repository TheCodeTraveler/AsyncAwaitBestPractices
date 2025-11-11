using System;
using AsyncAwaitBestPractices.MVVM;
using NUnit.Framework;

namespace AsyncAwaitBestPractices.UnitTests;

class Tests_InvalidCommandParameterException: BaseTest
{
	[Test]
	public void InvalidCommandParameterException_Constructor_ExpectedAndActualType()
	{
		//Arrange
		var expectedType = typeof(string);
		var actualType = typeof(int);

		//Act
		var exception = new InvalidCommandParameterException(expectedType, actualType);

		//Assert
		using (Assert.EnterMultipleScope())
		{
			Assert.That(exception.Message, Is.EqualTo($"Invalid type for parameter. Expected Type {expectedType}, but received Type {actualType}"));
			Assert.That(exception.InnerException, Is.Null);
		}
	}

	[Test]
	public void InvalidCommandParameterException_Constructor_ExpectedAndActualTypeWithInnerException()
	{
		//Arrange
		var expectedType = typeof(string);
		var actualType = typeof(int);
		var innerException = new ArgumentException("Inner exception");

		//Act
		var exception = new InvalidCommandParameterException(expectedType, actualType, innerException);

		//Assert
		using (Assert.EnterMultipleScope())
		{
			Assert.That(exception.Message, Is.EqualTo($"Invalid type for parameter. Expected Type {expectedType}, but received Type {actualType}"));
			Assert.That(exception.InnerException, Is.EqualTo(innerException));
		}
	}

	[Test]
	public void InvalidCommandParameterException_Constructor_ExpectedTypeOnly()
	{
		//Arrange
		var expectedType = typeof(string);

		//Act
		var exception = new InvalidCommandParameterException(expectedType);

		//Assert
		using (Assert.EnterMultipleScope())
		{
			Assert.That(exception.Message, Is.EqualTo($"Invalid type for parameter. Expected Type {expectedType}"));
			Assert.That(exception.InnerException, Is.Null);
		}
	}

	[Test]
	public void InvalidCommandParameterException_Constructor_ExpectedTypeWithInnerException()
	{
		//Arrange
		var expectedType = typeof(string);
		var innerException = new ArgumentException("Inner exception");

		//Act
		var exception = new InvalidCommandParameterException(expectedType, innerException);

		//Assert
		using (Assert.EnterMultipleScope())
		{
			Assert.That(exception.Message, Is.EqualTo($"Invalid type for parameter. Expected Type {expectedType}"));
			Assert.That(exception.InnerException, Is.EqualTo(innerException));
		}
	}

	[Test]
	public void InvalidCommandParameterException_InheritsFromException()
	{
		//Arrange
		var expectedType = typeof(string);

		//Act
		var exception = new InvalidCommandParameterException(expectedType);

		//Assert
		Assert.That(exception, Is.InstanceOf<Exception>());
	}
}
