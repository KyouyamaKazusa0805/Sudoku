using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sudoku.Data;
using Sudoku.DocComments;
using Sudoku.Extensions;
using Sudoku.Globalization;
using Sudoku.Solving.Checking;
using Sudoku.Solving.Manual.Singles;
using static Sudoku.Windows.Resources;

namespace Sudoku.Solving
{
	/// <summary>
	/// Provides operations for analysis result formatting.
	/// </summary>
	internal sealed class AnalysisResultFormatter : IFormattable
	{
		/// <summary>
		/// Initializes an instance with the specified analysis result and format.
		/// </summary>
		/// <param name="result">The analysis result.</param>
		public AnalysisResultFormatter(AnalysisResult result) => Result = result;


		/// <summary>
		/// Indicates the analysis result.
		/// </summary>
		public AnalysisResult Result { get; }


		/// <inheritdoc/>
		public override string ToString() => ToString(null, null);

		/// <inheritdoc cref="Formattable.ToString(string)"/>
		public string ToString(string format) => ToString(format, null);

		/// <inheritdoc/>
		public string ToString(string? format, IFormatProvider? formatProvider) =>
			ToString(format, formatProvider, Globalization.CountryCode.EnUs);

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
			_ = format.IsMatch(@"[^\^\-\.\?#@!abdl]") ? throw new FormatException("The specified format is invalid due to with invalid characters.") : 0;

			string formatLower = format.ToLower();
			static bool c(in string formatLower, char c) => formatLower.Contains(c);
			bool showSeparator = c(in formatLower, '-');
			bool showStepNum = c(in formatLower, '#');
			bool showSimple = c(in formatLower, '@');
			bool showBottleneck = c(in formatLower, '?');
			bool showDifficulty = c(in formatLower, '!');
			bool showStepsAfterBottleneck = c(in formatLower, '.');
			bool showAttributes = c(in formatLower, 'a');
			bool showBackdoors = c(in formatLower, 'b');
			bool showTechniqueDetail = c(in formatLower, 'd');
			bool showTechniqueSteps = c(in formatLower, 'l');

			ChangeLanguage(countryCode);

			// Get all information.
			var (
				solverName, hasSolved, total, max, pearl, diamond,
				puzzle, solution, elapsed, stepsCount, steps, _, additional) = Result;

			// Print header.
			var sb = new StringBuilder()
				.Append(GetValue("AnalysisResultPuzzle"))
				.AppendLine(puzzle.ToString("#"))
				.Append(GetValue("AnalysisResultSolvingTool"))
				.AppendLine(solverName);

			// Print solving steps (if worth).
			var bottleneckData = GetBottleneckData();

