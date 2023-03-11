namespace Sudoku.Analytics.Metadata;

/// <summary>
/// Represents a level that a step searcher belongs to. This type is used for the displaying of the step information list.
/// </summary>
public enum StepSearcherLevel
{
	/// <summary>
	/// Indicates the level is none.
	/// </summary>
	None = 0,

	/// <summary>
	/// Indicates the level is the elementary searchers,
	/// meaning the techniques found in the current step searcher is the elementary ones.
	/// </summary>
	Elementary,

	/// <summary>
	/// Indicates the level is the moderate searchers,
	/// meaning the techniques found in the current step searcher is more difficult than the elementary ones,
	/// but they aren't very difficult to understand its backing logic.
	/// </summary>
	Moderate,

	/// <summary>
	/// Indicates the level is the hard searchers,
	/// meaning means the techniques found in the current step searcher is more difficult than the moderate ones.
	/// </summary>
	Hard,

	/// <summary>
	/// Indicates the level is the fiendish searchers,
	/// meaning the techniques found in the current step searcher is more difficult than the hard ones.
	/// </summary>
	Fiendish,

	/// <summary>
	/// <para>
	/// Indicates the level is the hidden searchers,
	/// meaning the techniques found in the current step searcher should be hidden no matter how easy the technique is.
	/// </para>
	/// <para>The well-known technique searcher of this level is Brute Force.</para>
	/// </summary>
	Hidden
}
