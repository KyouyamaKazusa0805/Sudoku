using static Sudoku.Solving.AnalysisResultFormattingOptions;
using CountryCode = Sudoku.Globalization.CountryCode;

namespace Sudoku.Solving;

partial record AnalysisResult
{
	/// <summary>
	/// Provides operations for analysis result formatting.
	/// </summary>
	/// <param name="Result">Indicates the analysis result.</param>
	private sealed record Formatter(AnalysisResult Result) : IFormattable
	{
		/// <inheritdoc/>
		public override string ToString() => ToString(null, null);

		/// <summary>
		/// Returns a string that represents the current object with the specified format string.
		/// </summary>
		/// <param name="format">
		/// The format. If available, the parameter can be <see langword="null"/>.
		/// </param>
		/// <returns>The string result.</returns>
		public string ToString(string format) => ToString(format, null);

		/// <inheritdoc/>
		public string ToString(string? format, IFormatProvider? formatProvider) =>
			ToString(format, formatProvider, CountryCode.EnUs);

		/// <summary>
		/// Get the string result.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <param name="formatProvider">The format provider.</param>
		/// <param name="countryCode">The country code.</param>
		/// <returns>The result.</returns>
		/// <exception cref="FormatException">
		/// Throws when the specified format contains other invalid characters
		/// and the format provider can't work.
		/// </exception>
		public string ToString(string? format, IFormatProvider? formatProvider, CountryCode countryCode)
		{
			if (formatProvider.HasFormatted(this, format, out string? result))
			{
				return result;
			}

			format ??= ".-!l";
			if (format.IsMatch(@"[^\^\-\.\?#@!abdl]"))
			{
				throw new FormatException("The specified format is invalid due to with invalid characters.");
			}

			string formatLower = format.ToLower();
			var options = None;
			options |= c(in formatLower, '-') ? ShowSeparators : None;
			options |= c(in formatLower, '#') ? ShowStepLabel : None;
			options |= c(in formatLower, '@') ? ShowSimple : None;
			options |= c(in formatLower, '?') ? ShowBottleneck : None;
			options |= c(in formatLower, '!') ? ShowDifficulty : None;
			options |= c(in formatLower, '.') ? ShowStepsAfterBottleneck : None;
			options |= c(in formatLower, 'a') ? ShowAttributes : None;
			options |= c(in formatLower, 'b') ? ShowBackdoors : None;
			options |= c(in formatLower, 'd') ? ShowStepDetail : None;
			options |= c(in formatLower, 'l') ? ShowSteps : None;

			return ToString(options, countryCode);


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static bool c(in string formatLower, char c) => formatLower.Contains(c);
		}

		/// <summary>
		/// Get the string result with the specified formatting options.
		/// </summary>
		/// <param name="options">The options.</param>
		/// <returns>The result.</returns>
		public string ToString(AnalysisResultFormattingOptions options) => ToString(options, CountryCode.EnUs);

		/// <summary>
		/// Get the string result with the specified formatting options and the country code.
		/// </summary>
		/// <param name="options">The options.</param>
		/// <param name="countryCode">The country code.</param>
		/// <returns>The result.</returns>
		public string ToString(AnalysisResultFormattingOptions options, CountryCode countryCode)
		{
			TextResources.Current.ChangeLanguage(countryCode);

			// Get all information.
			var (
				solverName, hasSolved, total, max, pearl, diamond,
				puzzle, solution, elapsed, stepsCount, steps, _, additional) = Result;

			// Print header.
			var sb = new StringHandler(initialCapacity: 300);
			sb.AppendFormatted((string)TextResources.Current.AnalysisResultPuzzle);
			sb.AppendGridFormatted(puzzle, "#");
			sb.AppendFormatted((string)TextResources.Current.AnalysisResultSolvingTool);
			sb.AppendFormatted(solverName);
			sb.AppendLine();

			// Print solving steps (if worth).
			if (options.Flags(ShowSteps) && steps is { Count: not 0 })
			{
				sb.AppendFormatted((string)TextResources.Current.AnalysisResultSolvingSteps);
				sb.AppendLine();

				if (getBottleneck() is var (bIndex, bInfo))
				{
					for (int i = 0, count = steps.Count; i < count; i++)
					{
						if (i > bIndex && options.Flags(ShowStepsAfterBottleneck))
						{
							sb.AppendFormatted((string)TextResources.Current.Ellipsis);
							sb.AppendLine();

							break;
						}

						var info = steps[i];
						string infoStr = options.Flags(ShowSimple) ? info.ToSimpleString() : info.Formatize();
						bool showDiff = options.Flags(ShowDifficulty) && info.ShowDifficulty;

						string d = $"({info.Difficulty,5:0.0}";
						string s = $"{i + 1,4}";
						string labelInfo = (ShowStepLabel: options.Flags(ShowStepLabel), ShowDiff: showDiff) switch
						{
							(ShowStepLabel: true, ShowDiff: true) => $"{s}, {d}) ",
							(ShowStepLabel: true, ShowDiff: false) => $"{s} ",
							(ShowStepLabel: false, ShowDiff: true) => $"{d}) ",
							_ => string.Empty,
						};

						sb.AppendFormatted(labelInfo);
						sb.AppendFormatted(infoStr);
						sb.AppendLine();
					}

					if (options.Flags(ShowBottleneck))
					{
						a(ref sb, options.Flags(ShowSeparators));

						sb.AppendFormatted((string)TextResources.Current.AnalysisResultBottleneckStep);

						if (options.Flags(ShowStepLabel))
						{
							sb.AppendFormatted((string)TextResources.Current.AnalysisResultInStep);
							sb.AppendFormatted(bIndex + 1);
							sb.AppendFormatted((string)TextResources.Current.Colon);
						}

						sb.Append(' ');
						sb.AppendFormatted(bInfo);
						sb.AppendLine();
					}

					a(ref sb, options.Flags(ShowSeparators));
				}
			}

			// Print solving step statistics (if worth).
			if (steps is not null)
			{
				var solvingStepsGrouped = new List<IGrouping<string, StepInfo>>(
					from step in steps orderby step.Difficulty group step by step.Name
				);
				sb.AppendFormatted((string)TextResources.Current.AnalysisResultTechniqueUsed);
				sb.AppendLine();

				if (options.Flags(ShowStepDetail))
				{
					sb.AppendFormatted((string)TextResources.Current.AnalysisResultMin, 6);
					sb.Append(',');
					sb.Append(' ');
					sb.AppendFormatted((string)TextResources.Current.AnalysisResultTotal, 6);
					sb.AppendFormatted((string)TextResources.Current.AnalysisResultTechniqueUsing);
					sb.AppendLine();
				}

				foreach (var solvingStepsGroup in solvingStepsGrouped)
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

						sb.AppendFormatted(currentMinimum, 6, "0.0");
						sb.Append(',');
						sb.Append(' ');
						sb.AppendFormatted(currentTotal, 6, "0.0");
						sb.Append(')');
						sb.Append(' ');
					}

					sb.AppendFormatted(solvingStepsGroup.Count(), 3);
					sb.Append(' ');
					sb.Append('*');
					sb.Append(' ');
					sb.AppendFormatted(solvingStepsGroup.Key);
					sb.AppendLine();
				}

