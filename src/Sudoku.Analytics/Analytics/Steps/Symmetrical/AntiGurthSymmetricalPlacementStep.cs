using System.Text;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Configuration;
using Sudoku.Concepts;
using Sudoku.Rendering;
using static Sudoku.Analytics.Strings.StringsAccessor;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Anti- Gurth's Symmetrical Placement</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="symmetricType">
/// Indicates the symmetric type used. The supported value can only be:
/// <list type="bullet">
/// <item><see cref="SymmetricType.Central"/></item>
/// <item><see cref="SymmetricType.Diagonal"/></item>
/// <item><see cref="SymmetricType.AntiDiagonal"/></item>
/// <item><see cref="SymmetricType.XAxis"/></item>
/// <item><see cref="SymmetricType.YAxis"/></item>
/// </list>
/// </param>
/// <param name="mapping"><inheritdoc/></param>
public sealed class AntiGurthSymmetricalPlacementStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	SymmetricType symmetricType,
	Digit?[]? mapping
) : GurthSymmetricalPlacementStep(conclusions, views, options, symmetricType, mapping)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + .3M;

	/// <inheritdoc/>
	public override Technique Code => Technique.AntiGurthSymmetricalPlacement;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [SymmetryTypeStr, MappingStr]), new(ChineseLanguage, [SymmetryTypeStr, MappingStr])];

	private string SymmetryTypeStr => GetString($"{SymmetricType}Symmetry")!;

	private string MappingStr
	{
		get
		{
			var comma = GetString("Comma")!;
			if (Mapping is not null)
			{
				scoped var sb = new StringHandler(10);
				for (var i = 0; i < 9; i++)
				{
					var currentMappingRelationDigit = Mapping[i];

					sb.Append(i + 1);
					sb.Append(currentMappingRelationDigit is { } c && c != i ? $" -> {c + 1}" : string.Empty);
					sb.Append(comma);
				}

				sb.RemoveFromEnd(comma.Length);
				return sb.ToStringAndClear();
			}

			return GetString("NoMappingRelation")!;
		}
	}
}
