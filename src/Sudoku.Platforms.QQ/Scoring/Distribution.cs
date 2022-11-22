namespace Sudoku.Platforms.QQ.Scoring;

/// <summary>
/// Defines a type of distribution.
/// </summary>
public enum Distribution
{
	/// <summary>
	/// Indicates the distribution type is constant distribution.
	/// </summary>
	Constant,

	/// <summary>
	/// Indicates the distribution type is exponent distribution.
	/// </summary>
	Exponent,

	/// <summary>
	/// Indicates the distribution type is normal distribution. Via this distribution type you can get a list of numbers
	/// which contains more middle numbers and less border numbers.
	/// </summary>
	Normal
}
