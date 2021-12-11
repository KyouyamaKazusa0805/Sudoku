namespace Sudoku.Diagnostics.CodeAnalysis;

/// <summary>
/// To mark onto a parameter, to tell the compiler that the argument must be a discard
/// and can't be used when the return value is specified value.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
public sealed partial class DiscardWhenAttribute : Attribute
{
	/// <summary>
	/// Initializes a <see cref="DiscardWhenAttribute"/> instance.
	/// </summary>
	/// <param name="returnValue">The return value.</param>
	public DiscardWhenAttribute(bool returnValue) => ReturnValue = returnValue;


	/// <summary>
	/// Indicates the return value that makes the parameter a discard.
	/// </summary>
	public bool ReturnValue { get; }
}
