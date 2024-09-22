namespace Sudoku.Analytics.Primitives;

/// <summary>
/// Represents a list of constants of <see cref="PencilmarkVisibility"/>.
/// </summary>
/// <seealso cref="PencilmarkVisibility"/>
[method: Obsolete("Don't instantiate this type.", true)]
public readonly struct PencilmarkVisibilities()
{
	/// <summary>
	/// Indicates all visibilities.
	/// </summary>
	public const PencilmarkVisibility All = PencilmarkVisibility.Direct | PencilmarkVisibility.PartialMark | PencilmarkVisibility.FullMark;

	/// <summary>
	/// Indicates only for marks.
	/// </summary>
	public const PencilmarkVisibility Marks = All & ~PencilmarkVisibility.Direct;
}
