#undef SUPPORT_COMPARISON_OPERATORS

namespace System;

/// <summary>
/// Defines a generalized method that a <see langword="struct"/> implements to create a type-specific method
/// for detemining equality of instances.
/// </summary>
/// <typeparam name="TStruct">
/// The type of objects to compare. Here it should be a <see langword="struct"/>.
/// </typeparam>
public interface IValueEquatable<TStruct> : IEquatable<TStruct> where TStruct : struct, IValueEquatable<TStruct>
{
	/// <summary>
	/// Indicates whether the current object is equal to another object of the same type.
	/// </summary>
	/// <param name="other">An object to compare with this object.</param>
	/// <returns>
	/// <see langword="true"/> if the current object is equal to the other parameter;
	/// otherwise, <see langword="false"/>.
	/// </returns>
	bool Equals(in TStruct other);

	/// <inheritdoc/>
	bool IEquatable<TStruct>.Equals(TStruct other) => Equals(other);


#if SUPPORT_COMPARISON_OPERATORS
	/// <summary>
	/// Determine whether the two <typeparamref name="TStruct"/>-typed instance hold a same value to compare.
	/// </summary>
	/// <param name="left">The left instance to compare.</param>
	/// <param name="right">The right instance to compare.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	static abstract bool operator ==(in TStruct left, in TStruct right);

	/// <summary>
	/// Determine whether the two <typeparamref name="TStruct"/>-typed instance hold different values to compare.
	/// </summary>
	/// <param name="left">The left instance to compare.</param>
	/// <param name="right">The right instance to compare.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	static abstract bool operator !=(in TStruct left, in TStruct right);
#endif
}
