using System.SourceGeneration;
using Sudoku.Concepts;
using Sudoku.Rendering;
using static System.Numerics.BitOperations;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Subset</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="house">The house that pattern lies in.</param>
/// <param name="cells">Indicates the cells used.</param>
/// <param name="digitsMask">Indicates the mask that contains all digits used.</param>
public abstract partial class SubsetStep(
	Conclusion[] conclusions,
	View[]? views,
	[DataMember] House house,
	[DataMember] scoped in CellMap cells,
	[DataMember] Mask digitsMask
) : Step(conclusions, views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 3.0M;

	/// <summary>
	/// Indicates the number of cells used.
	/// Due to the technique logic, you can also treat the result value of this property as the number of digits used.
	/// </summary>
	public int Size => PopCount((uint)DigitsMask);
}