				if (options.Flags(ShowStepDetail))
				{
					sb.AppendFormatted("  (---");
					sb.AppendFormatted(total, 8);
					sb.Append(')');
					sb.Append(' ');
				}

				sb.AppendFormatted(stepsCount, 3);
				sb.Append(' ');
				sb.AppendFormatted(
					stepsCount == 1
						? (string)TextResources.Current.AnalysisResultStepSingular
						: (string)TextResources.Current.AnalysisResultStepPlural
				);
				sb.AppendLine();


				a(ref sb, options.Flags(ShowSeparators));
			}

			// Print detail data.
			sb.AppendFormatted((string)TextResources.Current.AnalysisResultPuzzleRating);
			sb.AppendFormatted(max, "0.0");
			sb.Append('/');
			sb.AppendFormatted(pearl, "0.0");
			sb.Append('/');
			sb.AppendFormatted(diamond, "0.0");
			sb.AppendLine();

			// Print the solution (if not null).
			if (solution is { } solutionNotNull)
			{
				sb.AppendFormatted((string)TextResources.Current.AnalysisResultPuzzleSolution);
				sb.AppendGridFormatted(solutionNotNull, "!");
				sb.AppendLine();
			}

			// Print the elapsed time.
			sb.AppendFormatted((string)TextResources.Current.AnalysisResultPuzzleHas);
			if (hasSolved)
			{
				sb.AppendFormatted((string)TextResources.Current.AnalysisResultNot);
			}
			sb.AppendFormatted((string)TextResources.Current.AnalysisResultBeenSolved);
			sb.AppendLine();
			sb.AppendFormatted((string)TextResources.Current.AnalysisResultTimeElapsed);
			sb.AppendFormatted(elapsed, @"hh\:mm\:ss\.fff");

			a(ref sb, options.Flags(ShowSeparators));

			// Print attributes (if worth).
			// Here use dynamic call (reflection) to get all methods which contains
			// only one parameter and its type is 'Grid'.
			if (options.Flags(ShowAttributes))
			{
				sb.AppendFormatted((string)TextResources.Current.AnalysisResultAttributes);
				sb.AppendLine();

				foreach (var methodInfo in
					from methodInfo in typeof(PuzzleAttributeChecker).GetMethods()
					where methodInfo.ReturnType == typeof(bool)
					let @params = methodInfo.GetParameters()
					where @params.Length == 1 && @params[0].ParameterType == typeof(SudokuGrid)
					select methodInfo)
				{
					var grid = (SudokuGrid)methodInfo.Invoke(null, new object[] { puzzle })!;

					sb.Append(' ', 4);
					sb.AppendFormatted(methodInfo.Name);
					sb.Append(':');
					sb.Append(' ');
					sb.AppendGridFormatted(grid, "0");
					sb.AppendLine();
				}

				a(ref sb, options.Flags(ShowSeparators));
			}

			// Print backdoors (if worth).
			if (options.Flags(ShowBackdoors))
			{
				sb.AppendFormatted((string)TextResources.Current.AnalysisResultBackdoors);
				sb.AppendLine();

				var searcher = new BackdoorSearcher();
				foreach (var assignment in searcher.SearchForBackdoorsExact(puzzle, 0))
				{
					sb.Append(' ', 4);
					sb.AppendFormatted(assignment[0]);
					sb.AppendLine();
				}

				a(ref sb, options.Flags(ShowSeparators));
			}

			// Print the additional information (if worth).
			if (additional is not null)
			{
				sb.AppendFormatted(additional);
			}

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

			(int, StepInfo)? getBottleneck()
			{
				var (_, solvingStepsCount, solvingSteps) = Result;
				if (solvingSteps is null)
				{
					return null;
				}

				for (int i = solvingStepsCount - 1; i >= 0; i--)
				{
					if (solvingSteps[i] is not SingleStepInfo and { ShowDifficulty: true } step)
					{
						return (i, step);
					}
				}

				// If code goes to here, all steps are more difficult than single techniques.
				// Get the first one is okay.
				return (0, solvingSteps[0]);
			}
		}
	}
}
