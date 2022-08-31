namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Gurth's Symmetrical Placement</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="SymmetryType">
/// Indicates the symmetry type used. The supported value can only be:
/// <list type="bullet">
/// <item><see cref="SymmetryType.Central"/></item>
/// <item><see cref="SymmetryType.Diagonal"/></item>
/// <item><see cref="SymmetryType.AntiDiagonal"/></item>
/// </list>
/// </param>
/// <param name="MappingRelations">
/// Indicates the mapping relations; in other words, this table shows what digits has symmetrical placement relation
/// to what digits.
/// </param>
[StepDisplayingFeature(StepDisplayingFeature.HideDifficultyRating)]
internal sealed record GurthSymmetricalPlacementStep(
	ConclusionList Conclusions,
	ViewList Views,
	SymmetryType SymmetryType,
	int?[]? MappingRelations
) : SymmetryStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal Difficulty
		=> SymmetryType switch
		{
			SymmetryType.Diagonal or SymmetryType.AntiDiagonal => 7.1M,
			SymmetryType.Central => 7.0M
		};

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.GurthSymmetricalPlacement;

	[FormatItem]
	internal string SymmetryTypeStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => R[$"{SymmetryType}Symmetry"]!;
	}

	[FormatItem]
	internal string MappingStr
	{
		get
		{
			const string separator = ", ";
			if (MappingRelations is not null)
			{
				scoped var sb = new StringHandler(10);
				for (int i = 0; i < 9; i++)
				{
					int? currentMappingRelationDigit = MappingRelations[i];

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
