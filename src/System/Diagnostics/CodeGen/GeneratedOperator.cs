namespace System.Diagnostics.CodeGen;

/// <summary>
/// Indicates the generated operator.
/// </summary>
[Flags]
public enum GeneratedOperator
{
	/// <summary>
	/// Indicates the generated operator is none.
	/// </summary>
	None = 0,

	/// <summary>
	/// Indicates the equality operator <c>==</c>.
	/// </summary>
	Equality = 1,

	/// <summary>
	/// Indicates the inequality operator <c>!=</c>.
	/// </summary>
	Inequality = 2,

	/// <summary>
	/// Indicates the equality operators (containing <see cref="Equality"/> and <see cref="Inequality"/>).
	/// </summary>
	/// <seealso cref="Equality"/>
	/// <seealso cref="Inequality"/>
	EqualityOperators = Equality | Inequality
}
