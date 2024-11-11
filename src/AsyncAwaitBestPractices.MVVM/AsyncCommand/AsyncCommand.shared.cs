﻿using System;
using System.Threading.Tasks;

namespace AsyncAwaitBestPractices.MVVM;

/// <summary>
/// An implementation of IAsyncCommand. Allows Commands to safely be used asynchronously with Task.
/// </summary>
/// <remarks>
/// Initializes a new instance AsyncCommand
/// </remarks>
/// <param name="execute">The Function executed when Execute or ExecuteAsync is called. This does not check canExecute before executing and will execute even if canExecute is false</param>
/// <param name="canExecute">The Function that verifies whether AsyncCommand should execute.</param>
/// <param name="onException">If an exception is thrown in the Task, <c>onException</c> will execute. If onException is null, the exception will be re-thrown</param>
/// <param name="continueOnCapturedContext">If set to <c>true</c> continue on captured context; this will ensure that the Synchronization Context returns to the calling thread. If set to <c>false</c> continue on a different context; this will allow the Synchronization Context to continue on a different thread</param>
public class AsyncCommand<TExecute, TCanExecute>(Func<TExecute?, Task> execute,
                    Func<TCanExecute?, bool>? canExecute = null,
                    Action<Exception>? onException = null,
                    bool continueOnCapturedContext = false) : BaseAsyncCommand<TExecute, TCanExecute>(execute, canExecute, onException, continueOnCapturedContext), IAsyncCommand<TExecute, TCanExecute>
{
    /// <summary>
    /// Executes the Command as a Task
    /// </summary>
    /// <returns>The executed Task</returns>
    public new Task ExecuteAsync(TExecute parameter) => base.ExecuteAsync(parameter);
}

/// <summary>
/// An implementation of IAsyncCommand. Allows Commands to safely be used asynchronously with Task.
/// </summary>
/// <remarks>
/// Initializes a new instance AsyncCommand
/// </remarks>
/// <param name="execute">The Function executed when Execute or ExecuteAsync is called. This does not check canExecute before executing and will execute even if canExecute is false</param>
/// <param name="canExecute">The Function that verifies whether AsyncCommand should execute.</param>
/// <param name="onException">If an exception is thrown in the Task, <c>onException</c> will execute. If onException is null, the exception will be re-thrown</param>
/// <param name="continueOnCapturedContext">If set to <c>true</c> continue on captured context; this will ensure that the Synchronization Context returns to the calling thread. If set to <c>false</c> continue on a different context; this will allow the Synchronization Context to continue on a different thread</param>
public class AsyncCommand<T>(Func<T?, Task> execute,
                    Func<object?, bool>? canExecute = null,
                    Action<Exception>? onException = null,
                    bool continueOnCapturedContext = false) : BaseAsyncCommand<T, object?>(execute, canExecute, onException, continueOnCapturedContext), IAsyncCommand<T>
{
    /// <summary>
    /// Executes the Command as a Task
    /// </summary>
    /// <returns>The executed Task</returns>
    public new Task ExecuteAsync(T parameter) => base.ExecuteAsync(parameter);
}

/// <summary>
/// An implementation of IAsyncCommand. Allows Commands to safely be used asynchronously with Task.
/// </summary>
public class AsyncCommand : BaseAsyncCommand<object?, object?>, IAsyncCommand
{
	/// <summary>
	/// Initializes a new instance of AsyncCommand
	/// </summary>
	/// <param name="execute">The Function executed when Execute or ExecuteAsync is called. This does not check canExecute before executing and will execute even if canExecute is false</param>
	/// <param name="canExecute">The Function that verifies whether AsyncCommand should execute.</param>
	/// <param name="onException">If an exception is thrown in the Task, <c>onException</c> will execute. If onException is null, the exception will be re-thrown</param>
	/// <param name="continueOnCapturedContext">If set to <c>true</c> continue on captured context; this will ensure that the Synchronization Context returns to the calling thread. If set to <c>false</c> continue on a different context; this will allow the Synchronization Context to continue on a different thread</param>
	public AsyncCommand(Func<Task> execute,
						Func<object?, bool>? canExecute = null,
						Action<Exception>? onException = null,
						bool continueOnCapturedContext = false)
		: base(ConvertExecute(execute), canExecute, onException, continueOnCapturedContext)
	{
	}

	/// <summary>
	/// Executes the Command as a Task
	/// </summary>
	/// <returns>The executed Task</returns>
	public Task ExecuteAsync() => ExecuteAsync(null);

	static Func<object?, Task>? ConvertExecute(Func<Task>? execute)
	{
		if (execute is null)
			return null;

		return _ => execute();
	}
}