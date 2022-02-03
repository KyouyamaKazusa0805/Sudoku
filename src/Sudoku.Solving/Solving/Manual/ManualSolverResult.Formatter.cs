using Sudoku.Resources;
using Sudoku.Solving.Manual.Steps;
using static System.Math;

namespace Sudoku.Solving.Manual;

partial record ManualSolverResult
{
	/// <summary>
	/// Provides operations for analysis result formatting.
	/// </summary>
	/// <param name="Result">Indicates the analysis result.</param>
	private readonly record struct Formatter(ManualSolverResult Result)
	{
		/// <inheritdoc/>
		public override string ToString() => ToString(null);

		/// <summary>
		/// Returns a string that represents the current object with the specified format string.
		/// </summary>
		/// <param name="format">
		/// The format. If available, the parameter can be <see langword="null"/>.
		/// </param>
		/// <returns>The string result.</returns>
		/// <exception cref="FormatException">
		/// Throws when the specified format contains other invalid characters
		/// and the format provider can't work.
		/// </exception>
		public string ToString(string? format)
		{
			format ??= ".-!l";
			if (format.IsMatch(@"[^\^\-\.\?#@!abdl]"))
			{
				throw new FormatException("The specified format is invalid due to with invalid characters.");
			}

			string formatLower = format.ToLower();
			var options = SolverResultFormattingOptions.None;
			options |= c(in formatLower, '-') ? SolverResultFormattingOptions.ShowSeparators : 0;
			options |= c(in formatLower, '#') ? SolverResultFormattingOptions.ShowStepLabel : 0;
			options |= c(in formatLower, '@') ? SolverResultFormattingOptions.ShowSimple : 0;
			options |= c(in formatLower, '?') ? SolverResultFormattingOptions.ShowBottleneck : 0;
			options |= c(in formatLower, '!') ? SolverResultFormattingOptions.ShowDifficulty : 0;
			options |= c(in formatLower, '.') ? SolverResultFormattingOptions.ShowStepsAfterBottleneck : 0;
			options |= c(in formatLower, 'd') ? SolverResultFormattingOptions.ShowStepDetail : 0;
			options |= c(in formatLower, 'l') ? SolverResultFormattingOptions.ShowSteps : 0;

			return ToString(options);


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static bool c(in string formatLower, char c) => formatLower.Contains(c);
		}

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
			sb.Append(ResourceManager.Shared["AnalysisResultPuzzle"]);
			sb.Append(puzzle.ToString("#"));
			sb.AppendLine();

			// Print solving steps (if worth).
			if (options.Flags(SolverResultFormattingOptions.ShowSteps) && steps.Length != 0)
			{
				sb.Append(ResourceManager.Shared["AnalysisResultSolvingSteps"]);
				sb.AppendLine();

				if (getBottleneck(this) is var (bIndex, bInfo))
				{
					for (int i = 0, count = steps.Length; i < count; i++)
					{
						if (i > bIndex && options.Flags(SolverResultFormattingOptions.ShowStepsAfterBottleneck))
						{
							sb.Append(ResourceManager.Shared["Ellipsis"]);
							sb.AppendLine();

							break;
						}

						var info = steps[i];
						string infoStr = options.Flags(SolverResultFormattingOptions.ShowSimple)
							? info.ToSimpleString()
							: info.Formatize();
						bool showDiff = options.Flags(SolverResultFormattingOptions.ShowDifficulty)
							&& info.ShowDifficulty;

						string d = $"({info.Difficulty,5:0.0}";
						string s = $"{i + 1,4}";
						string labelInfo = (
							ShowStepLabel: options.Flags(SolverResultFormattingOptions.ShowStepLabel),
							ShowDiff: showDiff
						) switch
						{
							(ShowStepLabel: true, ShowDiff: true) => $"{s}, {d}) ",
							(ShowStepLabel: true, ShowDiff: false) => $"{s} ",
							(ShowStepLabel: false, ShowDiff: true) => $"{d}) ",
							_ => string.Empty,
						};

						sb.Append(labelInfo);
						sb.Append(infoStr);
						sb.AppendLine();
					}

					if (options.Flags(SolverResultFormattingOptions.ShowBottleneck))
					{
						a(ref sb, options.Flags(SolverResultFormattingOptions.ShowSeparators));

						sb.Append(ResourceManager.Shared["AnalysisResultBottleneckStep"]);

						if (options.Flags(SolverResultFormattingOptions.ShowStepLabel))
						{
							sb.Append(ResourceManager.Shared["AnalysisResultInStep"]);
							sb.Append(bIndex + 1);
							sb.Append(ResourceManager.Shared["Colon"]);
						}

						sb.Append(' ');
						sb.Append(bInfo);
						sb.AppendLine();
					}

					a(ref sb, options.Flags(SolverResultFormattingOptions.ShowSeparators));
				}
			}

			// Print solving step statistics (if worth).
			if (!steps.IsDefault)
			{
				sb.Append(ResourceManager.Shared["AnalysisResultTechniqueUsed"]);
				sb.AppendLine();

				if (options.Flags(SolverResultFormattingOptions.ShowStepDetail))
				{
					sb.Append(ResourceManager.Shared["AnalysisResultMin"], 6);
					sb.Append(',');
					sb.Append(' ');
					sb.Append(ResourceManager.Shared["AnalysisResultTotal"], 6);
					sb.Append(ResourceManager.Shared["AnalysisResultTechniqueUsing"]);
				}

				foreach (var solvingStepsGroup in from s in steps orderby s.Difficulty group s by s.Name)
				{
					if (options.Flags(SolverResultFormattingOptions.ShowStepDetail))
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

				if (options.Flags(SolverResultFormattingOptions.ShowStepDetail))
				{
					sb.Append("  (---");
					sb.Append(total, 8);
					sb.Append(')');
					sb.Append(' ');
				}

				sb.Append(stepsCount, 3);
				sb.Append(' ');
				sb.Append(ResourceManager.Shared[stepsCount == 1 ? "AnalysisResultStepSingular" : "AnalysisResultStepPlural"]);
				sb.AppendLine();

				a(ref sb, options.Flags(SolverResultFormattingOptions.ShowSeparators));
			}

			// Print detail data.
			sb.Append(ResourceManager.Shared["AnalysisResultPuzzleRating"]);
			sb.Append(max, "0.0");
			sb.Append('/');
			sb.Append(pearl, "0.0");
			sb.Append('/');
			sb.Append(diamond, "0.0");
			sb.AppendLine();

			// Print the solution (if not null).
			if (!solution.IsUndefined)
			{
				sb.Append(ResourceManager.Shared["AnalysisResultPuzzleSolution"]);
				sb.Append(solution.ToString("!"));
			}

			// Print the elapsed time.
			sb.Append(ResourceManager.Shared["AnalysisResultPuzzleHas"]);
			sb.AppendWhen(isSolved, ResourceManager.Shared["AnalysisResultNot"]);
			sb.Append(ResourceManager.Shared["AnalysisResultBeenSolved"]);
			sb.AppendLine();
			sb.Append(ResourceManager.Shared["AnalysisResultTimeElapsed"]);
			sb.Append($@"{elapsed:hh\:mm\:ss\.fff}");
			sb.AppendLine();

			a(ref sb, options.Flags(SolverResultFormattingOptions.ShowSeparators));

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
