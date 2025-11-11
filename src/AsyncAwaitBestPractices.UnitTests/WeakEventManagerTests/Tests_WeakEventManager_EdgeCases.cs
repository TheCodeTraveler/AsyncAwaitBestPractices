using System;
using System.Reflection;
using NUnit.Framework;

namespace AsyncAwaitBestPractices.UnitTests;

class Tests_WeakEventManager_EdgeCases : BaseTest
{
	[Test]
	public void WeakEventManager_RaiseEvent_InvalidEventName()
	{
		//Arrange
		var weakEventManager = new WeakEventManager();

		//Act & Assert
		Assert.DoesNotThrow(() => weakEventManager.RaiseEvent(this, EventArgs.Empty, "NonExistentEvent"));
	}

	[Test]
	public void WeakEventManager_RaiseEvent_NullEventName()
	{
		//Arrange
		var weakEventManager = new WeakEventManager();

		//Act & Assert
		Assert.Throws<ArgumentNullException>(() => weakEventManager.RaiseEvent(this, EventArgs.Empty, null!));
	}

	[Test]
	public void WeakEventManager_RaiseEvent_EmptyEventName()
	{
		//Arrange
		var weakEventManager = new WeakEventManager();

		//Act & Assert
		Assert.DoesNotThrow(() => weakEventManager.RaiseEvent(this, EventArgs.Empty, string.Empty));
	}

	[Test]
	public void WeakEventManager_AddEventHandler_NullHandler()
	{
		//Arrange
		var weakEventManager = new WeakEventManager();

		//Act & Assert
		Assert.Throws<ArgumentNullException>(() => weakEventManager.AddEventHandler(null!));
	}

	[Test]
	public void WeakEventManager_RemoveEventHandler_NullHandler()
	{
		//Arrange
		var weakEventManager = new WeakEventManager();

		//Act & Assert
		Assert.Throws<ArgumentNullException>(() => weakEventManager.RemoveEventHandler(null!));
	}

	[Test]
	public void WeakEventManager_RemoveEventHandler_NonExistentHandler()
	{
		//Arrange
		var weakEventManager = new WeakEventManager();
		EventHandler handler = (sender, args) => { };

		//Act & Assert
		Assert.DoesNotThrow(() => weakEventManager.RemoveEventHandler(handler));
	}

	[Test]
	public void WeakEventManagerT_RaiseEvent_NullEventName()
	{
		//Arrange
		var weakEventManager = new WeakEventManager<string>();

		//Act & Assert
		Assert.Throws<ArgumentNullException>(() => weakEventManager.RaiseEvent(this, "test", null!));
	}

	[Test]
	public void WeakEventManagerT_AddEventHandler_NullHandler()
	{
		//Arrange
		var weakEventManager = new WeakEventManager<string>();

		//Act & Assert
		Assert.Throws<ArgumentNullException>(() => weakEventManager.AddEventHandler((EventHandler<string>)null!));
	}

	[Test]
	public void WeakEventManagerT_RemoveEventHandler_NullHandler()
	{
		//Arrange
		var weakEventManager = new WeakEventManager<string>();

		//Act & Assert
		Assert.Throws<ArgumentNullException>(() => weakEventManager.RemoveEventHandler((EventHandler<string>)null!));
	}

	[Test]
	public void WeakEventManager_HandleEvent_InvalidParameterCount()
	{
		//Arrange
		TestEvent += HandleTestEventWithWrongParameters;
		bool didEventFire = false;

		//Act & Assert
		Assert.Throws<InvalidHandleEventException>(() => 
			TestWeakEventManager.RaiseEvent(this, EventArgs.Empty, nameof(TestEvent)));

		void HandleTestEventWithWrongParameters(object? sender, EventArgs e, string extraParameter)
		{
			didEventFire = true;
		}
	}

	[Test]
	public void WeakEventManager_GarbageCollection_WeakReferencesCleanedUp()
	{
		//Arrange
		var weakEventManager = new WeakEventManager();
		var target = new TestEventTarget();
		EventHandler handler = target.HandleEvent;
		
		weakEventManager.AddEventHandler(handler);

		//Act
		target = null;
		GC.Collect();
		GC.WaitForPendingFinalizers();
		GC.Collect();

		//Assert - Should not throw even though target is collected
		Assert.DoesNotThrow(() => weakEventManager.RaiseEvent(this, EventArgs.Empty, "TestEvent"));
	}

	class TestEventTarget
	{
		public void HandleEvent(object? sender, EventArgs e)
		{
			// Test event handler
		}
	}
}
