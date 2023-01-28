namespace System.Diagnostics.CodeGen;

/// <summary>
/// Defines a mode to compare two instances of equality.
/// </summary>
public enum EqualityComparisonMode : int
{
	/// <summary>
	/// Indicates the source generator will disable the equality comparison.
	/// </summary>
	Disable = 0,

	/// <summary>
	/// Indicates the source generator will decide the suitable comparison mode.
	/// </summary>
	Intelligent,

	/// <summary>
	/// Indicates the comparison mode is to call <see cref="object"/>.<see langword="operator"/> ==(<see cref="object"/>, <see cref="object"/>)
	/// to check whether two instances are considered equal.
	/// </summary>
	EqualityOperator,

	/// <summary>
	/// Indicates the comparison mode is to call <c><see cref="IEquatable{T}.Equals(T)"/></c>
	/// to check whether two instances are considered equal.
	/// </summary>
	/// <seealso cref="IEquatable{T}.Equals(T)"/>
	InstanceEqualsMethod,

	/// <summary>
	/// Indicates the comparison mode is to call <c><see cref="IComparable{T}.CompareTo(T)"/></c>
	/// to check whether two instances are considered equal.
	/// </summary>
	/// <seealso cref="IComparable{T}.CompareTo(T)"/>
	InstanceCompareToMethod,

	/// <summary>
	/// Indicates the comparison mode is to call <c><see cref="EqualityComparer{T}.Equals(T, T)"/></c>
	/// via instance <see cref="EqualityComparer{T}.Default"/> to check whether two instances are considered equal.
	/// </summary>
	/// <seealso cref="EqualityComparer{T}.Default"/>
	/// <seealso cref="EqualityComparer{T}.Equals(T, T)"/>
	EqualityComparerDefaultInstance,

	/// <summary>
	/// Indicates the comparison mode is to compare two objects' reference, via calling <see cref="object.ReferenceEquals(object?, object?)"/>.
	/// </summary>
	/// <seealso cref="object.ReferenceEquals(object?, object?)"/>
	ObjectReference
}
