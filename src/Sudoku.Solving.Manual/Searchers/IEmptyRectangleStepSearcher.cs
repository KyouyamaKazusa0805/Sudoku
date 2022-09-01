namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Provides with an <b>Empty Rectangle</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Empty Rectangle</item>
/// </list>
/// </summary>
public interface IEmptyRectangleStepSearcher : ISingleDigitPatternStepSearcher
{
	/// <summary>
	/// Determine whether the specified cells in the specified block form an empty rectangle.
	/// </summary>
	/// <param name="cells">The cells to be checked.</param>
	/// <param name="block">The block where the cells may form an empty rectangle structure.</param>
	/// <param name="row">The row that the empty rectangle used.</param>
	/// <param name="column">The column that the empty rectangle used.</param>
	/// <returns>
	/// A <see cref="bool"/> value indicating that. If <see langword="true"/>,
	/// both arguments <paramref name="row"/> and <paramref name="column"/> can be used;
	/// otherwise, both arguments should be discards.
	/// </returns>
	protected internal static sealed bool IsEmptyRectangle(scoped in CellMap cells, int block, out int row, out int column)
	{
		int r = block / 3 * 3 + 9, c = block % 3 * 3 + 18;
		for (int i = r, count = 0, rPlus3 = r + 3; i < rPlus3; i++)
		{
			if ((cells & HousesMap[i]) is not [] || ++count <= 1)
			{
				continue;
			}

			row = column = -1;
			return false;
		}

		for (int i = c, count = 0, cPlus3 = c + 3; i < cPlus3; i++)
		{
			if ((cells & HousesMap[i]) is not [] || ++count <= 1)
			{
				continue;
			}

			row = column = -1;
			return false;
		}

		for (int i = r, rPlus3 = r + 3; i < rPlus3; i++)
		{
			for (int j = c, cPlus3 = c + 3; j < cPlus3; j++)
			{
				if (cells >> (HousesMap[i] | HousesMap[j]))
				{
					continue;
				}

				row = i;
				column = j;
				return true;
			}
		}

		row = column = -1;
		return false;
	}
}

[StepSearcher]
internal sealed unsafe partial class EmptyRectangleStepSearcher : IEmptyRectangleStepSearcher
{
	/// <inheritdoc/>
	public IStep? GetAll(ICollection<IStep> accumulator, scoped in Grid grid, bool onlyFindOne)
	{
		for (int digit = 0; digit < 9; digit++)
		{
			for (int block = 0; block < 9; block++)
			{
				// Check the empty rectangle occupies more than 2 cells.
				// and the structure forms an empty rectangle.
				var erMap = CandidatesMap[digit] & HousesMap[block];
				if (erMap.Count < 2
					|| !IEmptyRectangleStepSearcher.IsEmptyRectangle(erMap, block, out int row, out int column))
				{
					continue;
				}

				// Search for conjugate pair.
				for (int i = 0; i < 12; i++)
				{
					var linkMap = CandidatesMap[digit] & HousesMap[EmptyRectangleLinkIds[block, i]];
					if (linkMap.Count != 2)
					{
						continue;
					}

					short blockMask = linkMap.BlockMask;
					if (IsPow2(blockMask)
						|| i < 6 && (linkMap & HousesMap[column]) is []
						|| i >= 6 && (linkMap & HousesMap[row]) is [])
					{
						continue;
					}

					int t = (linkMap - HousesMap[i < 6 ? column : row])[0];
					int elimHouse = i < 6 ? t % 9 + 18 : t / 9 + 9;
					var elimCellMap = CandidatesMap[digit] & HousesMap[elimHouse] & HousesMap[i < 6 ? row : column];
					if (elimCellMap is not [var elimCell, ..])
					{
						continue;
					}

					if (!CandidatesMap[digit].Contains(elimCell))
					{
						continue;
					}

					// Gather all highlight candidates.
					var candidateOffsets = new List<CandidateViewNode>();
					var cpCells = new List<int>(2);
					foreach (int cell in HousesMap[block] & CandidatesMap[digit])
					{
						candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + digit));
					}
					foreach (int cell in linkMap)
					{
						candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + digit));
						cpCells.Add(cell);
					}

					var step = new EmptyRectangleStep(
						ImmutableArray.Create(new Conclusion(Elimination, elimCell, digit)),
						ImmutableArray.Create(
							View.Empty
								| candidateOffsets
								| new HouseViewNode(DisplayColorKind.Normal, block)
						),
						digit,
						block,
						new(cpCells[0], cpCells[1], digit)
					);

					if (onlyFindOne)
					{
						return step;
					}

					accumulator.Add(step);
				}
			}
		}

		return null;
	}
}
