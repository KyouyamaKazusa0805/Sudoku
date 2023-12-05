using System.Runtime.CompilerServices;
using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Configuration;
using Sudoku.Analytics.Rating;
using Sudoku.Concepts.ObjectModel;
using Sudoku.Rendering;
using static System.Algorithm.Sequences;
using static Sudoku.Analytics.Strings.StringsAccessor;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Death Blossom (House Blooming)</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="house">Indicates the pivot house.</param>
/// <param name="digit">Indicates the digit.</param>
/// <param name="branches">Indicates the branches.</param>
/// <param name="zDigitsMask">Indicates the digits mask as eliminations.</param>
public sealed partial class HouseDeathBlossomStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[Data] House house,
	[Data] Digit digit,
	[Data] HouseBlossomBranchCollection branches,
	[Data] Mask zDigitsMask
) : AlmostLockedSetsStep(conclusions, views, options), IEquatableStep<HouseDeathBlossomStep>
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 8.3M;

	/// <inheritdoc/>
	public override Technique Code => Technique.HouseDeathBlossom;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [HouseStr, BranchesStr]), new(ChineseLanguage, [HouseStr, BranchesStr])];

	/// <inheritdoc/>
	public override ExtraDifficultyFactor[] ExtraDifficultyFactors
		=> [new(ExtraDifficultyFactorNames.Petals, A002024(Branches.Count) * .1M)];

	private string HouseStr => Options.Converter.HouseConverter(1 << House);

	private string BranchesStr
		=> string.Join(GetString("Comma"), [.. from b in Branches select $"{Options.Converter.CellConverter([b.Key])} - {b.AlsPattern}"]);


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IEquatableStep<HouseDeathBlossomStep>.operator ==(HouseDeathBlossomStep left, HouseDeathBlossomStep right)
		=> (left.House, left.Digit, left.Branches) == (right.House, right.Digit, right.Branches);
}
