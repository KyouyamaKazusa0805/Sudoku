using System.Runtime.InteropServices;
using Sudoku.Analytics;
using Sudoku.Concepts;
using Sudoku.Linq;
using static Sudoku.Analytics.ConclusionType;

namespace Sudoku.Diff;

/// <summary>
/// Provides the method to operate with <see cref="Grid"/> instances, checking for the diff of two <see cref="Grid"/> instances.
/// </summary>
public static class GridDiff
{
	/// <summary>
	/// Try to check the technique that can make <paramref name="previous"/> to be changed into the state for <paramref name="current"/>.
	/// </summary>
	/// <param name="this">The step collector.</param>
	/// <param name="previous">The first sudoku grid puzzle to be checked. The value is at the previous state.</param>
	/// <param name="current">The second sudoku grid puzzle to be checked. The value is at the current state.</param>
	/// <param name="stepFilter">
	/// A filter method that checks whether a <see cref="Step"/> instance is valid. The value can be <see langword="null"/>
	/// if you don't want to make any extra checking.
	/// </param>
	/// <param name="steps">
	/// The found steps describing the changing making the <paramref name="previous"/> to be changed into <paramref name="current"/>.
	/// </param>
	/// <returns>
	/// A <see cref="bool"/> value indicating whether the operation is successful. Values are:
	/// <list type="table">
	/// <listheader>
	/// <term>Value</term>
	/// <description>Description</description>
	/// </listheader>
	/// <item>
	/// <term><see langword="true"/></term>
	/// <description>The diff change is found.</description>
	/// </item>
	/// <item>
	/// <term><see langword="false"/></term>
	/// <description>The diff change is not found, but two grids are valid to be checked.</description>
	/// </item>
	/// <item>
	/// <term><see langword="null"/></term>
	/// <description>
	/// Arguments <paramref name="previous"/> and <paramref name="current"/> are invalid that makes the operation failed to be checked.
	/// </description>
	/// </item>
	/// </list>
	/// </returns>
	public static bool TryGetDiffTechnique(
		this StepCollector @this,
		scoped ref readonly Grid previous,
		scoped ref readonly Grid current,
		StepFilter? stepFilter,
		out ReadOnlySpan<Step> steps
	)
	{
		if (!previous.IsValid || !current.IsValid)
		{
			goto ReturnNull;
		}

		// If the previous grid contains any missing candidates, we should reset candidate status and use it to check for steps.
		var temp = previous is { ContainsAnyMissingCandidates: true, ResetCandidatesGrid: var p } ? p : previous;

		// Check the validity of the current grid. A valid grid can only produce changes:
		//
		//     1) An extra assignment
		//     2) Some disappeared candidate
		//
		// Otherwise, the grid is an invalid grid pattern.
		var assignment = default(Conclusion?);
		var eliminations = new List<Conclusion>();
		for (var cell = 0; cell < 81; cell++)
		{
			switch (temp.GetState(cell), current.GetState(cell))
			{
				case var _ when temp[cell] == current[cell]:
				{
					continue;
				}
				case (CellState.Empty, CellState.Empty):
				{
					// Eliminations may exist here.
					var left = temp.GetCandidates(cell);
					var right = current.GetCandidates(cell);
					if ((left & right) != right || assignment is not null)
					{
						goto ReturnNull;
					}

					eliminations.AddRange(from digit in (Mask)(left & ~right) select new Conclusion(Elimination, cell, digit));
					break;
				}
				case (CellState.Empty, CellState.Modifiable):
				{
					// An assignment.
					var setDigit = current.GetDigit(cell);
					if ((temp.GetCandidates(cell) >> setDigit & 1) == 0 || eliminations.Count != 0)
					{
						goto ReturnNull;
					}

					assignment = new(Assignment, cell, setDigit);
					break;
				}
				default:
				{
					// Invalid.
					goto ReturnNull;
				}
			}
		}

		// Merge conclusion to be matched.
		var conclusions = (ConclusionBag)([.. (ReadOnlySpan<Conclusion>)(assignment is { } c ? [c] : []), .. eliminations]);
		var resultSteps = new List<Step>();
		foreach (var s in @this.Collect(in temp)!)
		{
			if (([.. s.Conclusions] & conclusions) == conclusions && (stepFilter?.Invoke(s) ?? true))
			{
				// Check whether the conclusion has already been deleted.
				var anyConclusionsExist = false;
				foreach (var conclusion in conclusions)
				{
					if (previous.Exists(conclusion.Candidate) is true)
					{
						anyConclusionsExist = true;
						break;
					}
				}
				if (anyConclusionsExist)
				{
					resultSteps.Add(s);
				}
			}
		}

		steps = CollectionsMarshal.AsSpan(resultSteps);
		return true;

	ReturnNull:
		steps = [];
		return false;
	}
}
