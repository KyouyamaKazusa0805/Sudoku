using Sudoku.Concepts;
using Sudoku.Linq;
using static System.Numerics.BitOperations;
using static Sudoku.SolutionWideReadOnlyFields;

namespace Sudoku.Operation;

/// <summary>
/// Read from two <see cref="Grid"/> instances, and analyzes difference of candidates between those two.
/// </summary>
/// <seealso cref="Grid"/>
public static class CandidateDifference
{
	/// <summary>
	/// Try to get difference of candidates between two grids.
	/// </summary>
	/// <param name="previous">The previous grid to be checked.</param>
	/// <param name="current">The current grid to be checked.</param>
	/// <param name="differentCandidates">The difference candidates.</param>
	/// <param name="differenceKind">The difference kind.</param>
	/// <returns>A <see cref="bool"/> result indicating whether the operation is successful.</returns>
	public static bool TryGetDifference(
		scoped ref readonly Grid previous,
		scoped ref readonly Grid current,
		out CandidateMap differentCandidates,
		out OperationKind differenceKind
	)
	{
		if (previous == current)
		{
			// Same grid. Just return with message "Nothing change".
			differentCandidates = [];
			differenceKind = OperationKind.None;
			return true;
		}

		// Check the difference of values between two grids.
		// If they hold only one different value cell, it must be an assignment;
		// otherwise, an appending operation or elimination.
		var assignment = -1;
		var eliminations = CandidateMap.Empty;
		var appending = CandidateMap.Empty;
		for (var cell = 0; cell < 81; cell++)
		{
			switch (previous.GetState(cell), current.GetState(cell))
			{
				case var _ when previous[cell] == current[cell]:
				{
					continue;
				}
				case (CellState.Empty, CellState.Empty):
				{
					// Eliminations may exist here.
					var left = previous.GetCandidates(cell);
					var right = current.GetCandidates(cell);
					switch (Math.Sign(PopCount((uint)left) - PopCount((uint)right)))
					{
						// Possible elimination.
						case 1 when (left & right) == right:
						{
							foreach (var candidate in from digit in (Mask)(left & ~right) select cell * 9 + digit)
							{
								eliminations.Add(candidate);
							}
							continue;
						}

						// If here, we can know that two state of candidates are strictly different, e.g. [2, 4] and [3, 5]
						// because the previous branch has checked the equality of candidates: 'previous[cell] == current[cell]'.
						// Therefore, invalid. Just return false.
						case 0:
						{
							goto ReturnNull;
						}

						// Possible appending.
						case -1 when (right & left) == left:
						{
							foreach (var candidate in from digit in (Mask)(right & ~left) select cell * 9 + digit)
							{
								appending.Add(candidate);
							}
							continue;
						}
					}
					break;
				}
				case (CellState.Empty, CellState.Modifiable):
				{
					// An assignment.
					// We should check it carefully because an assignment may also contain extra removal:
					// If a conclusion is an assignment, we will also remove the candidates of the same digit from its peer cells.
					var setDigit = current.GetDigit(cell);
					if ((previous.GetCandidates(cell) >> setDigit & 1) == 0
						|| !!eliminations && (
							eliminations.Digits != (Mask)(1 << setDigit)
							|| eliminations.Cells is var eliminatedCells && (PeersMap[cell] & eliminatedCells) != eliminatedCells
						))
					{
						goto ReturnNull;
					}

					assignment = cell * 9 + setDigit;
					break;
				}
				default:
				{
					// Invalid.
					goto ReturnNull;
				}
			}
		}

		differenceKind = assignment != -1
			? OperationKind.Assignment
			: appending
				? OperationKind.Appending
				: eliminations
					? OperationKind.Elimination
					: OperationKind.None;
		differentCandidates = differenceKind switch
		{
			OperationKind.None => [],
			OperationKind.Assignment => [assignment],
			OperationKind.Appending => appending,
			OperationKind.Elimination => eliminations
		};
		return differenceKind != OperationKind.None;

	ReturnNull:
		differentCandidates = [];
		differenceKind = default;
		return false;
	}
}
