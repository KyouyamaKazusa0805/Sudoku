namespace Sudoku.Concepts;

/// <summary>
/// Defines an inference.
/// </summary>
public enum Inference : byte
{
	/// <summary>
	/// Indicates the inference is strong inference.
	/// </summary>
	Strong,

	/// <summary>
	/// Indicates the inference is weak inference.
	/// </summary>
	Weak,

	/// <summary>
	/// Indicates the inference is strong inference that is generalized.
	/// </summary>
	StrongGeneralized,

	/// <summary>
	/// Indicates the inference is weak inference that is generalized.
	/// </summary>
	WeakGeneralized,

	/// <summary>
	/// Indicates the inference is conjugate pair.
	/// </summary>
	ConjuagtePair
}
