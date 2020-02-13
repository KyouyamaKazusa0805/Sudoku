using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sudoku.Data.Meta;
using Sudoku.Solving.Manual;
using Sudoku.Solving.Manual.Singles;

namespace Sudoku.Solving
{
	/// <summary>
	/// Provides an analysis result after a puzzle solved.
	/// </summary>
	public sealed class AnalysisResult : IEnumerable<TechniqueInfo>, IFormattable
	{
		/// <summary>
		/// Initializes an instance with some information.
		/// </summary>
		/// <param name="puzzle">The puzzle.</param>
		/// <param name="solverName">The name of the solver.</param>
		/// <param name="hasSolved">Indicates whether the puzzle has been solved.</param>
		/// <param name="solution">The solution grid.</param>
		/// <param name="elapsedTime">The elapsed time while solving.</param>
		/// <param name="solvingList">All steps produced in solving.</param>
		/// <param name="additional">The additional message.</param>
		public AnalysisResult(
			Grid puzzle, string solverName, bool hasSolved, Grid? solution,
			TimeSpan elapsedTime, IReadOnlyList<TechniqueInfo>? solvingList, string? additional) =>
			(Puzzle, SolverName, HasSolved, Solution, SolvingSteps, ElapsedTime, Additional) = (puzzle, solverName, hasSolved, solution, solvingList, elapsedTime, additional);


		/// <summary>
		/// <para>Indicates whether the puzzle has been solved.</para>
		/// <para>
		/// If the puzzle has multiple solutions or no solution,
		/// this value will be always <see langword="false"/>.
		/// </para>
		/// </summary>
		public bool HasSolved { get; }

		/// <summary>
		/// <para>Indicates the maximum difficulty of the puzzle.</para>
		/// <para>
		/// When the puzzle is solved by <see cref="ManualSolver"/>,
		/// the value will be the maximum value among all difficulty
		/// ratings in solving steps. If the puzzle has not been solved,
		/// or else the puzzle is solved by other solvers, this value will
		/// be always <c>20.0m</c>.
		/// </para>
		/// </summary>
		/// <seealso cref="ManualSolver"/>
		public decimal MaxDifficulty
		{
			get
			{
				return SolvingSteps is null || !SolvingSteps.Any()
					? 20
					: SolvingSteps.Max(info => info.Difficulty);
			}
		}

		/// <summary>
		/// <para>Indicates the total difficulty rating of the puzzle.</para>
		/// <para>
		/// When the puzzle is solved by <see cref="ManualSolver"/>,
		/// the value will be the sum of all difficulty ratings of steps. If
		/// the puzzle has not been solved, the value will be the sum of all
		/// difficulty ratings of steps recorded in <see cref="SolvingSteps"/>.
		/// However, if the puzzle is solved by other solvers, this value will
		/// be <c>0</c>.
		/// </para>
		/// </summary>
		/// <seealso cref="ManualSolver"/>
		/// <seealso cref="SolvingSteps"/>
		public decimal TotalDifficulty => SolvingSteps?.Sum(info => info.Difficulty) ?? 0;

		/// <summary>
		/// <para>
		/// Indicates the pearl difficulty rating of the puzzle, calculated
		/// during only by <see cref="ManualSolver"/>.
		/// </para>
		/// <para>
		/// When the puzzle is solved, the value will be the difficulty rating
		/// of the first solving step. If the puzzle has not solved or
		/// the puzzle is solved by other solvers, this value will be always <c>0</c>.
		/// </para>
		/// </summary>
		/// <seealso cref="ManualSolver"/>
		public decimal PearlDifficulty => (SolvingSteps?.FirstOrDefault()?.Difficulty) ?? 0;

		/// <summary>
		/// <para>
		/// Indicates the pearl difficulty rating of the puzzle, calculated
		/// during only by <see cref="ManualSolver"/>.
		/// </para>
		/// <para>
		/// When the puzzle is solved, the value will be the difficulty rating
		/// of the first step before the first one whose conclusion is
		/// <see cref="ConclusionType.Assignment"/>. If the puzzle has not solved
		/// or solved by other solvers, this value will be <c>20.0m</c>.
		/// </para>
		/// </summary>
		/// <seealso cref="ManualSolver"/>
		/// <seealso cref="ConclusionType"/>
		public decimal DiamondDifficulty
		{
			get
			{
				if (HasSolved)
				{
					if (SolvingSteps is null)
					{
						return 20m;
					}
					else
					{
						var list = new List<TechniqueInfo>(SolvingSteps);
						for (int i = 1; i < list.Count; i++)
						{
							if (list[i] is SingleTechniqueInfo)
							{
								return list[i - 1].Difficulty;
							}
						}
					}
				}

				// Not solved.
				return 20m;
			}
		}

