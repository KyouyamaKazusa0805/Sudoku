using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Sudoku.Data;
using Sudoku.DocComments;
using Sudoku.Extensions;
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
		/// <exception cref="FormatException">
		/// Throws when the specified format contains other invalid characters
		/// and the format provider cannot work.
		/// </exception>
		public string ToString(string? format, IFormatProvider? formatProvider)
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

			// Get all information.
			var (
				solverName, hasSolved, total, max, pearl, diamond,
				puzzle, solution, elapsed, stepsCount, steps, _, additional) = Result;

			// Print header.
			var sb =
				new StringBuilder()
					.AppendLine($"{GetValue("AnalysisResultPuzzle")}{puzzle:#}")
					.AppendLine($"{GetValue("AnalysisResultSolvingTool")}{solverName}");

			// Print solving steps (if worth).
			var bottleneckData = GetBottleneckData();
			void a() => sb.Append(showSeparator ? $"{new string('-', 10)}{Environment.NewLine}" : string.Empty);
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
						a();

						sb.AppendLine(
							$"{GetValue("AnalysisResultBottleneckStep")}" +
							$@"{(
								showStepNum
									? $"{GetValue("AnalysisResultInStep")}{bIndex + 1}{GetValue("Colon")}"
									: string.Empty
							)}" +
							$" {bInfo}");
					}

					a();
				}
			}

			// Print solving step statistics (if worth).
			if (GetSolvingStepsGrouped()?.ToList() is { Count: not 0 } solvingStepsGrouped)
			{
				sb.AppendLine(GetValue("AnalysisResultTechniqueUsed"));
				if (showTechniqueDetail)
				{
					sb.AppendLine(
						$"{GetValue("AnalysisResultMin"),6}, {GetValue("AnalysisResultTotal"),6}" +
						GetValue("AnalysisResultTechniqueUsing"));
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
						sb.Append($"{$"({currentMinimum:0.0}",6}, {currentTotal,6:0.0}) ");
					}

					sb.AppendLine($"{solvingStepsGroup.Count(),3} * {solvingStepsGroup.Key}");
				}

				if (showTechniqueDetail)
				{
					sb.Append($"{"(---",6}, {total,6}) ");
				}

				sb.AppendLine(
					$"{stepsCount,3} " +
					$@"{(
						stepsCount == 1 ? GetValue("AnalysisResultStepSingular") : GetValue("AnalysisResultStepPlural")
					)}");

				a();
			}

			// Print detail data.
			//sb.AppendLine($"Total solving steps count: {stepsCount}");
			//sb.AppendLine($"Difficulty total: {total}");
			sb.AppendLine($"{GetValue("AnalysisResultPuzzleRating")}{max:0.0}/{pearl:0.0}/{diamond:0.0}");

			// Print the solution (if not null).
			if (solution is not null)
			{
				sb.AppendLine($"{GetValue("AnalysisResultPuzzleSolution")}{solution:!}");
			}

			// Print the elapsed time.
			sb
				.AppendLine(
					$"{GetValue("AnalysisResultPuzzleHas")}" +
					$"{(hasSolved ? string.Empty : GetValue("AnalysisResultNot"))}" +
					GetValue("AnalysisResultBeenSolved"))
				.AppendLine($"{GetValue("AnalysisResultTimeElapsed")}{elapsed:hh\\:mm\\.ss\\.fff}");
			a();

			// Print attributes (if worth).
			// Here use dynamic call (reflection) to get all methods which contains
			// only one parameter and its type is 'Grid'.
			if (showAttributes)
			{
				sb.AppendLine(GetValue("AnalysisResultAttributes"));

				static bool m(ParameterInfo[] p, MethodInfo m) =>
					p.Length == 1 && p[0].ParameterType == typeof(Grid) && m.ReturnType == typeof(bool);
				foreach (var methodInfo in
					from methodInfo in typeof(PuzzleAttributeChecker).GetMethods()
					let @params = methodInfo.GetParameters()
					where m(@params, methodInfo)
					select methodInfo)
				{
					bool attributeResult = (bool)methodInfo.Invoke(null, new[] { puzzle })!;
					sb.AppendLine($"    {methodInfo.Name}: {attributeResult}");
				}
				a();
			}

			// Print backdoors (if worth).
			if (showBackdoors)
			{
				sb.AppendLine(GetValue("AnalysisResultBackdoors"));
				var searcher = new BackdoorSearcher();
				foreach (var assignment in searcher.SearchForBackdoorsExact(puzzle, 0))
				{
					sb.AppendLine($"    {assignment[0]}");
				}
				a();
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
		private IEnumerable<IGrouping<string, TechniqueInfo>>? GetSolvingStepsGrouped()
		{
			var solvingSteps = Result.SolvingSteps;
			return solvingSteps is null
				? null
				: from step in solvingSteps orderby step.Difficulty group step by step.Name;
		}

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
				var step = solvingSteps[i];
				if (step is not SingleTechniqueInfo and { ShowDifficulty: true })
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
