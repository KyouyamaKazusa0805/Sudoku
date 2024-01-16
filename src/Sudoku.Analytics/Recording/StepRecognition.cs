namespace Sudoku.Recording;

/// <summary>
/// Represents a type that stores a list of methods that can recognize users' operation.
/// </summary>
public static class StepRecognition
{
	/// <summary>
	/// Try to check the technique that can make <paramref name="previous"/> to be changed into the state for <paramref name="current"/>.
	/// </summary>
	/// <param name="collector">The step collector that limits the techniques can be used and analyzed.</param>
	/// <param name="previous">The first sudoku grid puzzle to be checked. The value is at the previous state.</param>
	/// <param name="current">The second sudoku grid puzzle to be checked. The value is at the current state.</param>
	/// <param name="steps">
	/// The found steps describing the changing making the <paramref name="previous"/> to be changed into <paramref name="current"/>.
	/// </param>
	/// <returns>A <see cref="bool"/> value indicating whether the operation is successful.</returns>
	/// <exception cref="InvalidOperationException">Throws when the previous or current grid is invalid.</exception>
	public static bool TryGetDiffTechnique(
		StepCollector collector,
		scoped ref readonly Grid previous,
		scoped ref readonly Grid current,
		out ReadOnlySpan<Step> steps
	)
	{
		if (!previous.IsValid || !current.IsValid)
		{
			throw new InvalidOperationException("Cannot analyze for grids with wrong candidates or values.");
		}

		if (!CandidateDelta.TryGetDelta(in previous, in current, out var differentCandidates, out var differenceKind)
			|| differenceKind is not (DeltaOperationKind.Assignment or DeltaOperationKind.Replacement or DeltaOperationKind.Elimination))
		{
			goto ReturnFalse;
		}

		var conclusions = new ConclusionSet(
			from candidate in differentCandidates
			select new Conclusion(differenceKind is DeltaOperationKind.Assignment or DeltaOperationKind.Replacement ? Assignment : Elimination, candidate)
		);

		// Merge conclusion to be matched.
		var resultSteps = new List<Step>();
		foreach (var s in collector.Collect(in previous))
		{
			if ((s.Conclusions.AsConclusionSet() & conclusions) == conclusions)
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

		steps = resultSteps.AsSpan();
		return true;

	ReturnFalse:
		steps = [];
		return false;
	}
}
