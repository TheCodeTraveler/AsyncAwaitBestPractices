using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AsyncAwaitBestPractices.MVVM;
using NUnit.Framework;

namespace AsyncAwaitBestPractices.UnitTests;

// NuGet gives the same lib/net11.0 assembly to every net11.0 app, including Blazor WebAssembly and other apps that run on Mono,
// and Mono can't run a runtime-async method. With runtime async on for the packages, the compiler converts a new async Task or ValueTask method
// without a warning, so this test is what notices a shipped method compiled that way. A new package needs its own TestCase.
class Tests_RuntimeAsync_ShippedAssemblies : BaseTest
{
	// MethodImplAttributes.Async doesn't exist before .NET 10, and this file also compiles for net462, net8.0 and net9.0.
	const MethodImplAttributes _asyncMethodImplFlag = (MethodImplAttributes)0x2000;

	const BindingFlags _allDeclaredMembers = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

	[TestCase(typeof(SafeFireAndForgetExtensions))]
	[TestCase(typeof(AsyncCommand))]
	public void ShippedAssembly_HasNoRuntimeAsyncMethods(Type typeFromShippedAssembly)
	{
		//Arrange
		var assembly = typeFromShippedAssembly.Assembly;

		//Act
		var runtimeAsyncMethods = FindRuntimeAsyncMethods(assembly).ToList();

		//Assert
		Assert.That(runtimeAsyncMethods, Is.Empty, $"{assembly.GetName().Name} contains runtime-async methods, which Mono, and so Blazor WebAssembly, can't run. Add <Features>$(Features);runtime-async=off</Features> to that package's project file; Directory.Build.targets says why.");
	}

#if NET11_0_OR_GREATER && WINDOWS
	// Without this, the test above would also pass if the scan could never see the flag, or if the switch silently stopped working:
	// a misspelled value or a new switch syntax produces classic code without an error.
	[Test]
	public void Scan_FindsRuntimeAsyncMethods_InTheTestAssemblyItself()
	{
		//Arrange
		var testAssembly = typeof(Tests_RuntimeAsync_ShippedAssemblies).Assembly;

		//Act
		var runtimeAsyncMethods = FindRuntimeAsyncMethods(testAssembly).ToList();

		//Assert
		Assert.That(runtimeAsyncMethods, Is.Not.Empty);
	}
#endif

	static IEnumerable<string> FindRuntimeAsyncMethods(Assembly assembly)
	{
		foreach (var type in assembly.GetTypes())
		{
			foreach (var method in type.GetMethods(_allDeclaredMembers).Cast<MethodBase>().Concat(type.GetConstructors(_allDeclaredMembers)))
			{
				if ((method.GetMethodImplementationFlags() & _asyncMethodImplFlag) != 0)
				{
					yield return $"{type.FullName}.{method.Name}";
				}
			}
		}
	}
}