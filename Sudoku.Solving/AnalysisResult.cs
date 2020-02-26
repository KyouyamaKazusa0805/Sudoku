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
					? 20m
					: SolvingSteps.Max(info => info.ShowDifficulty ? info.Difficulty : 0);
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
		public decimal TotalDifficulty =>
			SolvingSteps?.Sum(info => info.ShowDifficulty ? info.Difficulty : 0) ?? 0;

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
		public decimal PearlDifficulty =>
			SolvingSteps?.FirstOrDefault(info => info.ShowDifficulty)?.Difficulty ?? 0;

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
						for (int i = 1, count = SolvingSteps.Count; i < count; i++)
						{
							var info = SolvingSteps[i - 1];
							if (info.ShowDifficulty && SolvingSteps[i] is SingleTechniqueInfo)
							{
								return info.Difficulty;
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
		/// solvers, this value will be <see cref="DifficultyLevel.Unknown"/>.
		/// </summary>
		public DifficultyLevel DifficultyLevel
		{
			get
			{
				var maxLevel = DifficultyLevel.Unknown;
				if (HasSolved && !(SolvingSteps is null))
				{
					foreach (var step in SolvingSteps)
					{
						if (step.ShowDifficulty && step.DifficultyLevel > maxLevel)
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
		/// Indicates the bottle neck during the whole grid solving.
		/// </summary>
		public TechniqueInfo? Bottleneck
		{
			get
			{
				if (SolvingSteps is null)
				{
					return null;
				}

				for (int i = SolvingSteps.Count - 1; i >= 0; i--)
				{
					var step = SolvingSteps[i];
					if (!(step is SingleTechniqueInfo) && step.ShowDifficulty)
					{
						return step;
					}
				}

				// If code goes to here, all steps are more difficult than single techniques.
				// Get the first one is okay.
				return SolvingSteps[0];
			}
		}

		/// <summary>
		/// Indicates the solving steps during solving. If the puzzle is not
		/// solved and the manual solver cannot find out any steps, or else
		/// the puzzle is solved by other solvers, this value will be <see langword="null"/>.
		/// </summary>
		public IReadOnlyList<TechniqueInfo>? SolvingSteps { get; }


		/// <summary>
		/// Deconstruct this instance.
		/// </summary>
		/// <param name="solverName">
		/// (<see langword="out"/> parameter) The solver's name.
		/// </param>
		/// <param name="hasSolved">
		/// (<see langword="out"/> parameter) Indicates whether the puzzle has been solved.
		/// </param>
		public void Deconstruct(out string solverName, out bool hasSolved) =>
			(solverName, hasSolved) = (SolverName, HasSolved);

		/// <summary>
		/// Deconstruct this instance.
		/// </summary>
		/// <param name="hasSolved">
		/// (<see langword="out"/> parameter) Indicates whether the puzzle has been solved.
		/// </param>
		/// <param name="solvingStepsCount">
		/// (<see langword="out"/> parameter) The total number of all solving steps.
		/// </param>
		/// <param name="solvingSteps">
		/// (<see langword="out"/> parameter) The all solving steps.
		/// </param>
		public void Deconstruct(
			out bool hasSolved, out int solvingStepsCount,
			out IReadOnlyList<TechniqueInfo>? solvingSteps) =>
			(hasSolved, solvingStepsCount, solvingSteps) = (HasSolved, SolvingStepsCount, SolvingSteps);

		/// <summary>
		/// Deconstruct this instance.
		/// </summary>
		/// <param name="total">
		/// (<see langword="out"/> parameter) The total difficulty.
		/// </param>
		/// <param name="max">
		/// (<see langword="out"/> parameter) The maximum difficulty.
		/// </param>
		/// <param name="pearl">
		/// (<see langword="out"/> parameter) The pearl difficulty.
		/// </param>
		/// <param name="diamond">
		/// (<see langword="out"/> parameter) The diamond difficulty.
		/// </param>
		public void Deconstruct(
			out decimal? total, out decimal max,
			out decimal? pearl, out decimal? diamond) =>
			(total, max, pearl, diamond) = (TotalDifficulty, MaxDifficulty, PearlDifficulty, DiamondDifficulty);

		/// <summary>
		/// Deconstruct this instance.
		/// </summary>
		/// <param name="puzzle">
		/// (<see langword="out"/> parameter) The initial puzzle.
		/// </param>
		/// <param name="hasSolved">
		/// (<see langword="out"/> parameter) Indicates whether the puzzle has been solved.
		/// </param>
		/// <param name="elapsedTime">
		/// (<see langword="out"/> parameter) The elapsed time during solving.
		/// </param>
		/// <param name="solution">
		/// (<see langword="out"/> parameter) The solution.
		/// </param>
		/// <param name="difficultyLevel">
		/// (<see langword="out"/> parameter) The difficulty level.
		/// </param>
		public void Deconstruct(
			out Grid puzzle, out bool hasSolved, out TimeSpan elapsedTime,
			out Grid? solution, out DifficultyLevel difficultyLevel) =>
			(puzzle, hasSolved, elapsedTime, solution, difficultyLevel) = (Puzzle, HasSolved, ElapsedTime, Solution, DifficultyLevel);

		/// <summary>
		/// Deconstruct this instance.
		/// </summary>
		/// <param name="puzzle">
		/// (<see langword="out"/> parameter) The initial puzzle.
		/// </param>
		/// <param name="hasSolved">
		/// (<see langword="out"/> parameter) Indicates whether the grid has been solved.
		/// </param>
		/// <param name="solution">
		/// (<see langword="out"/> parameter) The solution.
		/// </param>
		/// <param name="difficultyLevel">
		/// (<see langword="out"/> parameter) The difficulty level.
		/// </param>
		/// <param name="bottleneck">
		/// (<see langword="out"/> parameter) The bottleneck.
		/// </param>
		/// <param name="solvingSteps">
		/// (<see langword="out"/> parameter) All steps.
		/// </param>
		public void Deconstruct(
			out Grid puzzle, out bool hasSolved, out Grid? solution,
			out DifficultyLevel difficultyLevel, out TechniqueInfo? bottleneck,
			out IReadOnlyList<TechniqueInfo>? solvingSteps) =>
			(puzzle, hasSolved, solution, difficultyLevel, bottleneck, solvingSteps) = (Puzzle, HasSolved, Solution, DifficultyLevel, Bottleneck, SolvingSteps);

		/// <summary>
		/// Deconstruct this instance.
		/// </summary>
		/// <param name="puzzle">
		/// (<see langword="out"/> parameter) The initial puzzle.
		/// </param>
		/// <param name="hasSolved">
		/// (<see langword="out"/> parameter) Indicates whether the puzzle has solved.
		/// </param>
		/// <param name="elapsedTime">
		/// (<see langword="out"/> parameter) The elapsed time during solving.
		/// </param>
		/// <param name="solution">
		/// (<see langword="out"/> parameter) The solution.
		/// </param>
		/// <param name="difficultyLevel">
		/// (<see langword="out"/> parameter) The difficulty level.
		/// </param>
		/// <param name="solvingStepsCount">
		/// (<see langword="out"/> parameter) The number of solving steps recorded.
		/// </param>
		/// <param name="solvingSteps">
		/// (<see langword="out"/> parameter) All solving steps.
		/// </param>
		/// <param name="additionalMessage">
		/// (<see langword="out"/> parameter) The additional message.
		/// </param>
		public void Deconstruct(
			out Grid puzzle, out bool hasSolved, out TimeSpan elapsedTime,
			out Grid? solution, out DifficultyLevel difficultyLevel,
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
		public string ToString(string? format, IFormatProvider? formatProvider) =>
			new AnalysisResultFormatter(this).ToString(format, formatProvider);

		/// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
