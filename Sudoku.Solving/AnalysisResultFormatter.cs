using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Solving.Checking;
using Sudoku.Solving.Manual.Singles;

namespace Sudoku.Solving
{
	/// <summary>
	/// Provides operations for analysis result formatting.
	/// </summary>
	[DebuggerStepThrough]
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

		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="ToString" and @paramType="string"]'/>
		public string ToString(string format) => ToString(format, null);

		/// <inheritdoc/>
		/// <exception cref="ArgumentNullException">
		/// Throws when the specified format is <see langword="null"/> and the format provider
		/// cannot work.
		/// </exception>
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
			bool c(char c) => formatLower.Contains(c);
			bool showSeparator = c('-');
			bool showStepNum = c('#');
			bool showSimple = c('@');
			bool showBottleneck = c('?');
			bool showDifficulty = c('!');
			bool showStepsAfterBottleneck = c('.');
			bool showAttributes = c('a');
			bool showBackdoors = c('b');
			bool showTechniqueDetail = c('d');
			bool showTechniqueSteps = c('l');

			// Get all information.
			var (solverName, hasSolved) = Result;
			var (total, max, pearl, diamond) = Result;
			var (puzzle, _, elapsed, solution, _, stepsCount, steps, additional) = Result;

			// Print header.
			var sb = new StringBuilder();
			sb.AppendLine($"Puzzle: {puzzle:#}");
			sb.AppendLine($"Solving tool: {solverName}");

			// Print solving steps (if worth).
			var bottleneckData = GetBottleneckData();
			string separator = $"{new string('-', 10)}{Environment.NewLine}";
			void appendSeparator() => sb.Append(showSeparator ? separator : string.Empty);
			if (!(steps is null) && steps.Count != 0 && showTechniqueSteps)
			{
				sb.AppendLine("Solving steps:");
				if (!(bottleneckData is null))
				{
					var (bIndex, bInfo) = (ValueTuple<int, TechniqueInfo>)bottleneckData;
					for (int i = 0; i < steps.Count; i++)
					{
						if (i > bIndex && showStepsAfterBottleneck)
						{
							sb.AppendLine("......");
							break;
						}

						var info = steps[i];
						string infoStr = showSimple ? info.ToSimpleString() : info.ToString();
						bool showDiff = showDifficulty ? info.ShowDifficulty : false;
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
						appendSeparator();

						string bottleLabelInfo = showStepNum ? $" In step {bIndex + 1}:" : string.Empty;
						sb.AppendLine($"Bottleneck step:{bottleLabelInfo} {bInfo}");
					}

					appendSeparator();
				}
			}

			// Print solving step statistics (if worth).
			var solvingStepsGrouped = GetSolvingStepsGrouped();
			if (!(solvingStepsGrouped is null) && solvingStepsGrouped.Count() != 0)
			{
				sb.AppendLine("Technique used:");
				if (showTechniqueDetail)
				{
					sb.AppendLine($"{"min",6}, {"total",6}  technique using");
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

				sb.AppendLine($"{stepsCount,3} {(stepsCount == 1 ? "step" : "steps")} in total");

				appendSeparator();
			}

			// Print detail data.
			//sb.AppendLine($"Total solving steps count: {stepsCount}");
			//sb.AppendLine($"Difficulty total: {total}");
			sb.AppendLine($"Puzzle rating: {max:0.0}/{pearl:0.0}/{diamond:0.0}");

			// Print the solution (if not null).
			if (!(solution is null))
			{
				sb.AppendLine($"Puzzle solution: {solution:!}");
			}

			// Print the elapsed time.
			sb.AppendLine($"Puzzle has {(hasSolved ? "" : "not ")}been solved.");
			sb.AppendLine($"Time elapsed: {elapsed:hh':'mm'.'ss'.'fff}");
			appendSeparator();

			// Print attributes (if worth).
			// Here use dynamic call (reflection) to get all methods which contains
			// only one parameter and its type is 'IReadOnlyGrid'.
			if (showAttributes)
			{
				sb.AppendLine("Attributes:");

				static bool isSpecifiedMethod(ParameterInfo[] parameters, MethodInfo methodInfo) =>
					parameters.Length == 1
					&& parameters[0].ParameterType == typeof(IReadOnlyGrid)
					&& methodInfo.ReturnType == typeof(bool);
				foreach (var methodInfo in
					from methodInfo in typeof(PuzzleAttributeChecker).GetMethods()
					let parameters = methodInfo.GetParameters()
					where isSpecifiedMethod(parameters, methodInfo)
					select methodInfo)
				{
					bool attributeResult = (bool)methodInfo.Invoke(null, new object[] { puzzle })!;
					sb.AppendLine($"    {methodInfo.Name}: {attributeResult}");
				}
				appendSeparator();
			}

			// Print backdoors (if worth).
			if (showBackdoors)
			{
				sb.AppendLine("Backdoors:");
				var searcher = new BackdoorSearcher();
				foreach (var assignment in searcher.SearchForBackdoorsExact(puzzle, 0))
				{
					sb.AppendLine($"    {assignment[0]}");
				}
				appendSeparator();
			}

			// Print the additional information (if worth).
			if (!(additional is null))
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
			(_, _, var solvingSteps) = Result;
			return solvingSteps is null
				? null
				: from solvingStep in solvingSteps
				  orderby solvingStep.Difficulty
				  group solvingStep by solvingStep.Name;
		}

		/// <summary>
		/// Get the data of bottleneck.
		/// </summary>
		/// <returns>The data.</returns>
		private (int _stepIndex, TechniqueInfo _stepInfo)? GetBottleneckData()
		{
			var (_, solvingStepsCount, solvingSteps) = Result;
			if (solvingSteps is null)
			{
				return null;
			}

			for (int i = solvingStepsCount - 1; i >= 0; i--)
			{
				var step = solvingSteps[i];
				if (!(step is SingleTechniqueInfo) && step.ShowDifficulty)
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