			void a(bool showSeparator) =>
				sb.Append(showSeparator ? $"{new string('-', 10)}{Environment.NewLine}" : string.Empty);
			if (showTechniqueSteps && steps is { Count: not 0 })
			{
				sb.AppendLine(GetValue("AnalysisResultSolvingSteps"));
				if (bottleneckData is not null)
				{
					var (bIndex, bInfo) = bottleneckData.Value;
					for (int i = 0; i < steps.Count; i++)
					{
						if (i > bIndex && showStepsAfterBottleneck)
						{
							sb.AppendLine(GetValue("Ellipsis"));
							break;
						}

						var info = steps[i];
						string infoStr = showSimple ? info.ToSimpleString() : info.ToString();
						bool showDiff = showDifficulty && info.ShowDifficulty;
						string labelInfo = (showStepNum, showDiff) switch
						{
							(true, true) => $"{i + 1,4}, {$"({info.Difficulty}",5:0.0}) ",
							(true, false) => $"{i + 1,4} ",
							(false, true) => $"{$"({info.Difficulty}",5:0.0}) ",
							_ => string.Empty,
						};
						sb.AppendLine($"{labelInfo}{infoStr}");
					}

					if (showBottleneck)
					{
						a(showSeparator);

						sb
							.Append(GetValue("AnalysisResultBottleneckStep"))
							.Append(
								showStepNum
								? $"{GetValue("AnalysisResultInStep")}{bIndex + 1}{GetValue("Colon")}"
								: string.Empty)
							.Append(' ')
							.AppendLine(bInfo);
					}

					a(showSeparator);
				}
			}

			// Print solving step statistics (if worth).
			if (GetSolvingStepsGrouped()?.ToList() is { Count: not 0 } solvingStepsGrouped)
			{
				sb.AppendLine(GetValue("AnalysisResultTechniqueUsed"));
				if (showTechniqueDetail)
				{
					sb
						.Append($"{GetValue("AnalysisResultMin"),6}")
						.Append(',')
						.Append(' ')
						.Append($"{GetValue("AnalysisResultTotal"),6}")
						.AppendLine(GetValue("AnalysisResultTechniqueUsing"));
				}

				foreach (var solvingStepsGroup in solvingStepsGrouped)
				{
					if (showTechniqueDetail)
					{
						decimal currentTotal = 0, currentMinimum = decimal.MaxValue;
						foreach (var solvingStep in solvingStepsGroup)
						{
							decimal difficulty = solvingStep.Difficulty;
							currentTotal += difficulty;
							currentMinimum = Math.Min(currentMinimum, difficulty);
						}
						sb
							.Append($"{$"({currentMinimum:0.0}",6}")
							.Append(',')
							.Append(' ')
							.Append($"{currentTotal,6:0.0}")
							.Append(')')
							.Append(' ');
					}

					sb
						.Append($"{solvingStepsGroup.Count(),3}")
						.Append(' ')
						.Append('*')
						.Append(' ')
						.AppendLine(solvingStepsGroup.Key);
				}

				if (showTechniqueDetail)
				{
					sb
						.Append($"{"(---",6}")
						.Append($"{total,6}")
						.Append(')')
						.Append(' ');
				}

				sb
					.Append($"{stepsCount,3}")
					.AppendLine(
						$@"{(
							stepsCount == 1
							? GetValue("AnalysisResultStepSingular")
							: GetValue("AnalysisResultStepPlural")
						)}");

				a(showSeparator);
			}

			// Print detail data.
#if OBSOLETE
			sb.AppendLine($"Total solving steps count: {stepsCount}");
			sb.AppendLine($"Difficulty total: {total}");
#endif
			sb
				.Append(GetValue("AnalysisResultPuzzleRating"))
				.Append(max.ToString("0.0"))
				.Append('/')
				.Append(pearl.ToString("0.0"))
				.Append('/')
				.AppendLine(diamond.ToString("0.0"));

			// Print the solution (if not null).
			if (solution.HasValue)
			{
				sb
					.Append(GetValue("AnalysisResultPuzzleSolution"))
					.AppendLine(solution.Value.ToString("!"));
			}

			// Print the elapsed time.
			sb
				.Append(GetValue("AnalysisResultPuzzleHas"))
				.Append(hasSolved ? string.Empty : GetValue("AnalysisResultNot"))
				.AppendLine(GetValue("AnalysisResultBeenSolved"))
				.Append(GetValue("AnalysisResultTimeElapsed"))
				.AppendLine(elapsed.ToString(@"hh\:mm\.ss\.fff"));
			a(showSeparator);

			// Print attributes (if worth).
			// Here use dynamic call (reflection) to get all methods which contains
			// only one parameter and its type is 'Grid'.
			if (showAttributes)
			{
				sb.AppendLine(GetValue("AnalysisResultAttributes"));
				foreach (var methodInfo in
					from methodInfo in typeof(PuzzleAttributeChecker).GetMethods()
					where methodInfo.ReturnType == typeof(bool)
					let @params = methodInfo.GetParameters()
					where @params.Length == 1 && @params[0].ParameterType == typeof(SudokuGrid)
					select methodInfo)
				{
					bool attributeResult = (bool)methodInfo.Invoke(null, new object[] { puzzle })!;
					sb
						.Append(new string(' ', 4))
						.Append(methodInfo.Name)
						.Append(':')
						.Append(' ')
						.AppendLine(attributeResult);
				}

				a(showSeparator);
			}

			// Print backdoors (if worth).
			if (showBackdoors)
			{
				sb.AppendLine(GetValue("AnalysisResultBackdoors"));
				var searcher = new BackdoorSearcher();
				foreach (var assignment in searcher.SearchForBackdoorsExact(puzzle, 0))
				{
					sb.Append(new string(' ', 4)).AppendLine(assignment[0]);
				}

				a(showSeparator);
			}

			// Print the additional information (if worth).
			if (additional is not null)
			{
				sb.AppendLine(additional);
			}

			return sb.ToString();
		}

		/// <summary>
		/// Indicates all groups that grouped by solving steps during solving.
		/// If and only if <see cref="AnalysisResult.SolvingSteps"/> is <see langword="null"/>, this value
		/// will be <see langword="null"/>.
		/// </summary>
		/// <returns>The list grouped and ordered.</returns>
		/// <seealso cref="AnalysisResult.SolvingSteps"/>
		private IEnumerable<IGrouping<string, TechniqueInfo>>? GetSolvingStepsGrouped() =>
			Result.SolvingSteps is IReadOnlyList<TechniqueInfo> solvingSteps
			? from step in solvingSteps orderby step.Difficulty group step by step.Name
			: null;

		/// <summary>
		/// Get the data of bottleneck.
		/// </summary>
		/// <returns>The data.</returns>
		private (int StepIndex, TechniqueInfo StepInfo)? GetBottleneckData()
		{
			var (_, solvingStepsCount, solvingSteps) = Result;
			if (solvingSteps is null)
			{
				return null;
			}

			for (int i = solvingStepsCount - 1; i >= 0; i--)
			{
				if (solvingSteps[i] is not SingleTechniqueInfo and { ShowDifficulty: true } step)
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
