namespace System;

/// <summary>
/// Defines a generalized method that a <see langword="struct"/> implements to create a type-specific method
/// for determining equality of instances.
/// </summary>
/// <typeparam name="TStruct">
/// The type of objects to compare. Here it should be a <see langword="struct"/>.
/// </typeparam>
public interface IValueEquatable<[Self] TStruct> : IEquatable<TStruct>
where TStruct : struct, IValueEquatable<TStruct>
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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	bool IEquatable<TStruct>.Equals(TStruct other) => Equals(other);
}
