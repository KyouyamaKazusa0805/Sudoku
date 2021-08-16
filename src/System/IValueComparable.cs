#undef SUPPORT_COMPARISON_OPERATORS

namespace System;

/// <summary>
/// Defines a generalized comparison method that a <see langword="struct"/> implements
/// to create a type-specific comparison method for ordering or sorting its instances.
/// </summary>
/// <typeparam name="TStruct">
/// The type of objects to compare. Here it should be a <see langword="struct"/>.
/// </typeparam>
public interface IValueComparable<TStruct> : IComparable<TStruct> where TStruct : struct, IValueComparable<TStruct>
{
	/// <summary>
	/// Compares the current instance with another object of the same type and returns
	/// an integer that indicates whether the current instance precedes, follows, or
	/// occurs in the same position in the sort order as the other object.
	/// </summary>
	/// <param name="other">An object to compare with this instance.</param>
	/// <returns>
	/// A value that indicates the relative order of the objects being compared. The
	/// return value has these meanings:
	/// <list type="table">
	/// <listheader>
	/// <term>Value</term>
	/// <description>Meaning</description>
	/// </listheader>
	/// <item>
	/// <term><c><![CDATA[< 0]]></c></term>
	/// <description>This instance precedes other in the sort order.</description>
	/// </item>
	/// <item>
	/// <term><c>0</c></term>
	/// <description>This instance occurs in the same position in the sort order as other.</description>
	/// </item>
	/// <item>
	/// <term><c><![CDATA[> 0]]></c></term>
	/// <description>This instance follows other in the sort order.</description>
	/// </item>
	/// </list>
	/// </returns>
	int CompareTo(in TStruct other);

	/// <inheritdoc/>
	int IComparable<TStruct>.CompareTo(TStruct other) => CompareTo(other);


#if SUPPORT_COMPARISON_OPERATORS
	/// <summary>
	/// Determines whetehr the left-side <typeparamref name="TStruct"/>-typed instance holds a greater value
	/// than the same-typed right-side one.
	/// </summary>
	/// <param name="left">The left instance to compare.</param>
	/// <param name="right">The right instance to compare.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	static abstract bool operator >(in TStruct left, in TStruct right);

	/// <summary>
	/// Determines whetehr the left-side <typeparamref name="TStruct"/>-typed instance holds a less value
	/// than the right-side same-typed one.
	/// </summary>
	/// <param name="left">The left instance to compare.</param>
	/// <param name="right">The right instance to compare.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	static abstract bool operator <(in TStruct left, in TStruct right);

	/// <summary>
	/// Determines whetehr the left-side <typeparamref name="TStruct"/>-typed instance holds a greater value
	/// than the right-side same-typed one, or two values are equal.
	/// </summary>
	/// <param name="left">The left instance to compare.</param>
	/// <param name="right">The right instance to compare.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	static abstract bool operator >=(in TStruct left, in TStruct right);

	/// <summary>
	/// Determines whetehr the left-side <typeparamref name="TStruct"/>-typed instance holds a less value
	/// than the right-side same-typed one, or two values are equal.
	/// </summary>
	/// <param name="left">The left instance to compare.</param>
	/// <param name="right">The right instance to compare.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	static abstract bool operator <=(in TStruct left, in TStruct right);
#endif
}
