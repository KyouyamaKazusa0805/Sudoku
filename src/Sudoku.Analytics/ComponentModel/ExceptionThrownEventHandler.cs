using Sudoku.Analytics;

namespace Sudoku.ComponentModel;

/// <summary>
/// Represents a delegate type that executes a list of methods that will be triggered when an exception is thrown.
/// </summary>
/// <param name="sender">The object that triggers the execution.</param>
/// <param name="e">The event argument.</param>
public delegate void ExceptionThrownEventHandler<in TSelf, out TResult>(TSelf sender, ExceptionThrownEventArgs e)
	where TSelf : IAnalyzer<TSelf, TResult>
	where TResult : IAnalyzerResult<TSelf, TResult>;
