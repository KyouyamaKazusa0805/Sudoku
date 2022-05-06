namespace System.Diagnostics.CodeGen;

/// <summary>
/// Defines a case that the specified attribute is not found.
/// </summary>
public enum EnumSwitchExpressionDefaultBehavior : byte
{
	/// <summary>
	/// Indicates the result behavior is to return the string representation of the integer value
	/// corrsponding to the current enumeration field.
	/// </summary>
	ReturnByCorrespondingIntegerValue,

	/// <summary>
	/// Indicates the result behavior is to throw an <see cref="ArgumentOutOfRangeException"/> to report it.
	/// </summary>
	/// <seealso cref="ArgumentOutOfRangeException"/>
	Throw
}
