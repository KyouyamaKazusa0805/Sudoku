namespace Sudoku.Concepts;

/// <summary>
/// Defines an inference.
/// </summary>
public enum Inference : byte
{
	/// <summary>
	/// Indicates the inference is strong inference.
	/// </summary>
	[EnumFieldName(" == ")]
	Strong,

	/// <summary>
	/// Indicates the inference is weak inference.
	/// </summary>
	[EnumFieldName(" -- ")]
	Weak,

	/// <summary>
	/// Indicates the inference is strong inference that is generalized.
	/// </summary>
	[EnumFieldName(" =~ ")]
	StrongGeneralized,

	/// <summary>
	/// Indicates the inference is weak inference that is generalized.
	/// </summary>
	[EnumFieldName(" -~ ")]
	WeakGeneralized,

	/// <summary>
	/// Indicates the inference is conjugate pair.
	/// </summary>
	[EnumFieldName(" == ")]
	ConjuagtePair,

	/// <summary>
	/// Indicates the inference is the default case that doesn't belong to above.
	/// </summary>
	[EnumFieldName(" -- ")]
	Default
}
