using System;
using System.Threading.Tasks;
using AsyncAwaitBestPractices.MVVM;
using NUnit.Framework;

namespace AsyncAwaitBestPractices.UnitTests;

class Tests_BaseCommand_IsNullable : BaseTest
{
	[Test]
	public void IsNullable_ReferenceType_ReturnsTrue()
	{
		//Arrange
		var command = new TestAsyncCommand(NoParameterTask);

		//Act
		var result = command.TestIsNullable<string>();

		//Assert
		Assert.That(result, Is.True);
	}

	[Test]
	public void IsNullable_ValueType_ReturnsFalse()
	{
		//Arrange
		var command = new TestAsyncCommand(NoParameterTask);

		//Act
		var result = command.TestIsNullable<int>();

		//Assert
		Assert.That(result, Is.False);
	}

	[Test]
	public void IsNullable_NullableValueType_ReturnsTrue()
	{
		//Arrange
		var command = new TestAsyncCommand(NoParameterTask);

		//Act
		var result = command.TestIsNullable<int?>();

		//Assert
		Assert.That(result, Is.True);
	}

	class TestAsyncCommand(Func<Task> execute) : BaseCommand<object?>(null)
	{
		readonly Func<Task> _execute = execute;

		public bool TestIsNullable<T>() => IsNullable<T>();

		public async void Execute(object? parameter) => await _execute();
	}
}
