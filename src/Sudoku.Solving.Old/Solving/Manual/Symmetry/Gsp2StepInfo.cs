namespace Sudoku.Solving.Manual.Symmetry;

/// <summary>
/// Provides a usage of <b>Gurth's symmetrical placement 2</b> (GSP2) technique.
/// </summary>
/// <param name="Conclusions">All conclusions.</param>
/// <param name="Views">All views.</param>
/// <param name="SymmetryType">The symmetry type used.</param>
/// <param name="SwappingTable">Indicates the swapping table.</param>
/// <param name="MappingTable">
/// The mapping table. The value is always not <see langword="null"/> unless the current instance
/// contains multiple different symmetry types.
/// </param>
public sealed record Gsp2StepInfo(
	IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
	SymmetryType SymmetryType, int[]?[]? SwappingTable, int?[]? MappingTable
) : SymmetryStepInfo(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal Difficulty => 8.5M;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Nightmare;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.Gsp2;

	[FormatItem]
	private string SymmetryTypeSnippet
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => TextResources.Current.SymmetryTypeSnippet;
	}

	[FormatItem]
	private string SymmetryTypeName
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => SymmetryType switch
		{
			SymmetryType.Central => TextResources.Current.SymmetryTypeCentral,
			SymmetryType.Diagonal => TextResources.Current.SymmetryTypeDiagnoal,
			SymmetryType.AntiDiagonal => TextResources.Current.SymmetryTypeAntiDiagonal
		};
	}

	[FormatItem]
	[NotNullIfNotNull(nameof(MappingTable))]
	private string? MappingRelations
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			if (MappingTable is null)
			{
				return null;
			}
			else
			{
				const string separator = ", ";

				var sb = new StringHandler(initialCapacity: 100);
				for (int i = 0; i < 9; i++)
				{
					int? value = MappingTable[i];

					sb.Append(i + 1);

					if (value is { } v && value != i)
					{
						sb.Append(" -> ");
						sb.Append(v + 1);
					}

					sb.Append(separator);
				}

				sb.RemoveFromEnd(separator.Length);
				return $"{(string)TextResources.Current.MappingRelationSnippet}{sb.ToStringAndClear()}";
			}
		}
	}

	[FormatItem]
	private string ClosedBracket
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => TextResources.Current.ClosedBracket;
	}

	[FormatItem]
	private string GspBaseInfo
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => $"{Name}: {SymmetryTypeSnippet}{SymmetryTypeName}{MappingRelations}";
	}

	[FormatItem]
	[NotNullIfNotNull(nameof(SwappingTable))]
	private string? SwappingStr
	{
		get
		{
			if (SwappingTable is { Length: not 0 })
			{
				const string separator = ", ";

				var sb = new StringHandler(initialCapacity: 100);
				sb.Append((string)TextResources.Current.SymmetrySnippet);

				foreach (int[]? swappingRegionPair in SwappingTable)
				{
					if (swappingRegionPair is not null)
					{
						sb.Append(new RegionCollection(swappingRegionPair[0]).ToString());
						sb.Append((string)TextResources.Current.WithKeyword);
						sb.Append(new RegionCollection(swappingRegionPair[1]).ToString());
						sb.Append(separator);
					}
				}

				sb.Append((string)TextResources.Current.SymmetryThenWeGet);

				return sb.ToStringAndClear();
			}
			else
			{
				return null;
			}
		}
	}
}