		/// <summary>
		/// Indicates the number of all solving steps recorded.
		/// </summary>
		public int SolvingStepsCount => SolvingSteps?.Count ?? 1;

		/// <summary>
		/// Indicates the solver's name.
		/// </summary>
		public string SolverName { get; }

		/// <summary>
		/// Indicates the additional message during solving, which
		/// can be the message from an exception, or the debugging information.
		/// If this instance does not need to have this one, the value
		/// will be <see langword="null"/>.
		/// </summary>
		public string? Additional { get; }

		/// <summary>
		/// Indicates the solving elapsed time.
		/// </summary>
		public TimeSpan ElapsedTime { get; }

		/// <summary>
		/// Indicates the difficulty level of the puzzle.
		/// If the puzzle has not solved or solved by other
		/// solvers, this value will be <see cref="DifficultyLevels.Unknown"/>.
		/// </summary>
		public DifficultyLevels DifficultyLevel
		{
			get
			{
				var maxLevel = DifficultyLevels.Unknown;
				if (HasSolved && !(SolvingSteps is null))
				{
					foreach (var step in SolvingSteps)
					{
						if (step.DifficultyLevel > maxLevel)
						{
							maxLevel = step.DifficultyLevel;
						}
					}
				}

				return maxLevel;
			}
		}

		/// <summary>
		/// Indicates the initial puzzle.
		/// </summary>
		public Grid Puzzle { get; }

		/// <summary>
		/// Indicates the solution grid. If and only if the puzzle
		/// is not solved, this value will be <see langword="null"/>.
		/// </summary>
		public Grid? Solution { get; }

		/// <summary>
		/// Indicates the solving steps during solving. If the puzzle is not
		/// solved and the manual solver cannot find out any steps, or else
		/// the puzzle is solved by other solvers, this value will be <see langword="null"/>.
		/// </summary>
		public IReadOnlyList<TechniqueInfo>? SolvingSteps { get; }

		/// <summary>
		/// Indicates all groups that grouped by solving steps during solving.
		/// If and only if <see cref="SolvingSteps"/> is <see langword="null"/>, this value
		/// will be <see langword="null"/>.
		/// </summary>
		private IEnumerable<IGrouping<string, TechniqueInfo>>? SolvingStepsGrouped
		{
			get
			{
				return SolvingSteps is null
					? null
					: from solvingStep in SolvingSteps
					  orderby solvingStep.Difficulty
					  group solvingStep by solvingStep.Name;
			}
		}


		/// <summary>
		/// Deconstruct this instance.
		/// </summary>
		/// <param name="solverName">(out parameter) The solver's name.</param>
		/// <param name="hasSolved">
		/// (out parameter) Indicates whether the puzzle has been solved.
		/// </param>
		public void Deconstruct(out string solverName, out bool hasSolved) =>
			(solverName, hasSolved) = (SolverName, HasSolved);

		/// <summary>
		/// Deconstruct this instance.
		/// </summary>
		/// <param name="hasSolved">
		/// (out parameter) Indicates whether the puzzle has been solved.
		/// </param>
		/// <param name="solvingStepsCount">
		/// (out parameter) The total number of all solving steps.
		/// </param>
		/// <param name="solvingSteps">(out parameter) The all solving steps.</param>
		public void Deconstruct(
			out bool hasSolved,
			out int solvingStepsCount, out IReadOnlyList<TechniqueInfo>? solvingSteps) =>
			(hasSolved, solvingStepsCount, solvingSteps) = (HasSolved, SolvingStepsCount, SolvingSteps);

		/// <summary>
		/// Deconstruct this instance.
		/// </summary>
		/// <param name="total">(out parameter) The total difficulty.</param>
		/// <param name="max">(out parameter) The maximum difficulty.</param>
		/// <param name="pearl">(out parameter) The pearl difficulty.</param>
		/// <param name="diamond">(out parameter) The diamond difficulty.</param>
		public void Deconstruct(
			out decimal? total, out decimal max,
			out decimal? pearl, out decimal? diamond) =>
			(total, max, pearl, diamond) = (TotalDifficulty, MaxDifficulty, PearlDifficulty, DiamondDifficulty);

		/// <summary>
		/// Deconstruct this instance.
		/// </summary>
		/// <param name="puzzle">(out parameter) The initial puzzle.</param>
		/// <param name="hasSolved">
		/// (out parameter) Indicates whether the puzzle has been solved.
		/// </param>
		/// <param name="elapsedTime">
		/// (out parameter) The elapsed time during solving.
		/// </param>
		/// <param name="solution">(out parameter) The solution.</param>
		/// <param name="difficultyLevel">(out parameter) The difficulty level.</param>
		public void Deconstruct(
			out Grid puzzle, out bool hasSolved, out TimeSpan elapsedTime,
			out Grid? solution, out DifficultyLevels difficultyLevel) =>
			(puzzle, hasSolved, elapsedTime, solution, difficultyLevel) = (Puzzle, HasSolved, ElapsedTime, Solution, DifficultyLevel);

