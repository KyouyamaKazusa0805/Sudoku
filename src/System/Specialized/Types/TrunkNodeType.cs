namespace System;

/// <summary>
/// Represents a type of a <see cref="TrunkNode{T}"/>.
/// </summary>
/// <seealso cref="TrunkNode{T}"/>
public enum TrunkNodeType
{
	/// <summary>
	/// Indicates the type is a value.
	/// </summary>
	Value,

	/// <summary>
	/// Indicates the type is array.
	/// </summary>
	Array,

	/// <summary>
	/// Indicates the type is list.
	/// </summary>
	List
}
