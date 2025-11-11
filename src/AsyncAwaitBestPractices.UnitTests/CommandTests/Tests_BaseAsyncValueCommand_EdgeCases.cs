using System;
using System.Threading.Tasks;
using AsyncAwaitBestPractices.MVVM;
using NUnit.Framework;

namespace AsyncAwaitBestPractices.UnitTests;

class Tests_BaseAsyncValueCommand_EdgeCases : BaseTest
{
	[Test]
	public void BaseAsyncValueCommand_Constructor_NullExecute_ThrowsArgumentNullException()
	{
		//Act & Assert
		Assert.Throws<ArgumentNullException>(() => new TestAsyncValueCommand(null!));
	}

	[Test]
	public void BaseAsyncValueCommand_ICommand_CanExecute_InvalidParameterType_ThrowsInvalidCommandParameterException()
	{
		//Arrange
		var command = new TestAsyncValueCommand(IntParameterValueTask);
		System.Windows.Input.ICommand iCommand = command;

		//Act & Assert
		Assert.Throws<InvalidCommandParameterException>(() => iCommand.CanExecute("invalid"));
	}

	[Test]
	public void BaseAsyncValueCommand_ICommand_Execute_InvalidParameterType_ThrowsInvalidCommandParameterException()
	{
		//Arrange
		var command = new TestAsyncValueCommand(IntParameterValueTask);
		System.Windows.Input.ICommand iCommand = command;

		//Act & Assert
		Assert.Throws<InvalidCommandParameterException>(() => iCommand.Execute("invalid"));
	}

	class TestAsyncValueCommand : BaseAsyncValueCommand<int, int>
	{
		public TestAsyncValueCommand(Func<int, ValueTask>? execute) : base(execute, null, null, false)
		{
		}
	}
}
