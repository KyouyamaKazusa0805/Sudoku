using System.Runtime.InteropServices;
using Sudoku.Analytics;
using Sudoku.Concepts;
using static Sudoku.Analytics.ConclusionType;

namespace Sudoku.Operation;

/// <summary>
/// Represents a type that stores a list of methods that can recognize users' operation.
/// </summary>
public static class OperationRecognition
{
	/// <summary>
	/// Try to check the technique that can make <paramref name="previous"/> to be changed into the state for <paramref name="current"/>.
	/// </summary>
	/// <param name="stepRangeCollector">The step collector.</param>
	/// <param name="previous">The first sudoku grid puzzle to be checked. The value is at the previous state.</param>
	/// <param name="current">The second sudoku grid puzzle to be checked. The value is at the current state.</param>
	/// <param name="steps">
	/// The found steps describing the changing making the <paramref name="previous"/> to be changed into <paramref name="current"/>.
	/// </param>
	/// <returns>A <see cref="bool"/> value indicating whether the operation is successful.</returns>
	public static bool TryGetDiffTechnique(
		scoped ref readonly Grid previous,
		scoped ref readonly Grid current,
		StepCollector stepRangeCollector,
		out ReadOnlySpan<Step> steps
	)
	{
		if (!CandidateDifference.TryGetDifference(in previous, in current, out var differentCandidates, out var differenceKind)
			|| differenceKind is not (OperationKind.Assignment or OperationKind.Elimination))
		{
			goto ReturnFalse;
		}

		var conclusions = new ConclusionBag(
			from candidate in differentCandidates
			select new Conclusion(differenceKind == OperationKind.Assignment ? Assignment : Elimination, candidate)
		);

		// Merge conclusion to be matched.
		var resultSteps = new List<Step>();
		foreach (var s in stepRangeCollector.Collect(in previous)!)
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

		if (resultSteps.Count == 0)
		{
			goto ReturnFalse;
		}

		steps = CollectionsMarshal.AsSpan(resultSteps);
		return true;

	ReturnFalse:
		steps = [];
		return false;
	}
}
