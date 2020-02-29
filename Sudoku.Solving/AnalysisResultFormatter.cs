using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Sudoku.Data.Extensions;
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

		/// <summary>
		/// Returns a string that represents the current object with a specified format.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <returns>The string instance.</returns>
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

			if (format is null)
			{
				throw new ArgumentNullException(nameof(format));
			}
			if (Regex.IsMatch(format, @"[^\-\.#@%!]"))
			{
				throw new FormatException("The specified format is invalid due to with invalid characters.");
			}

			string formatLower = format.ToLower();
			bool showSeparator = formatLower.Contains('-');
			bool showStepNum = formatLower.Contains('#');
			bool showSimple = formatLower.Contains('@');
			bool showBottleneck = formatLower.Contains('%');
			bool showDifficulty = formatLower.Contains('!');
			bool showStepsAfterBottleneck = formatLower.Contains('.');

			var (solverName, hasSolved) = Result;
			var (total, max, pearl, diamond) = Result;
			var (puzzle, _, elapsed, solution, _, stepsCount, steps, additional) = Result;

			// Print header.
			var sb = new StringBuilder();
			sb.AppendLine($"Puzzle: {puzzle:#}");
			sb.AppendLine($"Solving tool: {solverName}");

			// Print solving steps.
			var bottleneckData = GetBottleneckData();
			string separator = new string('-', 10);
			if (!(steps is null) && steps.Count != 0)
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
						if (showSeparator)
						{
							sb.AppendLine(separator);
						}

						string bottleLabelInfo = showStepNum ? $" In step {bIndex + 1}:" : string.Empty;
						sb.AppendLine($"Bottleneck step:{bottleLabelInfo} {bInfo}");
					}

					if (showSeparator)
					{
						sb.AppendLine(separator);
					}
				}
			}

			// Print solving step statistics.
			var solvingStepsGrouped = GetSolvingStepsGrouped();
			if (!(solvingStepsGrouped is null) && solvingStepsGrouped.Count() != 0)
			{
				sb.AppendLine("Technique used:");
				foreach (var solvingStepsGroup in solvingStepsGrouped)
				{
					sb.AppendLine($"{solvingStepsGroup.Count()} * {solvingStepsGroup.Key}");
				}

				if (showSeparator)
				{
					sb.AppendLine(separator);
				}
			}

			// Print detail data.
			sb.AppendLine($"Total solving steps count: {stepsCount}");
			sb.AppendLine($"Difficulty total: {total}");
			sb.AppendLine($"Puzzle rating: {max:0.0}/{pearl:0.0}/{diamond:0.0}");

			// Print the solution (if not null).
			if (!(solution is null))
			{
				sb.AppendLine($"Puzzle solution: {solution:!}");
			}

			// Print the elapsed time.
			sb.AppendLine($"Puzzle has {(hasSolved ? "" : "not ")}been solved.");
			sb.AppendLine($"Time elapsed: {elapsed:hh':'mm'.'ss'.'fff}");

			// Print the additional information.
			if (!(additional is null))
			{
				if (showSeparator)
				{
					sb.AppendLine(separator);
				}

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
			var (_, _, solvingSteps) = Result;
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
