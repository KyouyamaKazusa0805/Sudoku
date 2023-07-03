namespace System.SourceGeneration;

/// <summary>
/// Defines a behavior by source generator on generating <see langword="operator"/> <c>==</c> and <see langword="operator"/> <c>!=</c>
/// overloading operators.
/// </summary>
public enum EqualityOperatorsBehavior
{
	/// <summary>
	/// Indicates the source generator will choose a suitable way to implement equality operators.
	/// </summary>
	Intelligent,

	/// <summary>
	/// Indicates the operators will be made to be <see langword="virtual"/>.
	/// This option can only be used for interfaces implementing operators.
	/// </summary>
	MakeVirtual,

	/// <summary>
	/// Indicates the operators will be made to be <see langword="abstract"/>.
	/// This option can only be used for interfaces implementing operators.
	/// </summary>
	MakeAbstract
}
