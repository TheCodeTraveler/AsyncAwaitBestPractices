using System;
using System.Threading.Tasks;
using AsyncAwaitBestPractices.MVVM;
using NUnit.Framework;

namespace AsyncAwaitBestPractices.UnitTests;

class Tests_BaseAsyncCommand_EdgeCases : BaseTest
{
	[Test]
	public void BaseAsyncCommand_Constructor_NullExecute_ThrowsArgumentNullException()
	{
		//Act & Assert
		Assert.Throws<ArgumentNullException>(() => new TestAsyncCommand(null!));
	}

	[Test]
	public void BaseAsyncCommand_ICommand_CanExecute_InvalidParameterType_ThrowsInvalidCommandParameterException()
	{
		//Arrange
		var command = new TestAsyncCommand(NoParameterTask);
		System.Windows.Input.ICommand iCommand = command;

		//Act & Assert
		Assert.Throws<InvalidCommandParameterException>(() => iCommand.CanExecute("invalid"));
	}

	[Test]
	public void BaseAsyncCommand_ICommand_Execute_InvalidParameterType_ThrowsInvalidCommandParameterException()
	{
		//Arrange
		var command = new TestAsyncCommand(NoParameterTask);
		System.Windows.Input.ICommand iCommand = command;

		//Act & Assert
		Assert.Throws<InvalidCommandParameterException>(() => iCommand.Execute("invalid"));
	}

	class TestAsyncCommand : BaseAsyncCommand<object?, object?>
	{
		public TestAsyncCommand(Func<object?, Task>? execute) : base(execute, null, null, false)
		{
		}
	}
}
