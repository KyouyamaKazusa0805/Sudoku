namespace Sudoku.Measuring;

/// <summary>
/// Represents a type that can calculates the measurable items on an <see cref="AnalyzerResult"/> instance.
/// </summary>
/// <param name="analyzerResult">The instance.</param>
/// <seealso cref="AnalyzerResult"/>
public sealed class AnalyzerResultMesurable(AnalyzerResult analyzerResult)
{
	/// <summary>
	/// Indicates the distance values for all steps produced.
	/// </summary>
	public double DistanceAll
	{
		get
		{
			if (!analyzerResult.IsSolved)
			{
				return 0;
			}

			var steps = analyzerResult.StepsSpan;
			if (steps.Length is 0 or 1)
			{
				return 0;
			}

			var result = 0D;
			for (var i = 0; i < steps.Length - 1; i++)
			{
				var previous = steps[i];
				var next = steps[i + 1];
				if ((previous, next) is not (SingleStep { Cell: var l }, SingleStep { Cell: var r }))
				{
					return 0;
				}

				result += Distance.GetDistance(l, r);
			}

			return result;
		}
	}
}
