using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Sudoku.Data;
using Sudoku.Solving.Checking;
using Sudoku.Solving.Manual;
using Sudoku.Solving.Manual.Singles;
using Sudoku.Resources;
using static Sudoku.Solving.AnalysisResultFormattingOptions;
using CountryCode = Sudoku.Globalization.CountryCode;

namespace Sudoku.Solving
{
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
				var sb = new ValueStringBuilder(300);
				sb.Append((string)TextResources.Current.AnalysisResultPuzzle);
				sb.AppendLine(puzzle.ToString("#"));
				sb.Append((string)TextResources.Current.AnalysisResultSolvingTool);
				sb.AppendLine(solverName);

				// Print solving steps (if worth).
				if (options.Flags(ShowSteps) && steps is { Count: not 0 })
				{
					sb.AppendLine((string)TextResources.Current.AnalysisResultSolvingSteps);
					if (getBottleneck() is var (bIndex, bInfo))
					{
						for (int i = 0, count = steps.Count; i < count; i++)
						{
							if (i > bIndex && options.Flags(ShowStepsAfterBottleneck))
							{
								sb.AppendLine((string)TextResources.Current.Ellipsis);
								break;
							}

							var info = steps[i];
							string infoStr = options.Flags(ShowSimple) ? info.ToSimpleString() : info.Formatize();
							bool showDiff = options.Flags(ShowDifficulty) && info.ShowDifficulty;

							string d = $"({info.Difficulty.ToString("0.0")}".PadLeft(5);
							string s = (i + 1).ToString().PadLeft(4);
							string labelInfo = (ShowStepLabel: options.Flags(ShowStepLabel), ShowDiff: showDiff) switch
							{
								(ShowStepLabel: true, ShowDiff: true) => $"{s}, {d}) ",
								(ShowStepLabel: true, ShowDiff: false) => $"{s} ",
								(ShowStepLabel: false, ShowDiff: true) => $"{d}) ",
								_ => string.Empty,
							};

							sb.Append(labelInfo);
							sb.AppendLine(infoStr);
						}

						if (options.Flags(ShowBottleneck))
						{
							a(ref sb, options.Flags(ShowSeparators));

							sb.Append((string)TextResources.Current.AnalysisResultBottleneckStep);
							sb.Append(
								options.Flags(ShowStepLabel)
								? $@"{
									TextResources.Current.AnalysisResultInStep
								}{(bIndex + 1).ToString()}{TextResources.Current.Colon}"
								: string.Empty
							);
							sb.Append(' ');
							sb.AppendLine(bInfo);
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
					sb.AppendLine((string)TextResources.Current.AnalysisResultTechniqueUsed);
					if (options.Flags(ShowStepDetail))
					{
						sb.Append((string)TextResources.Current.AnalysisResultMin.PadLeft(6));
						sb.Append(',');
						sb.Append(' ');
						sb.Append((string)TextResources.Current.AnalysisResultTotal.PadLeft(6));
						sb.AppendLine((string)TextResources.Current.AnalysisResultTechniqueUsing);
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
								currentMinimum = Math.Min(currentMinimum, difficulty);
							}

							sb.Append($"({currentMinimum.ToString("0.0")}".PadLeft(6));
							sb.Append(',');
							sb.Append(' ');
							sb.Append(currentTotal.ToString("0.0").PadLeft(6));
							sb.Append(')');
							sb.Append(' ');
						}

						sb.Append(solvingStepsGroup.Count().ToString().PadLeft(3));
						sb.Append(' ');
						sb.Append('*');
						sb.Append(' ');
						sb.AppendLine(solvingStepsGroup.Key);
					}

					if (options.Flags(ShowStepDetail))
					{
						sb.Append("  (---");
						sb.Append(total.ToString().PadLeft(8));
						sb.Append(')');
						sb.Append(' ');
					}

					sb.Append(stepsCount.ToString().PadLeft(3));
					sb.Append(' ');
					sb.AppendLine(
						stepsCount == 1
						? (string)TextResources.Current.AnalysisResultStepSingular
						: (string)TextResources.Current.AnalysisResultStepPlural
					);

					a(ref sb, options.Flags(ShowSeparators));
				}

				// Print detail data.
				sb.Append((string)TextResources.Current.AnalysisResultPuzzleRating);
				sb.Append(max.ToString("0.0"));
				sb.Append('/');
				sb.Append(pearl.ToString("0.0"));
				sb.Append('/');
				sb.AppendLine(diamond.ToString("0.0"));

				// Print the solution (if not null).
				if (solution is { } solutionNotNull)
				{
					sb.Append((string)TextResources.Current.AnalysisResultPuzzleSolution);
					sb.AppendLine(solutionNotNull.ToString("!"));
				}

				// Print the elapsed time.
				sb.Append((string)TextResources.Current.AnalysisResultPuzzleHas);
				sb.Append(hasSolved ? string.Empty : (string)TextResources.Current.AnalysisResultNot);
				sb.AppendLine((string)TextResources.Current.AnalysisResultBeenSolved);
				sb.Append((string)TextResources.Current.AnalysisResultTimeElapsed);
				sb.AppendLine(elapsed.ToString("hh\\:mm\\:ss\\.fff"));

				a(ref sb, options.Flags(ShowSeparators));

				// Print attributes (if worth).
				// Here use dynamic call (reflection) to get all methods which contains
				// only one parameter and its type is 'Grid'.
				if (options.Flags(ShowAttributes))
				{
					sb.AppendLine((string)TextResources.Current.AnalysisResultAttributes);
					foreach (var methodInfo in
						from methodInfo in typeof(PuzzleAttributeChecker).GetMethods()
						where methodInfo.ReturnType == typeof(bool)
						let @params = methodInfo.GetParameters()
						where @params.Length == 1 && @params[0].ParameterType == typeof(SudokuGrid)
						select methodInfo)
					{
						var grid = (SudokuGrid)methodInfo.Invoke(null, new object[] { puzzle })!;

						sb.Append(new string(' ', 4));
						sb.Append(methodInfo.Name);
						sb.Append(':');
						sb.Append(' ');
						sb.AppendLine(grid.ToString("0"));
					}

					a(ref sb, options.Flags(ShowSeparators));
				}

				// Print backdoors (if worth).
				if (options.Flags(ShowBackdoors))
				{
					sb.AppendLine((string)TextResources.Current.AnalysisResultBackdoors);
					var searcher = new BackdoorSearcher();
					foreach (var assignment in searcher.SearchForBackdoorsExact(puzzle, 0))
					{
						sb.Append(new string(' ', 4));
						sb.AppendLine(assignment[0]);
					}

					a(ref sb, options.Flags(ShowSeparators));
				}

				// Print the additional information (if worth).
				if (additional is not null)
				{
					sb.AppendLine(additional);
				}

				return sb.ToString();


				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				static void a(ref ValueStringBuilder sb, bool showSeparator) =>
					sb.Append(showSeparator ? $"{new string('-', 10)}{Environment.NewLine}" : string.Empty);

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
}
