namespace Sudoku.Solving.Manual.Searchers.Singles;

/// <summary>
/// Defines a step searcher that searches for single steps.
/// </summary>
public interface ISingleStepSearcher : IStepSearcher
{
	/// <summary>
	/// Indicates whether the solver enables the technique full house.
	/// </summary>
	bool EnableFullHouse { get; set; }

	/// <summary>
	/// Indicates whether the solver enables the technique last digit.
	/// </summary>
	bool EnableLastDigit { get; set; }

	/// <summary>
	/// Indicates whether the solver shows the direct lines (cross-hatching information).
	/// </summary>
	/// <remarks>
	/// <para>
	/// <b>Direct line</b>s is a concept that describes the crosshatching information of a hidden single.
	/// For example, in this following grid:
	/// <code><![CDATA[
	/// .---------.---------.---------.
	/// | .  .  . | .  .  . | .  .  . |
	/// | .  .  . | .  .  1 | .  .  . |
	/// | .  .  . | .  .  . | .  .  . |
	/// :---------+---------+---------:
	/// | .  .  1 | x  x  x | .  .  . |
	/// | .  .  . | x  .  x | .  .  . |
	/// | .  .  . | x  x  x | 1  .  . |
	/// :---------+---------+---------:
	/// | .  .  . | .  .  . | .  .  . |
	/// | .  .  . | 1  .  . | .  .  . |
	/// | .  .  . | .  .  . | .  .  . |
	/// '---------'---------'---------'
	/// ]]></code>
	/// The start point of the direct lines are:
	/// <list type="bullet">
	/// <item><c>r4c3(1)</c>, removes the cases of digit 1 for cells <c>r4c456</c>.</item>
	/// <item><c>r2c6(1)</c>, removes the cases of digit 1 for cells <c>r456c6</c>.</item>
	/// <item><c>r6c7(1)</c>, removes the cases of digit 1 for cells <c>r6c456</c>.</item>
	/// <item><c>r8c4(1)</c>, removes the cases of digit 1 for cells <c>r456c4</c>.</item>
	/// </list>
	/// </para>
	/// <para>
	/// All the end points may be displayed using a cross mark ('<c>x</c>'), and the start
	/// point may be used a circle mark ('<c>o</c>').
	/// </para>
	/// </remarks>
	bool ShowDirectLines { get; set; }
}
