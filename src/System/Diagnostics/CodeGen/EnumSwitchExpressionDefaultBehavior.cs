namespace System.Diagnostics.CodeGen;

/// <summary>
/// Represents a default behavior of generated file for default cases in <see langword="switch"/> expressions.
/// </summary>
public enum EnumSwitchExpressionDefaultBehavior : byte
{
	/// <summary>
	/// Indicates the source generator will emit a <see langword="default"/>(<see cref="int"/>) value as default cases.
	/// </summary>
	ReturnIntegerValue,

	/// <summary>
	/// Indicates the source generator will emit <see langword="null"/> as default cases.
	/// </summary>
	ReturnNull,

	/// <summary>
	/// Indicates the source generator will throw <see cref="ArgumentOutOfRangeException"/> to report invalid values
	/// as default cases.
	/// </summary>
	Throw
}
