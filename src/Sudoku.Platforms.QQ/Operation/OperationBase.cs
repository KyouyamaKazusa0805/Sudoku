namespace Sudoku.Platforms.QQ.Operation;

/// <summary>
/// Defines a base type of the operation.
/// </summary>
public abstract record OperationBase
{
	/// <summary>
	/// Try to execute the task.
	/// </summary>
	/// <returns>The task.</returns>
	public abstract Task ExecuteAsync();
}
