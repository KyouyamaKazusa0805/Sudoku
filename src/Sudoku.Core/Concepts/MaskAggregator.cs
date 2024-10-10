namespace Sudoku.Concepts;

/// <summary>
/// Represents an aggregator of mask merging.
/// </summary>
public static class MaskAggregator
{
	/// <summary>
	/// Indicates the method is "or".
	/// </summary>
	public const char Or = '|';

	/// <summary>
	/// Indicates the method is "and".
	/// </summary>
	public const char And = '&';

	/// <summary>
	/// Indicates the method is "and not".
	/// </summary>
	public const char AndNot = '~';
}
