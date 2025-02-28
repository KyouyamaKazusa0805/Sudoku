namespace Sudoku.Behaviors.Diff;

/// <summary>
/// Provides a way to analyze technique usages on difference between two grids.
/// </summary>
public static class DiffTechniqueAnalysis
{
	/// <summary>
	/// Try to analyze technique used that causes <paramref name="left"/> changing into <paramref name="right"/>.
	/// </summary>
	/// <param name="left">The first grid to be checked.</param>
	/// <param name="right">The second grid to be checked.</param>
	/// <param name="collector">The collector instance that is used for collecting found steps.</param>
	/// <param name="step">The step.</param>
	/// <returns>A <see cref="bool"/> result indicating whether such step can be inferred.</returns>
	public static bool TryAnalyzeTechnique(
		ref readonly Grid left,
		ref readonly Grid right,
		Collector collector,
		[NotNullWhen(true)] out Step? step
	)
	{
		if (!DiffAnalysis.TryAnalyzeDiff(left, right, out var result)
			|| result.Type is not (DiffType.AddModifiable or DiffType.RemoveCandidate)
			|| left.GetUniqueness() == Uniqueness.Bad || right.GetUniqueness() == Uniqueness.Bad)
		{
			step = null;
			return false;
		}

		var foundSteps = collector.Collect(left);
		switch (result)
		{
			case AddModifiableDiffResult { Candidates: [var assignment] }:
			{
				foreach (var s in foundSteps)
				{
					if (s.Conclusions.AsSet().Contains(new(Assignment, assignment)))
					{
						step = s;
						return true;
					}
				}
				break;
			}
			case RemoveCandidateDiffResult { Candidates: var eliminations }:
			{
				foreach (var s in foundSteps)
				{
					var possibleEliminations = (
						from conclusion in s.Conclusions
						where conclusion.ConclusionType == Elimination
						select conclusion.Candidate
					).Span.AsCandidateMap();
					if ((possibleEliminations & eliminations) == eliminations)
					{
						step = s;
						return true;
					}
				}
				break;
			}
		}

		step = null;
		return false;
	}
}
