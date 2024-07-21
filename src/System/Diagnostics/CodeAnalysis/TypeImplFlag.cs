namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Represents a flag that supports for auto implementation.
/// </summary>
[Flags]
public enum TypeImplFlag
{
	/// <summary>
	/// Indicates no elements will be generated.
	/// </summary>
	None = 0,

	/// <summary>
	/// Indicates the method <see cref="object.Equals(object?)"/> will be generated.
	/// </summary>
	/// <seealso cref="object.Equals(object?)"/>
	Object_Equals = 1 << 0,

	/// <summary>
	/// Indicates the method <see cref="object.GetHashCode"/> will be generated.
	/// </summary>
	/// <seealso cref="object.GetHashCode"/>
	Object_GetHashCode = 1 << 1,

	/// <summary>
	/// Indicates the method <see cref="object.ToString"/> will be generated.
	/// </summary>
	/// <seealso cref="object.ToString"/>
	Object_ToString = 1 << 2,

	/// <summary>
	/// Indicates the equality operators defined in <see cref="IEqualityOperators{TSelf, TOther, TResult}"/> will be generated.
	/// </summary>
	/// <seealso cref="IEqualityOperators{TSelf, TOther, TResult}.op_Equality(TSelf, TOther)"/>
	/// <seealso cref="IEqualityOperators{TSelf, TOther, TResult}.op_Inequality(TSelf, TOther)"/>
	EqualityOperators = 1 << 3,

	/// <summary>
	/// Indicates the comparison operators defined in <see cref="IComparisonOperators{TSelf, TOther, TResult}"/> will be generated.
	/// </summary>
	/// <seealso cref="IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)"/>
	/// <seealso cref="IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)"/>
	/// <seealso cref="IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)"/>
	/// <seealso cref="IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)"/>
	ComparisonOperators = 1 << 4,

	/// <summary>
	/// Indicates all the methods
	/// <see cref="object.Equals(object?)"/>, <see cref="object.GetHashCode"/> and <see cref="object.ToString"/>
	/// will be generated.
	/// </summary>
	AllObjectMethods = Object_Equals | Object_GetHashCode | Object_ToString,

	/// <summary>
	/// Indicates all the operators
	/// <see cref="IEqualityOperators{TSelf, TOther, TResult}"/> and <see cref="IComparisonOperators{TSelf, TOther, TResult}"/>
	/// will be generated.
	/// </summary>
	AllOperators = EqualityOperators | ComparisonOperators
}
