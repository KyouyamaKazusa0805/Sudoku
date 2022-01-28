namespace Sudoku.Resources;

/// <summary>
/// Defines an indexer mode on resource document.
/// </summary>
public enum ResourceDocumentIndexerMode : byte
{
	/// <summary>
	/// Indicates the mode is the basic mode. If the key cannot be found in the document,
	/// the indexer will throw a <see cref="KeyNotFoundException"/>.
	/// </summary>
	Basic,

	/// <summary>
	/// Indicates the mode is the nullable-return mode. If the key cannot be found in the document,
	/// the indexer will return <see langword="null"/> instead of throwing.
	/// </summary>
	NullableReturn
}
