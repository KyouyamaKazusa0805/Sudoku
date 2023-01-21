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
	/// Indicates the greater-than operator <c><![CDATA[>]]></c>.
	/// </summary>
	GreaterThan = 4,

	/// <summary>
	/// Indicates the greater-than operator <c><![CDATA[>=]]></c>.
	/// </summary>
	GreaterThanOrEqual = 8,

	/// <summary>
	/// Indicates the greater-than operator <c><![CDATA[<]]></c>.
	/// </summary>
	LessThan = 16,

	/// <summary>
	/// Indicates the greater-than operator <c><![CDATA[<=]]></c>.
	/// </summary>
	LessThanOrEqual = 32,

	/// <summary>
	/// Indicates the Boolean true operator <c><see langword="true"/></c>.
	/// </summary>
	True = 64,

	/// <summary>
	/// Indicates the Boolean false operator <c><see langword="false"/></c>.
	/// </summary>
	False = 128,

	/// <summary>
	/// Indicates the equality operators (containing <see cref="Equality"/> and <see cref="Inequality"/>).
	/// </summary>
	/// <seealso cref="Equality"/>
	/// <seealso cref="Inequality"/>
	EqualityOperators = Equality | Inequality,

	/// <summary>
	/// Indicates the equality operators (containing <see cref="GreaterThan"/>, <see cref="GreaterThanOrEqual"/>,
	/// <see cref="LessThan"/> or <see cref="LessThanOrEqual"/>).
	/// </summary>
	/// <seealso cref="Equality"/>
	/// <seealso cref="Inequality"/>
	/// <seealso cref="LessThan"/>
	/// <seealso cref="LessThanOrEqual"/>
	ComparisonOperators = GreaterThan | GreaterThanOrEqual | LessThan | LessThanOrEqual,

	/// <summary>
	/// Indicates the equality operators (containing <see cref="True"/> and <see cref="False"/>).
	/// </summary>
	/// <seealso cref="True"/>
	/// <seealso cref="False"/>
	Boolean = True | False,
}
