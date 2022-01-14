namespace Sudoku.Diagnostics.CodeAnalysis;

/// <summary>
/// Used for applying onto a setter to tell the compiler that the setter is only used for the invocation
/// by compiler rather than users.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class InitializationOnlyAttribute : Attribute
{
	/// <summary>
	/// Initializes an <see cref="InitializationOnlyAttribute"/> instance via a caller enumeration field.
	/// </summary>
	/// <param name="caller">The caller.</param>
	public InitializationOnlyAttribute(InitializationCaller caller) => Caller = caller;


	/// <summary>
	/// Indicates the caller.
	/// </summary>
	public InitializationCaller Caller { get; }
}
