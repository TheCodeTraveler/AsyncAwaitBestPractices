using System;
using System.Threading.Tasks;
using AsyncAwaitBestPractices.MVVM;
using NUnit.Framework;

namespace AsyncAwaitBestPractices.UnitTests;

class Tests_BaseCommand : BaseTest
{
	[Test]
	public void BaseCommand_CanExecute_DefaultCanExecuteReturnsTrue()
	{
		//Arrange
		var command = new TestAsyncCommand(NoParameterTask);

		//Act & Assert
		Assert.That(command.CanExecute(null), Is.True);
	}

	[Test]
	public void BaseCommand_CanExecute_CustomCanExecute()
	{
		//Arrange
		var command = new TestAsyncCommand(NoParameterTask, CanExecuteFalse);

		//Act & Assert
		Assert.That(command.CanExecute(null), Is.False);
	}

	[Test]
	public void BaseCommand_RaiseCanExecuteChanged_EventFired()
	{
		//Arrange
		var command = new TestAsyncCommand(NoParameterTask);
		bool eventFired = false;
		command.CanExecuteChanged += (sender, args) => eventFired = true;

		//Act
		command.RaiseCanExecuteChanged();

		//Assert
		Assert.That(eventFired, Is.True);
	}

	[Test]
	public void BaseCommand_CanExecuteChanged_WeakEventManager()
	{
		//Arrange
		var command = new TestAsyncCommand(NoParameterTask);
		bool eventFired = false;
		EventHandler handler = (sender, args) => eventFired = true;

		//Act
		command.CanExecuteChanged += handler;
		command.RaiseCanExecuteChanged();

		//Assert
		Assert.That(eventFired, Is.True);

		//Act
		eventFired = false;
		command.CanExecuteChanged -= handler;
		command.RaiseCanExecuteChanged();

		//Assert
		Assert.That(eventFired, Is.False);
	}

	class TestAsyncCommand(Func<Task> execute, Func<object?, bool>? canExecute = null) : BaseCommand<object?>(canExecute)
	{
		readonly Func<Task> _execute = execute;

		public async void Execute(object? parameter) => await _execute();
	}
}
