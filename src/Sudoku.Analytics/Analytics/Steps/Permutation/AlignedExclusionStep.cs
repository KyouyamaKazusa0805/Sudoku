using System.Runtime.CompilerServices;
using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Configuration;
using Sudoku.Concepts;
using Sudoku.Rendering;
using static Sudoku.Analytics.Strings.StringsAccessor;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Aligned Exclusion</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="cells">Indicates the target cells used in the pattern.</param>
/// <param name="lockedCombinations">Indicates all locked combinations.</param>
public sealed partial class AlignedExclusionStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[DataMember] scoped ref readonly CellMap cells,
	[DataMember] (Digit[], Cell)[] lockedCombinations
) : PermutationStep(conclusions, views, options), IEquatableStep<AlignedExclusionStep>
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty
		=> Size switch
		{
			2 => 6.2M,
			3 => 7.5M,
			4 => 8.1M,
			5 => 8.4M,
			_ => throw new NotSupportedException("The subset is too complex to be calculated.")
		};

	/// <summary>
	/// Indicates the size of the permutation.
	/// </summary>
	public Count Size => Cells.Count;

	/// <inheritdoc/>
	public override Technique Code
		=> Size switch
		{
			>= 2 and <= 5 => Technique.AlignedPairExclusion + (short)(Size - 2),
			_ => throw new NotSupportedException("The subset is too complex to be calculated.")
		};

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(ChineseLanguage, [CellsStr, ConclusionNegatedStr]), new(EnglishLanguage, [CellsStr, ConclusionNegatedStr])];

	private string CellsStr => Options.Converter.CellConverter(Cells);

	private string ConclusionNegatedStr => Options.Converter.ConclusionConverter(from c in Conclusions select ~c);


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IEquatableStep<AlignedExclusionStep>.operator ==(AlignedExclusionStep left, AlignedExclusionStep right)
		=> left.Cells == right.Cells;
}
