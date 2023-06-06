namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Gurth's Symmetrical Placement</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="symmetryType">
/// Indicates the symmetry type used. The supported value can only be:
/// <list type="bullet">
/// <item><see cref="SymmetryType.Central"/></item>
/// <item><see cref="SymmetryType.Diagonal"/></item>
/// <item><see cref="SymmetryType.AntiDiagonal"/></item>
/// </list>
/// </param>
/// <param name="mapping">
/// Indicates the mapping relations; in other words, this table shows what digits has symmetrical placement relation to what digits.
/// </param>
public sealed partial class GurthSymmetricalPlacementStep(
	Conclusion[] conclusions,
	View[]? views,
	[PrimaryConstructorParameter] SymmetryType symmetryType,
	[PrimaryConstructorParameter] Digit?[]? mapping
) : SymmetryStep(conclusions, views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty
		=> SymmetryType switch { SymmetryType.Diagonal or SymmetryType.AntiDiagonal => 7.1M, SymmetryType.Central => 7.0M };

	/// <inheritdoc/>
	public override Technique Code => Technique.GurthSymmetricalPlacement;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?> { { EnglishLanguage, new[] { SymmetryTypeStr, MappingStr } }, { ChineseLanguage, new[] { SymmetryTypeStr, MappingStr } } };

	private string SymmetryTypeStr => R[$"{SymmetryType}Symmetry"]!;

	private string MappingStr
	{
		get
		{
			var separator = R["Comma"]!;
			if (Mapping is not null)
			{
				scoped var sb = new StringHandler(10);
				for (var i = 0; i < 9; i++)
				{
					var currentMappingRelationDigit = Mapping[i];

					sb.Append(i + 1);
					sb.Append(currentMappingRelationDigit is { } c && c != i ? $" -> {c + 1}" : string.Empty);
					sb.Append(separator);
				}

				sb.RemoveFromEnd(separator.Length);
				return sb.ToStringAndClear();
			}
			else
			{
				return R["NoMappingRelation"]!;
			}
		}
	}
}
