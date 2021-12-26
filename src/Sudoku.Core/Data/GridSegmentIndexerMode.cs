namespace Sudoku.Data;

/// <summary>
/// Defines a way to find the element at the specified index in <see cref="GridSegment"/>.
/// </summary>
public enum GridSegmentIndexerMode : byte
{
	/// <summary>
	/// Indicates the indexing way is to index by cell index.
	/// In this mode, you should specify the value from 0 to 81, and the indexer will throw exceptions
	/// when the specified cell doesn't contain in the collection.
	/// </summary>
	ByCellIndex,

	/// <summary>
	/// Indicates the indexing way is to index by array index.
	/// In this mode, you should specify the value from 0 to length of the elements in the segment.
	/// </summary>
	ByArrayIndex
}