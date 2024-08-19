namespace System;

/// <summary>
/// Represents a mode that indexes a value used by <see cref="Indexed{T}"/> objects.
/// </summary>
/// <seealso cref="Indexed{T}"/>
public enum IndexingMode
{
	/// <summary>
	/// Indicates the indexing mode is to treat the current element as the start position.
	/// </summary>
	Current,

	/// <summary>
	/// Indicates the indexing mode is to treat the memory start position as the start position.
	/// </summary>
	MemoryStart
}
