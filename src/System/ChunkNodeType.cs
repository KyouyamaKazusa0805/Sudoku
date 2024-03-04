namespace System;

/// <summary>
/// Represents a type of a <see cref="ChunkNode{T}"/>.
/// </summary>
/// <seealso cref="ChunkNode{T}"/>
public enum ChunkNodeType
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
