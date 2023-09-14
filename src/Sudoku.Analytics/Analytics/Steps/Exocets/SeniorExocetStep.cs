using System.Diagnostics.CodeAnalysis;
using System.SourceGeneration;
using System.Text;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Eliminations;
using Sudoku.Analytics.Rating;
using Sudoku.DataModel;
using Sudoku.Rendering;
using Sudoku.Text.Notation;
using static Sudoku.Analytics.Strings.StringsAccessor;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Senior Exocet</b> technique.
/// </summary>
/// <param name="views"><inheritdoc/></param>
/// <param name="exocet"><inheritdoc/></param>
/// <param name="digitsMask"><inheritdoc/></param>
/// <param name="endoTargetCell">Indicates the target cell in the cross-line cells' houses.</param>
/// <param name="extraHouses">Indicates the extra houses used.</param>
/// <param name="eliminations"><inheritdoc/></param>
public sealed partial class SeniorExocetStep(
	View[]? views,
	Exocet exocet,
	Mask digitsMask,
	[DataMember] Cell endoTargetCell,
	[DataMember] House[]? extraHouses,
	ExocetElimination[] eliminations
) : ExocetStep(views, exocet, digitsMask, eliminations)
{
	/// <summary>
	/// Indicates whether the specified instance contains any extra houses.
	/// </summary>
	public bool ContainsExtraHouses => ExtraHouses is not null && Array.Exists(ExtraHouses, static m => m != 0);

	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + .2M;

	/// <inheritdoc/>
	public override Technique Code => ContainsExtraHouses ? Technique.ComplexSeniorExocet : Technique.SeniorExocet;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> [new(ExtraDifficultyCaseNames.ExtraHouse, ContainsExtraHouses ? 0 : .2M)];

	[SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
	private string AdditionalFormat
	{
		get
		{
			const string separator = ", ";
			var endoTargetSnippet = GetString("EndoTarget")!;
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
					sb.Append(HouseNotation.ToMaskString(mask));
					sb.Append(separator);

					count++;
				}

				if (count != 0)
				{
					sb.RemoveFromEnd(separator.Length);

					var extraHousesIncluded = GetString("IncludedExtraHouses")!;
					return $"{endoTargetStr}{extraHousesIncluded}{sb.ToStringAndClear()}";
				}
			}

			return endoTargetStr;
		}
	}

	private string EndoTargetCellStr => CellNotation.ToString(EndoTargetCell);
}
