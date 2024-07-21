namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Defines a behavior by source generator on generating <see cref="object.GetHashCode"/> overridden methods.
/// </summary>
/// <seealso cref="object.GetHashCode"/>
public enum GetHashCodeBehavior
{
	/// <summary>
	/// Indicates the source generator will adopt an intelligent way to generate hashing expression.
	/// If none of all data members in a type marked <see cref="HashCodeMemberAttribute"/>, a <see cref="NotSupportedException"/>
	/// will be thrown; if one member is marked and its type can be directly converted into an <see cref="int"/>, then use itself;
	/// otherwise, call <see cref="HashCode.Combine{T1}(T1)"/> method set or use <see cref="HashCode.Add{T}(T)"/>.
	/// </summary>
	Intelligent,

	/// <summary>
	/// Indicates throws <see cref="NotSupportedException"/>.
	/// </summary>
	ThrowNotSupportedException,

	/// <summary>
	/// Indicates the method will be made <see langword="abstract"/>.
	/// </summary>
	MakeAbstract
}
