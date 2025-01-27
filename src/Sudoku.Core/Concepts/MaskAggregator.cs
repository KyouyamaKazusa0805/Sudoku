namespace Sudoku.Concepts;

/// <summary>
/// Represents an aggregator of mask merging.
/// </summary>
public enum MaskAggregator
{
	/// <summary>
	/// Indicates the method is "or".
	/// </summary>
	Or = '|',

	/// <summary>
	/// Indicates the method is "and".
	/// </summary>
	And = '&',

	/// <summary>
	/// Indicates the method is "and not".
	/// </summary>
	AndNot = '~'
}
