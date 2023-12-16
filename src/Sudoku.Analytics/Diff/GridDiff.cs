using System.Runtime.InteropServices;
using Sudoku.Analytics;
using Sudoku.Concepts;

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
	/// <param name="steps">
	/// The found steps describing the changing making the <paramref name="previous"/> to be changed into <paramref name="current"/>.
	/// </param>
	/// <returns>A <see cref="bool"/> value indicating whether the operation is successful.</returns>
	public static bool TryGetDiffTechnique(
		scoped ref readonly Grid previous,
		scoped ref readonly Grid current,
		StepCollector @this,
		out ReadOnlySpan<Step> steps
	)
	{
		if (previous - current is not { } conclusions)
		{
			goto ReturnNull;
		}

		// Merge conclusion to be matched.
		var resultSteps = new List<Step>();
		foreach (var s in @this.Collect(in previous)!)
		{
			if (([.. s.Conclusions] & conclusions) == conclusions)
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
