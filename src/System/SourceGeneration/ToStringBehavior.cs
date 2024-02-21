namespace System.SourceGeneration;

/// <summary>
/// Defines a behavior by source generator on generating <see cref="object.GetHashCode"/> overridden methods.
/// </summary>
/// <seealso cref="object.GetHashCode"/>
public enum ToStringBehavior
{
	/// <summary>
	/// Indicates the source generator will automatically determine which expression to be output.
	/// </summary>
	Intelligent,

	/// <summary>
	/// Indicates the source generator will generate an expression
	/// to call overload <see cref="ISimpleFormattable.ToString(string?)"/> method.
	/// </summary>
	/// <seealso cref="ISimpleFormattable.ToString(string?)"/>
	CallOverload,

	/// <summary>
	/// Indicates the source generator will generate an expression
	/// to output a string whose value is directly from a property or field result.
	/// </summary>
	Specified,

	/// <summary>
	/// Indicates the source generator will generate an expression
	/// like a <see langword="record"/> or <see langword="record struct"/> default output.
	/// </summary>
	RecordLike,

	/// <summary>
	/// Indicates throws <see cref="NotSupportedException"/>.
	/// </summary>
	ThrowNotSupportedException,

	/// <summary>
	/// Indicates the method will be made <see langword="abstract"/>.
	/// </summary>
	MakeAbstract
}
