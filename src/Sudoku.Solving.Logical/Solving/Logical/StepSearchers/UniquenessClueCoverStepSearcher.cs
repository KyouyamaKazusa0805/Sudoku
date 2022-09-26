namespace Sudoku.Solving.Logical.StepSearchers;

[StepSearcher]
[SukakuNotSupported]
internal sealed partial class UniquenessClueCoverStepSearcher : IUniquenessClueCoverStepSearcher
{
	/// <inheritdoc/>
	/// <remarks>
	/// <para><b>Developer notes</b></para>
	/// <para>
	/// The technique is very complex that I cannot enumerate all possible cases.
	/// Here I only give you some cases that satisfy the pattern:
	/// <list type="bullet">
	/// <item><see href="http://sudopedia.enjoysudoku.com/Uniqueness_Clue_Cover.html"/></item>
	/// </list>
	/// </para>
	/// </remarks>
	public IStep? GetAll(scoped in LogicalAnalysisContext context)
	{
		scoped ref readonly var grid = ref context.Grid;
		var accumulator = context.Accumulator!;
		var onlyFindOne = context.OnlyFindOne;
		if (CheckType2(accumulator, grid, onlyFindOne) is { } type2Step)
		{
			return type2Step;
		}

		return null;
	}

	/// <summary>
	/// Checks for the type 2. The sketch is like:
	/// <code><![CDATA[
	/// +----------+---------+----------+
	/// |  .  .  . | .  .  . | *1  .  . |
	/// | =1  .  . | .  .  . | =2  .  . |
	/// | *2  .  . | .  .  . |  .  .  . |
	/// +----------+---------+----------+
	/// ]]></code>
	/// In the sketch, the notation <c>*n</c> means the value cell is filled with that digit <c>n</c>,
	/// and the notation <c>=n</c> means the digit <c>n</c> is the correct value.
	/// <para>
	/// The test examples:
	/// <list type="bullet">
	/// <item>
	/// <c>000000000000000002000001000001030040005607300030020008002060500650008904900400007</c>
	/// </item>
	/// <item>
	/// <c>000000000000000001000002000002300400005600017080009003040+20700805310070072095006+4:815 816 825 826 835 977 978 981</c>
	/// </item>
	/// </list>
	/// </para>
	/// </summary>
	private IStep? CheckType2(ICollection<IStep> accumulator, scoped in Grid grid, bool onlyFindOne)
	{
		for (var chuteIndex = 0; chuteIndex < 6; chuteIndex++)
		{
			var (chute, isRow, _) = Chutes[chuteIndex];

			if (chute - EmptyCells is not [var c1, var c2] valueCells)
			{
				// The number of the value cells must be 2 for this type.
				continue;
			}

			if (PeersMap[c1].Contains(c2) || PeersMap[c2].Contains(c1))
			{
				// The two value cells cannot lie in a same house.
				continue;
			}

			int c1Digit = grid[c1], c2Digit = grid[c2];
			if (c1Digit == c2Digit)
			{
				// The two value cells cannot be filled with a same digit.
				continue;
			}

			var digitsMask = (short)(1 << c1Digit | 1 << c2Digit);
			var elimHouseType = isRow ? HouseType.Column : HouseType.Row;
			var excludedHouseType = isRow ? HouseType.Row : HouseType.Column;
			var excludedLines = HousesMap[c1.ToHouseIndex(excludedHouseType)] | HousesMap[c2.ToHouseIndex(excludedHouseType)];
			var elimCells = chute - excludedLines & (
				HousesMap[c1.ToHouseIndex(elimHouseType)] | HousesMap[c2.ToHouseIndex(elimHouseType)]
			);

			var conclusions = new List<Conclusion>(2);
			foreach (var elimCell in elimCells)
			{
				var correspondingValueCell = (HousesMap[elimCell.ToHouseIndex(elimHouseType)] & chute & valueCells)[0];
				var elimDigit = TrailingZeroCount((short)(digitsMask & ~(1 << grid[correspondingValueCell])));
				if (CandidatesMap[elimDigit].Contains(elimCell))
				{
					conclusions.Add(new(Assignment, elimCell, elimDigit));
				}
			}
			if (conclusions.Count == 0)
			{
				// No eliminations can be found.
				continue;
			}

			var step = new UniquenessClueCoverType2Step(
				conclusions.ToImmutableArray(),
				ImmutableArray.Create(
					View.Empty
						| new ChuteViewNode(DisplayColorKind.Normal, chuteIndex)
						| new CellViewNode[] { new(DisplayColorKind.Normal, c1), new(DisplayColorKind.Normal, c2) }
				),
				c1Digit,
				c2Digit,
				c1,
				c2,
				chuteIndex
			);
			if (onlyFindOne)
			{
				return step;
			}

			accumulator.Add(step);
		}

		return null;
	}
}
