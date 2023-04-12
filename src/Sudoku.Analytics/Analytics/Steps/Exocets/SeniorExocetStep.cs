namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Senior Exocet</b> technique.
/// </summary>
public sealed class SeniorExocetStep(
	View[]? views,
	Exocet exocet,
	Mask digitsMask,
	int endoTargetCell,
	int[]? extraHouses,
	ExocetElimination[] eliminations
) : ExocetStep(views, exocet, digitsMask, eliminations)
{
	/// <summary>
	/// Indicates whether the specified instance contains any extra houses.
	/// </summary>
	public bool ContainsExtraHouses => ExtraHouses is not null && Array.Exists(ExtraHouses, static m => m != 0);

	/// <summary>
	/// Indicates the target cell in the cross-line cells' houses.
	/// </summary>
	public int EndoTargetCell { get; } = endoTargetCell;

	/// <summary>
	/// Indicates the extra houses used.
	/// </summary>
	public int[]? ExtraHouses { get; } = extraHouses;

	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + .2M;

	/// <inheritdoc/>
	public override Technique Code => ContainsExtraHouses ? Technique.ComplexSeniorExocet : Technique.SeniorExocet;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new ExtraDifficultyCase[] { new(ExtraDifficultyCaseNames.ExtraHouse, ContainsExtraHouses ? 0 : .2M) };

	[SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
	private string AdditionalFormat
	{
		get
		{
			const string separator = ", ";
			var endoTargetSnippet = R["EndoTarget"]!;
			var endoTargetStr = $"{endoTargetSnippet}{EndoTargetCellStr}";
			if (ExtraHouses is not null)
			{
				scoped var sb = new StringHandler(100);
				var count = 0;
				for (var digit = 0; digit < 9; digit++)
				{
					if (ExtraHouses[digit] is not (var mask and not 0))
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
	}

	private string EndoTargetCellStr => RxCyNotation.ToCellString(EndoTargetCell);
}
