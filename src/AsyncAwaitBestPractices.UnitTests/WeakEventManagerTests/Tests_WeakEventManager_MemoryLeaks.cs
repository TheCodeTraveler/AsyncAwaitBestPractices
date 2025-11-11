using System;
using System.Runtime.CompilerServices;
using NUnit.Framework;

namespace AsyncAwaitBestPractices.UnitTests;

class Tests_WeakEventManager_MemoryLeaks : BaseTest
{
	[Test]
	public void WeakEventManager_GarbageCollection_WeakReferencesCleanedUp()
	{
		//Arrange
		var weakEventManager = new WeakEventManager();
		CreateAndSubscribeTarget(weakEventManager);

		//Act
		GC.Collect();
		GC.WaitForPendingFinalizers();
		GC.Collect();

		//Assert - Should not throw even though target is collected
		Assert.DoesNotThrow(() => weakEventManager.RaiseEvent(this, EventArgs.Empty, "TestEvent"));
	}

	[Test]
	public void WeakEventManagerT_GarbageCollection_WeakReferencesCleanedUp()
	{
		//Arrange
		var weakEventManager = new WeakEventManager<string>();
		CreateAndSubscribeTargetT(weakEventManager);

		//Act
		GC.Collect();
		GC.WaitForPendingFinalizers();
		GC.Collect();

		//Assert - Should not throw even though target is collected
		Assert.DoesNotThrow(() => weakEventManager.RaiseEvent(this, "test", "TestEvent"));
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	void CreateAndSubscribeTarget(WeakEventManager weakEventManager)
	{
		var target = new TestEventTarget();
		EventHandler handler = target.HandleEvent;
		weakEventManager.AddEventHandler(handler);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	void CreateAndSubscribeTargetT(WeakEventManager<string> weakEventManager)
	{
		var target = new TestEventTarget();
		EventHandler<string> handler = target.HandleEventT;
		weakEventManager.AddEventHandler(handler);
	}

	class TestEventTarget
	{
		public void HandleEvent(object? sender, EventArgs e) { }
		public void HandleEventT(object? sender, string e) { }
	}
}
