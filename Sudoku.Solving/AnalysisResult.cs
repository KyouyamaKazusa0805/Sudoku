using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
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
		/// <param name="solverName">The name of the solver.</param>
		/// <param name="puzzle">The puzzle.</param>
		/// <param name="solution">The solution grid.</param>
		/// <param name="hasSolved">Indicates whether the puzzle has been solved.</param>
		/// <param name="elapsedTime">The elapsed time while solving.</param>
		/// <param name="additional">The additional message.</param>
		public AnalysisResult(
			string solverName, IReadOnlyGrid puzzle, IReadOnlyGrid? solution, bool hasSolved,
			TimeSpan elapsedTime, string? additional)
			: this(solverName, puzzle, solution, hasSolved, elapsedTime, null, null, additional)
		{
		}

		/// <summary>
		/// Initializes an instance with some information.
		/// </summary>
		/// <param name="solverName">The name of the solver.</param>
		/// <param name="puzzle">The puzzle.</param>
		/// <param name="solution">The solution grid.</param>
		/// <param name="hasSolved">Indicates whether the puzzle has been solved.</param>
		/// <param name="elapsedTime">The elapsed time while solving.</param>
		/// <param name="steps">All steps produced in solving.</param>
		/// <param name="stepGrids">All intermediate grids.</param>
		public AnalysisResult(
			string solverName, IReadOnlyGrid puzzle, IReadOnlyGrid? solution, bool hasSolved,
			TimeSpan elapsedTime, IReadOnlyList<TechniqueInfo>? steps, IReadOnlyList<IReadOnlyGrid>? stepGrids)
			: this(solverName, puzzle, solution, hasSolved, elapsedTime, steps, stepGrids, null)
		{
		}

		/// <summary>
		/// Initializes an instance with some information.
		/// </summary>
		/// <param name="solverName">The name of the solver.</param>
		/// <param name="puzzle">The puzzle.</param>
		/// <param name="solution">The solution grid.</param>
		/// <param name="hasSolved">Indicates whether the puzzle has been solved.</param>
		/// <param name="elapsedTime">The elapsed time while solving.</param>
		/// <param name="steps">All steps produced in solving.</param>
		/// <param name="stepGrids">All intermediate grids.</param>
		/// <param name="additional">The additional message.</param>
		public AnalysisResult(
			string solverName, IReadOnlyGrid puzzle, IReadOnlyGrid? solution, bool hasSolved,
			TimeSpan elapsedTime, IReadOnlyList<TechniqueInfo>? steps, IReadOnlyList<IReadOnlyGrid>? stepGrids,
			string? additional) =>
			(Puzzle, SolverName, HasSolved, Solution, SolvingSteps, ElapsedTime, Additional, StepGrids) = (
				puzzle, solverName, hasSolved, solution, steps, elapsedTime, additional, stepGrids);


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
		/// be always <c>20M</c>.
		/// </para>
		/// </summary>
		/// <seealso cref="ManualSolver"/>
		public decimal MaxDifficulty =>
			SolvingSteps?.None() ?? true
				? 20M
				: SolvingSteps.Max(info => info.ShowDifficulty ? info.Difficulty : 0);

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
		public decimal TotalDifficulty => SolvingSteps?.Sum(info => info.ShowDifficulty ? info.Difficulty : 0) ?? 0;

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
		public decimal PearlDifficulty => SolvingSteps?.FirstOrDefault(info => info.ShowDifficulty)?.Difficulty ?? 0;

		/// <summary>
		/// <para>
		/// Indicates the pearl difficulty rating of the puzzle, calculated
		/// during only by <see cref="ManualSolver"/>.
		/// </para>
		/// <para>
		/// When the puzzle is solved, the value will be the difficulty rating
		/// of the first step before the first one whose conclusion is
		/// <see cref="ConclusionType.Assignment"/>. If the puzzle has not solved
		/// or solved by other solvers, this value will be <c>20.0M</c>.
		/// </para>
		/// </summary>
		/// <seealso cref="ManualSolver"/>
		/// <seealso cref="ConclusionType"/>
		public decimal DiamondDifficulty
		{
			get
			{
				if (SolvingSteps is null)
				{
					return 20M;
				}

				if (HasSolved)
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

				// Not solved.
				return 20M;
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
				if (HasSolved && SolvingSteps is not null)
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
		/// Indicates the initial puzzle.
		/// </summary>
		public IReadOnlyGrid Puzzle { get; }

		/// <summary>
		/// Indicates the solution grid. If and only if the puzzle
		/// is not solved, this value will be <see langword="null"/>.
		/// </summary>
		public IReadOnlyGrid? Solution { get; }

		/// <summary>
		/// Indicates the solving steps during solving. If the puzzle is not
		/// solved and the manual solver cannot find out any steps, or else
		/// the puzzle is solved by other solvers, this value will be <see langword="null"/>.
		/// </summary>
		public IReadOnlyList<TechniqueInfo>? SolvingSteps { get; }

		/// <summary>
		/// Indicates the intermediate grids while solving.
		/// </summary>
		public IReadOnlyList<IReadOnlyGrid>? StepGrids { get; }


		/// <include file='..\GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="solverName">
		/// (<see langword="out"/> parameter) The solver's name.
		/// </param>
		/// <param name="hasSolved">
		/// (<see langword="out"/> parameter) Indicates whether the puzzle has been solved.
		/// </param>
		public void Deconstruct(out string solverName, out bool hasSolved) =>
			(solverName, hasSolved) = (SolverName, HasSolved);

		/// <include file='..\GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
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
			out bool hasSolved, out int solvingStepsCount, out IReadOnlyList<TechniqueInfo>? solvingSteps) =>
			(hasSolved, solvingStepsCount, solvingSteps) = (HasSolved, SolvingStepsCount, SolvingSteps);

		/// <include file='..\GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
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
		public void Deconstruct(out decimal? total, out decimal max, out decimal? pearl, out decimal? diamond) =>
			(total, max, pearl, diamond) = (TotalDifficulty, MaxDifficulty, PearlDifficulty, DiamondDifficulty);

		/// <include file='..\GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
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
			out IReadOnlyGrid puzzle, out bool hasSolved, out TimeSpan elapsedTime,
			out IReadOnlyGrid? solution, out DifficultyLevel difficultyLevel) =>
			(puzzle, hasSolved, elapsedTime, solution, difficultyLevel) = (Puzzle, HasSolved, ElapsedTime, Solution, DifficultyLevel);

		/// <include file='..\GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="solverName">(<see langword="out"/> parameter) The solver name.</param>
		/// <param name="hasSolved">
		/// (<see langword="out"/> parameter) Indicates whether the solver has solved the puzzle.
		/// </param>
		/// <param name="total">(<see langword="out"/> parameter) The total difficulty.</param>
		/// <param name="max">(<see langword="out"/> parameter) The maximum difficulty of all steps.</param>
		/// <param name="pearl">(<see langword="out"/> parameter) The pearl difficulty of the puzzle.</param>
		/// <param name="diamond">(<see langword="out"/> parameter) The diamond difficulty of the puzzle.</param>
		/// <param name="puzzle">(<see langword="out"/> parameter) The puzzle.</param>
		/// <param name="solution">(<see langword="out"/> parameter) The solution.</param>
		/// <param name="elasped">(<see langword="out"/> parameter) The time elapsed.</param>
		/// <param name="stepCount">(<see langword="out"/> parameter) The number of all steps.</param>
		/// <param name="steps">(<see langword="out"/> parameter) The steps.</param>
		/// <param name="stepGrids">(<see langword="out"/> parameter) The grids corresponding to the steps.</param>
		/// <param name="additional">(<see langword="out"/> parameter) The additional message.</param>
		public void Deconstruct(
			out string solverName, out bool hasSolved, out decimal total, out decimal max, out decimal pearl,
			out decimal diamond, out IReadOnlyGrid puzzle, out IReadOnlyGrid? solution, out TimeSpan elasped,
			out int stepCount, out IReadOnlyList<TechniqueInfo>? steps, out IReadOnlyList<IReadOnlyGrid>? stepGrids,
			out string? additional)
		{
			solverName = SolverName;
			hasSolved = HasSolved;
			total = TotalDifficulty;
			max = MaxDifficulty;
			pearl = PearlDifficulty;
			diamond = DiamondDifficulty;
			puzzle = Puzzle;
			solution = Solution;
			elasped = ElapsedTime;
			stepCount = SolvingStepsCount;
			steps = SolvingSteps;
			stepGrids = StepGrids;
			additional = Additional;
		}


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

		/// <include file='..\GlobalDocComments.xml' path='comments/method[@name="ToString" and @paramType="string"]'/>
		public string ToString(string format) => ToString(format, null);

		/// <inheritdoc/>
		public string ToString(string? format, IFormatProvider? formatProvider) =>
			new AnalysisResultFormatter(this).ToString(format, formatProvider);

		/// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
