using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Configuration;
using Sudoku.Concepts;
using Sudoku.Rendering;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Exocet</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digitsMask">Indicates the mask that holds a list of digits used in the pattern.</param>
/// <param name="baseCells">Indicates the base cells used.</param>
/// <param name="targetCells">Indicates the target cells used.</param>
/// <param name="endoTargetCells">Indicates the endo-target cells used.</param>
/// <param name="crosslineCells">Indicates the cross-line cells used.</param>
public abstract partial class ExocetStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[DataMember] Mask digitsMask,
	[DataMember] scoped ref readonly CellMap baseCells,
	[DataMember] scoped ref readonly CellMap targetCells,
	[DataMember] scoped ref readonly CellMap endoTargetCells,
	[DataMember] scoped ref readonly CellMap crosslineCells
) : Step(conclusions, views, options)
{
	/// <summary>
	/// <para>Indicates the delta value of the pattern.</para>
	/// <para>
	/// The values can be -2, -1, 0, 1 and 2, separated with 3 groups:
	/// <list type="table">
	/// <listheader>
	/// <term>Value</term>
	/// <description>Description</description>
	/// </listheader>
	/// <item>
	/// <term>-2 or -1 <![CDATA[(< 0)]]></term>
	/// <description>The base contain more cells than the target, meaning the pattern will be a "Senior Exocet"</description>
	/// </item>
	/// <item>
	/// <term>1 or 2 <![CDATA[(> 0)]]></term>
	/// <description>
	/// The target contain more cells than the base, meaning the pattern will contain extra items like conjugate pairs of other digits
	/// </description>
	/// </item>
	/// <item>
	/// <term>0</term>
	/// <description>The base has same number of cells with the target, a standard "Junior Exocet" will be formed</description>
	/// </item>
	/// </list>
	/// </para>
	/// </summary>
	public Offset Delta => TargetCells.Count - BaseCells.Count;

	/// <inheritdoc/>
	public override decimal BaseDifficulty => Delta switch { -2 or -1 => 9.6M, 0 => 9.4M, 1 or 2 => 9.5M };

	/// <inheritdoc/>
	public override Technique Code => Delta < 0 ? Technique.SeniorExocet : Technique.JuniorExocet;
}
