namespace Sudoku.Recognition.Behaviors;

/// <summary>
/// Read from two <see cref="Grid"/> instances, and analyzes difference of candidates between those two.
/// </summary>
/// <seealso cref="Grid"/>
public static class CandidateDelta
{
	/// <summary>
	/// Try to get difference of candidates between two grids.
	/// </summary>
	/// <param name="previous">The previous grid to be checked.</param>
	/// <param name="current">The current grid to be checked.</param>
	/// <param name="deltaCandidates">The difference candidates.</param>
	/// <param name="deltaKind">The difference kind.</param>
	/// <returns>A <see cref="bool"/> result indicating whether the operation is successful.</returns>
	public static bool TryGetDelta(
		scoped ref readonly Grid previous,
		scoped ref readonly Grid current,
		out CandidateMap deltaCandidates,
		out DeltaOperationKind deltaKind
	)
	{
		if (previous == current)
		{
			// Same grid. Just return with message "Nothing change".
			deltaCandidates = [];
			deltaKind = DeltaOperationKind.None;
			return true;
		}

		// Check the difference of values between two grids.
		// If they hold only one different value cell, it must be an assignment;
		// otherwise, an appending operation or elimination.
		var assignment = -1;
		var isReplacement = false;
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

						// Special state: candidate replacing.
						case 0:
						{
#if false
							// If here, we can know that two state of candidates are strictly different, e.g. [2, 4] and [3, 5]
							// because the previous branch has checked the equality of candidates: 'previous[cell] == current[cell]'.
							// Therefore, invalid. Just return false.
							goto ReturnNull;
#else
							continue;
#endif
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
				case (CellState.Modifiable, CellState.Modifiable):
				{
					// A replacement.
					var previousSetDigit = previous.GetDigit(cell);
					var currentSetDigit = current.GetDigit(cell);
					assignment = cell * 9 + currentSetDigit;
					isReplacement = true;
					break;
				}
				case (CellState.Empty, CellState.Modifiable):
				{
					// An assignment.
					var setDigit = current.GetDigit(cell);
					if (assignment != -1)
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

		// Special handling.
		if (eliminations.Cells == current.EmptyCells && !!current.EmptyCells)
		{
			deltaCandidates = eliminations;
			deltaKind = DeltaOperationKind.InitialPencilmarking;
			return true;
		}

		deltaKind = isReplacement
			? DeltaOperationKind.Replacement
			: assignment != -1
				? DeltaOperationKind.Assignment
				: appending
					? DeltaOperationKind.Appending
					: eliminations ? DeltaOperationKind.Elimination : DeltaOperationKind.None;
		deltaCandidates = deltaKind switch
		{
			DeltaOperationKind.None => [],
			DeltaOperationKind.Assignment or DeltaOperationKind.Replacement => [assignment],
			DeltaOperationKind.Appending => appending,
			DeltaOperationKind.Elimination => eliminations
		};
		return deltaKind != DeltaOperationKind.None;

	ReturnNull:
		deltaCandidates = [];
		deltaKind = default;
		return false;
	}
}
