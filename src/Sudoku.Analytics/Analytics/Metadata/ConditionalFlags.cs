using Sudoku.Analytics.StepSearchers;

namespace Sudoku.Analytics.Metadata;

/// <summary>
/// Represents a list of cases that describes some cases that <see cref="StepSearcher"/> is partially allowed
/// in searching or gathering operation. Fields in this type can be used by <see cref="StepSearcherAttribute.Flags"/> property assigning.
/// </summary>
/// <remarks><i>
/// This type is marked <see cref="FlagsAttribute"/>, which means you can use
/// <see cref="ConditionalFlags"/>.<see langword="operator"/> | to combine multiple fields.
/// </i></remarks>
/// <seealso cref="StepSearcher"/>
/// <seealso cref="StepSearcherAttribute.Flags"/>
[Flags]
public enum ConditionalFlags
{
	/// <summary>
	/// Indicates the step searcher can be called anywhere as long it is enabled. This is also the default value of this enumeration type.
	/// </summary>
	Default = 0,

	/// <summary>
	/// Indicates the <see cref="StepSearcher"/> is only enabled for standard sudoku puzzles, which means other variants are disallowed
	/// using this <see cref="StepSearcher"/> instance. For example, <see cref="UniqueRectangleStepSearcher"/>
	/// can only be used for searching in a standard sudoku, sukakus are disabled.
	/// </summary>
	/// <seealso cref="UniqueRectangleStepSearcher"/>
	Standard = 1,

	/// <summary>
	/// Indicates a <see cref="StepSearcher"/> will produce high time complexity,
	/// meaning it will be disabled if a user want to disable high time-complexity algorithms.
	/// </summary>
	TimeComplexity = 1 << 1,

	/// <summary>
	/// Indicates the <see cref="StepSearcher"/> will produce high space complexity,
	/// meaning it will be disabled if a user want to disable high space-complexity algorithms.
	/// </summary>
	SpaceComplexity = 1 << 2,

	/// <summary>
	/// Indicates the reserved field. This field may be used for future considerations.
	/// </summary>
	Advanced = 1 << 3
}
