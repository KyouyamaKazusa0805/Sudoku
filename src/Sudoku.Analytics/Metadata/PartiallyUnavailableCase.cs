namespace Sudoku.Analytics.Metadata;

/// <summary>
/// Represents a case that a <see cref="StepSearcher"/> becomes unavailable.
/// </summary>
/// <remarks>
/// This type is marked <see cref="FlagsAttribute"/>, which means you can use
/// <see cref="PartiallyUnavailableCase"/>.<see langword="operator"/> | to combine multiple fields.
/// </remarks>
[Flags]
public enum PartiallyUnavailableCase
{
	/// <summary>
	/// Indicates the case is none.
	/// </summary>
	None = 0,

	/// <summary>
	/// Indicates the <see cref="StepSearcher"/> is becoming unavailable if it is called to search for technique steps in a Sukaku puzzle.
	/// For example, uniqueness-related <see cref="StepSearcher"/>s.
	/// </summary>
	Sukaku = 1,

	/// <summary>
	/// Indicates the <see cref="StepSearcher"/> produces high time complexity,
	/// to be becoming unavailable if user disabled slow-speed step searchers.
	/// </summary>
	TimeComplexity = 1 << 1,

	/// <summary>
	/// Indicates the <see cref="StepSearcher"/> produces high space complexity,
	/// to be becoming unavailable if user disabled high memory-allocation step searchers.
	/// </summary>
	SpaceComplexity = 1 << 2,

	/// <summary>
	/// Indicates the reserved field. This field may be used for future considerations.
	/// </summary>
	Advanced = 1 << 3
}
