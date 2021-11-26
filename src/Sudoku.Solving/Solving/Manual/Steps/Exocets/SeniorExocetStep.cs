namespace Sudoku.Solving.Manual.Steps.Exocets;

/// <summary>
/// Provides with a step that is a <b>Senior Exocet</b> technique.
/// </summary>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Exocet"><inheritdoc/></param>
/// <param name="DigitsMask"><inheritdoc/></param>
/// <param name="EndoTargetCell">Indicates the target cell that is embedded into the cross-line cells.</param>
/// <param name="ExtraRegionsMask">Indicates the mask tnat holds the extra regions used.</param>
/// <param name="Eliminations"><inheritdoc/></param>
public sealed record SeniorExocetStep(
	ImmutableArray<PresentationData> Views,
	in ExocetPattern Exocet,
	short DigitsMask,
	int EndoTargetCell,
	int[]? ExtraRegionsMask,
	ImmutableArray<ExocetElimination> Eliminations
) : ExocetStep(Views, Exocet, DigitsMask, Eliminations)
{
	/// <summary>
	/// Indicates whether the specified instance contains any extra regions.
	/// </summary>
	public bool ContainsExtraRegions =>
		ExtraRegionsMask is not null && Array.Exists(ExtraRegionsMask, static m => m != 0);

	/// <inheritdoc/>
	public override decimal Difficulty =>
		9.6M // Base difficulty.
		+ (ContainsExtraRegions ? 0 : .2M); // Extra region difficulty.

	/// <inheritdoc/>
	public override Technique TechniqueCode => ContainsExtraRegions ? Technique.ComplexSe : Technique.Se;

	[FormatItem]
	private string AdditionalFormat
	{
		get
		{
			const string separator = ", ";
			string endoTargetSnippet = TextResources.Current.EndoTargetSnippet;
			string endoTargetStr = $"{endoTargetSnippet}{EndoTargetCellStr}";
			if (ExtraRegionsMask is not null)
			{
				var sb = new StringHandler(initialCapacity: 100);
				int count = 0;
				for (int digit = 0; digit < 9; digit++)
				{
					if (ExtraRegionsMask[digit] is not (var mask and not 0))
					{
						continue;
					}

					sb.Append(digit + 1);
					sb.Append(new RegionCollection(mask.GetAllSets()).ToString());
					sb.Append(separator);

					count++;
				}

				if (count != 0)
				{
					sb.RemoveFromEnd(separator.Length);

					string extraRegionsIncluded = TextResources.Current.IncludedExtraRegionSnippet;
					return $"{endoTargetStr}{extraRegionsIncluded}{sb.ToStringAndClear()}";
				}
			}

			return endoTargetStr;
		}
	}

	[FormatItem]
	private string EndoTargetCellStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Cells { EndoTargetCell }.ToString();
	}
}
