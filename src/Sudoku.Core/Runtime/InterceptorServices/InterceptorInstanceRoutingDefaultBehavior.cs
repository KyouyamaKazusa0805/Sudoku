namespace Sudoku.Runtime.InterceptorServices;

/// <summary>
/// Indicates the default behavior on routing for interceptor instance type checking.
/// </summary>
public enum InterceptorInstanceRoutingDefaultBehavior
{
	/// <summary>
	/// Indicates the default behavior is to return <see langword="default"/> or do nothing.
	/// </summary>
	DoNothingOrReturnDefault,

	/// <summary>
	/// Indicates the default behavior is to throw a <see cref="NotSupportedException"/>.
	/// </summary>
	ThrowNotSupportedException
}
