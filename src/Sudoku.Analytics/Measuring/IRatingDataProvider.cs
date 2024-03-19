namespace Sudoku.Measuring;

/// <summary>
/// Represents a rating data provider.
/// </summary>
internal interface IRatingDataProvider
{
	/// <summary>
	/// Indicates the factor name that describes the difficulty of this factor.
	/// </summary>
	public abstract string FactorName { get; }

	/// <summary>
	/// Indicates the factor value that describes how difficult the factor is.
	/// </summary>
	public abstract decimal Value { get; }
}
