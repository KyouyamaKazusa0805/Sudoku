namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Defines a step whose technique used is the loop.
/// </summary>
public interface ILoopLikeStep : IStep
{
	/// <summary>
	/// <para>Indicates whether the current technique is the nice loop.</para>
	/// <para>
	/// A <b>Nice</b> loop is a loop that all weak links can be gathered to remove candidates
	/// (if possible removable candidates exist).
	/// </para>
	/// </summary>
	/// <returns>
	/// The return value will be a <c><see cref="bool"/>?</c>:
	/// <list type="table">
	/// <item>
	/// <term><see langword="true"/></term>
	/// <description>The technique is the nice loop.</description>
	/// </item>
	/// <item>
	/// <term><see langword="false"/></term>
	/// <description>The technique is not a nice loop, e.g. a normal dynamic loop.</description>
	/// </item>
	/// <item>
	/// <term><see langword="null"/></term>
	/// <description>
	/// The technique is not a loop, but the structure forms a valid loop-shaped one, e.g. guardian.
	/// </description>
	/// </item>
	/// </list>
	/// </returns>
	public bool? IsNice { get; }
}
