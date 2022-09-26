namespace Sudoku.Solving.Logical.Implementations.Steps;

/// <summary>
/// Provides with a step that is a <b>Senior Exocet</b> technique.
/// </summary>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Exocet"><inheritdoc/></param>
/// <param name="DigitsMask"><inheritdoc/></param>
/// <param name="EndoTargetCell">Indicates the target cell that is embedded into the cross-line cells.</param>
/// <param name="ExtraHousesMask">Indicates the mask that holds the extra houses used.</param>
/// <param name="Eliminations"><inheritdoc/></param>
[StepDisplayingFeature(StepDisplayingFeature.DifficultyRatingNotStable)]
internal sealed record SeniorExocetStep(
	ViewList Views,
	scoped in ExocetPattern Exocet,
	short DigitsMask,
	int EndoTargetCell,
	int[]? ExtraHousesMask,
	ImmutableArray<ExocetElimination> Eliminations
) : ExocetStep(Views, Exocet, DigitsMask, Eliminations), IStepWithPhasedDifficulty
{
	/// <summary>
	/// Indicates whether the specified instance contains any extra houses.
	/// </summary>
	public bool ContainsExtraHouses
		=> ExtraHousesMask is not null && Array.Exists(ExtraHousesMask, static m => m != 0);

	/// <inheritdoc/>
	public override decimal Difficulty => ((IStepWithPhasedDifficulty)this).TotalDifficulty;

	/// <inheritdoc/>
	public decimal BaseDifficulty => 9.6M;

	/// <inheritdoc/>
	public (string Name, decimal Value)[] ExtraDifficultyValues
		=> new[] { (PhasedDifficultyRatingKinds.ExtraHouse, ContainsExtraHouses ? 0 : .2M) };

	/// <inheritdoc/>
	public override Technique TechniqueCode
		=> ContainsExtraHouses ? Technique.ComplexSeniorExocet : Technique.SeniorExocet;

	[ResourceTextFormatter]
	internal string AdditionalFormat()
	{
		const string separator = ", ";
		var endoTargetSnippet = R["EndoTarget"]!;
		var endoTargetStr = $"{endoTargetSnippet}{EndoTargetCellStr}";
		if (ExtraHousesMask is not null)
		{
			scoped var sb = new StringHandler(100);
			var count = 0;
			for (var digit = 0; digit < 9; digit++)
			{
				if (ExtraHousesMask[digit] is not (var mask and not 0))
				{
					continue;
				}

				sb.Append(digit + 1);
				sb.Append(HouseFormatter.Format(mask));
				sb.Append(separator);

				count++;
			}

			if (count != 0)
			{
				sb.RemoveFromEnd(separator.Length);

				var extraHousesIncluded = R["IncludedExtraHouses"]!;
				return $"{endoTargetStr}{extraHousesIncluded}{sb.ToStringAndClear()}";
			}
		}

		return endoTargetStr;
	}

	[ResourceTextFormatter]
	internal string EndoTargetCellStr() => RxCyNotation.ToCellString(EndoTargetCell);
}
