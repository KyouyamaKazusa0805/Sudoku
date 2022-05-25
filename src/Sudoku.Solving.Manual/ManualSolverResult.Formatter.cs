using static Sudoku.Solving.Manual.SolverResultFormattingOptions;

namespace Sudoku.Solving.Manual;

partial record class ManualSolverResult
{
	/// <summary>
	/// Provides operations for analysis result formatting.
	/// </summary>
	/// <param name="Result">Indicates the analysis result.</param>
	private readonly record struct Formatter(ManualSolverResult Result)
	{
		/// <inheritdoc/>
		public override string ToString()
			=> ToString(ShowStepsAfterBottleneck | ShowSeparators | ShowDifficulty | ShowSteps);

		/// <summary>
		/// Get the string result with the specified formatting options.
		/// </summary>
		/// <param name="options">The options.</param>
		/// <returns>The result.</returns>
		public string ToString(SolverResultFormattingOptions options)
		{
			// Get all information.
			if (
				Result is not
				{
					IsSolved: var isSolved,
					TotalDifficulty: var total,
					MaxDifficulty: var max,
					PearlDifficulty: var pearl,
					DiamondDifficulty: var diamond,
					OriginalPuzzle: var puzzle,
					Solution: var solution,
					ElapsedTime: var elapsed,
					SolvingStepsCount: var stepsCount,
					Steps: var steps
				}
			)
			{
				throw new();
			}

			// Print header.
			var sb = new StringHandler();
			sb.Append(R["AnalysisResultPuzzle"]!);
			sb.Append($"{puzzle:#}");
			sb.AppendLine();

			// Print solving steps (if worth).
			if (options.Flags(ShowSteps) && !steps.IsDefaultOrEmpty)
			{
				sb.Append(R["AnalysisResultSolvingSteps"]!);
				sb.AppendLine();

				if (getBottleneck(this) is var (bIndex, bInfo))
				{
					for (int i = 0, count = steps.Length; i < count; i++)
					{
						if (i > bIndex && options.Flags(ShowStepsAfterBottleneck))
						{
							sb.Append(R.EmitPunctuation(Punctuation.Ellipsis));
							sb.AppendLine();

							break;
						}

						var info = steps[i];
						string infoStr = options.Flags(ShowSimple)
							? info.ToSimpleString()
							: info.Formatize();
						bool showDiff = options.Flags(ShowDifficulty)
							&& info.ShowDifficulty;

						string d = $"({info.Difficulty,5:0.0}";
						string s = $"{i + 1,4}";
						string labelInfo = (options.Flags(ShowStepLabel), showDiff) switch
						{
							(true, true) => $"{s}, {d}) ",
							(true, false) => $"{s} ",
							(false, true) => $"{d}) ",
							_ => string.Empty,
						};

						sb.Append(labelInfo);
						sb.Append(infoStr);
						sb.AppendLine();
					}

					if (options.Flags(ShowBottleneck))
					{
						a(ref sb, options.Flags(ShowSeparators));

						sb.Append(R["AnalysisResultBottleneckStep"]!);

						if (options.Flags(ShowStepLabel))
						{
							sb.Append(R["AnalysisResultInStep"]!);
							sb.Append(bIndex + 1);
							sb.Append(R.EmitPunctuation(Punctuation.Colon));
						}

						sb.Append(' ');
						sb.Append(bInfo);
						sb.AppendLine();
					}

					a(ref sb, options.Flags(ShowSeparators));
				}
			}

			// Print solving step statistics (if worth).
			if (!steps.IsDefault)
			{
				sb.Append(R["AnalysisResultTechniqueUsed"]!);
				sb.AppendLine();

				if (options.Flags(ShowStepDetail))
				{
					sb.Append(R["AnalysisResultMin"]!, 6);
					sb.Append(',');
					sb.Append(' ');
					sb.Append(R["AnalysisResultTotal"]!, 6);
					sb.Append(R["AnalysisResultTechniqueUsing"]!);
				}

				foreach (var solvingStepsGroup in from s in steps orderby s.Difficulty group s by s.Name)
				{
					if (options.Flags(ShowStepDetail))
					{
						decimal currentTotal = 0, currentMinimum = decimal.MaxValue;
						foreach (var solvingStep in solvingStepsGroup)
						{
							decimal difficulty = solvingStep.Difficulty;
							currentTotal += difficulty;
							currentMinimum = Min(currentMinimum, difficulty);
						}

						sb.Append(currentMinimum, 6, "0.0");
						sb.Append(',');
						sb.Append(' ');
						sb.Append(currentTotal, 6, "0.0");
						sb.Append(')');
						sb.Append(' ');
					}

					sb.Append(solvingStepsGroup.Count(), 3);
					sb.Append(" * ");
					sb.Append(solvingStepsGroup.Key);
					sb.AppendLine();
				}

				if (options.Flags(ShowStepDetail))
				{
					sb.Append("  (---");
					sb.Append(total, 8);
					sb.Append(')');
					sb.Append(' ');
				}

				sb.Append(stepsCount, 3);
				sb.Append(' ');
				sb.Append(R[stepsCount == 1 ? "AnalysisResultStepSingular" : "AnalysisResultStepPlural"]!);
				sb.AppendLine();

				a(ref sb, options.Flags(ShowSeparators));
			}

			// Print detail data.
			sb.Append(R["AnalysisResultPuzzleRating"]!);
			sb.Append(max, "0.0");
			sb.Append('/');
			sb.Append(pearl, "0.0");
			sb.Append('/');
			sb.Append(diamond, "0.0");
			sb.AppendLine();

			// Print the solution (if not null).
			if (!solution.IsUndefined)
			{
				sb.Append(R["AnalysisResultPuzzleSolution"]!);
				sb.Append($"{solution:!}");
			}

			// Print the elapsed time.
			sb.Append(R["AnalysisResultPuzzleHas"]!);
			sb.AppendWhen(!isSolved, R["AnalysisResultNot"]!);
			sb.Append(R["AnalysisResultBeenSolved"]!);
			sb.AppendLine();
			sb.Append(R["AnalysisResultTimeElapsed"]!);
			sb.Append($@"{elapsed:hh\:mm\:ss\.fff}");
			sb.AppendLine();

			a(ref sb, options.Flags(ShowSeparators));

			return sb.ToStringAndClear();


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static void a(ref StringHandler sb, bool showSeparator)
			{
				if (showSeparator)
				{
					sb.Append('-', 10);
					sb.AppendLine();
				}
			}

			static (int, Step)? getBottleneck(in Formatter @this)
			{
				if (@this.Result is not { IsSolved: true, Steps: var steps, SolvingStepsCount: var stepsCount })
				{
					return null;
				}

				for (int i = stepsCount - 1; i >= 0; i--)
				{
					if (steps[i] is not SingleStep and { ShowDifficulty: true } step)
					{
						return (i, step);
					}
				}

				// If code goes to here, all steps are more difficult than single techniques.
				// Get the first one is okay.
				return (0, steps[0]);
			}
		}
	}
}
