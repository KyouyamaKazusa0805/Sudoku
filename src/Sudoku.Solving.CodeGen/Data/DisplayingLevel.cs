namespace Sudoku.Solving.Manual;

/// <summary>
/// Indicates the difficulty level of the step.
/// This enumeration type is used for the displaying of the step information list.
/// </summary>
public enum DisplayingLevel : byte
{
	/// <summary>
	/// Indicates the level is none.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	None = 0,

	/// <summary>
	/// Indicates the level is the elementary searchers, which means the techniques found
	/// in the current step searcher is the elementary ones.
	/// </summary>
	A,

	/// <summary>
	/// Indicates the level is the moderate searchers, which means the techniques found
	/// in the current step searcher is more difficult than the elementary ones,
	/// but they aren't very difficult to comprehend the backing logic.
	/// </summary>
	B,

	/// <summary>
	/// Indicates the level is the hard searchers, which means the techniques found
	/// in the current step searcher is more difficult than the moderate ones.
	/// </summary>
	C,

	/// <summary>
	/// Indicates the level is the fiendish searchers, which means the techniques found
	/// in the current step searcher is more difficult than the hard ones.
	/// </summary>
	D,

	/// <summary>
	/// <para>
	/// Indicates the level is the hidden searchers, which means the techniques found
	/// in the current step searcher should be hidden no matter how easy the technique is.
	/// </para>
	/// <para>The well-known technique searcher of this level is Brute Force.</para>
	/// </summary>
	E
}