		/// <summary>
		/// Deconstruct this instance.
		/// </summary>
		/// <param name="puzzle">(out parameter) The initial puzzle.</param>
		/// <param name="hasSolved">
		/// (out parameter) Indicates whether the puzzle has solved.
		/// </param>
		/// <param name="elapsedTime">
		/// (out parameter) The elapsed time during solving.
		/// </param>
		/// <param name="solution">(out parameter) The solution.</param>
		/// <param name="difficultyLevel">(out parameter) The difficulty level.</param>
		/// <param name="solvingStepsCount">
		/// (out parameter) The number of solving steps recorded.
		/// </param>
		/// <param name="solvingSteps">(out parameter) All solving steps.</param>
		/// <param name="additionalMessage">
		/// (out parameter) The additional message.
		/// </param>
		public void Deconstruct(
			out Grid puzzle, out bool hasSolved, out TimeSpan elapsedTime,
			out Grid? solution, out DifficultyLevels difficultyLevel,
			out int solvingStepsCount, out IReadOnlyList<TechniqueInfo>? solvingSteps,
			out string? additionalMessage) =>
			(puzzle, hasSolved, elapsedTime, solution, difficultyLevel, solvingStepsCount, solvingSteps, additionalMessage) = (Puzzle, HasSolved, ElapsedTime, Solution, DifficultyLevel, SolvingStepsCount, SolvingSteps, Additional);

		/// <summary>
		/// <para>Returns an enumerator that iterates through the collection.</para>
		/// <para>Note that this method will not return <see langword="null"/> anytime.</para>
		/// </summary>
		/// <returns>
		/// An enumerator that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator<TechniqueInfo> GetEnumerator() =>
			(SolvingSteps ?? Array.Empty<TechniqueInfo>()).GetEnumerator();

		/// <inheritdoc/>
		public override string ToString() => ToString(null, null);

		/// <summary>
		/// Returns a string that represents the current object with a specified format.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <returns>The string instance.</returns>
		public string ToString(string format) => ToString(format, null);

		/// <inheritdoc/>
		public string ToString(string? format, IFormatProvider? formatProvider)
		{
			if (formatProvider?.GetFormat(GetType()) is ICustomFormatter customFormatter)
			{
				return customFormatter.Format(format, this, formatProvider);
			}

			string? formatLower = format?.ToLower();
			string separator = new string('-', 10);
			var sb = new StringBuilder();

			// Print header.
			sb.AppendLine($"Puzzle: {Puzzle:#}");
			sb.AppendLine($"Solving tool: {SolverName}");

			// Print solving steps.
			if (!(SolvingSteps is null) && SolvingSteps.Count != 0)
			{
				sb.AppendLine("Solving steps:");
				foreach (var info in SolvingSteps)
				{
					string infoStr = formatLower?.Contains('s') ?? false ? info.ToSimpleString() : info.ToString();
					sb.AppendLine($"{$"({info.Difficulty}",5:0.0}) {infoStr}");
				}

				sb.AppendLine(separator);
			}

			// Print solving step statistics.
			if (!(SolvingStepsGrouped is null) && SolvingStepsGrouped.Count() != 0)
			{
				sb.AppendLine("Technique used:");
				foreach (var solvingStepsGroup in SolvingStepsGrouped)
				{
					sb.AppendLine($"{solvingStepsGroup.Count()} * {solvingStepsGroup.Key}");
				}

				sb.AppendLine(separator);
			}

			// Print detail data.
			sb.AppendLine($"Total solving steps count: {SolvingStepsCount}");
			sb.AppendLine($"Difficulty total: {TotalDifficulty}");
			sb.AppendLine($"Puzzle rating: {MaxDifficulty:0.0}/{PearlDifficulty:0.0}/{DiamondDifficulty:0.0}");

			// Print the solution (if not null).
			if (!(Solution is null))
			{
				sb.AppendLine($"Puzzle solution: {Solution:!}");
			}

			// Print the elapsed time.
			sb.AppendLine($"Puzzle has {(HasSolved ? "" : "not ")}been solved.");
			sb.AppendLine($"Time elapsed: {ElapsedTime:hh':'mm'.'ss'.'fff}");

			// Print the additional information.
			if (!(Additional is null))
			{
				sb.AppendLine(separator);
				sb.AppendLine(Additional);
			}

			return sb.ToString();
		}

		/// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
