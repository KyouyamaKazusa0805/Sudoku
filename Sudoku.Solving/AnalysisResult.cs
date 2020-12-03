using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.DocComments;
using Sudoku.Extensions;
using Sudoku.Globalization;
using Sudoku.Solving.Manual;
using Sudoku.Solving.Manual.Singles;

namespace Sudoku.Solving
{
	/// <summary>
	/// Provides an analysis result after a puzzle solved.
	/// </summary>
	/// <param name="HasSolved">Indicates whether the puzzle has been solved.</param>
	/// <param name="SolverName">Indicates the solver name.</param>
	/// <param name="Additional">Indicates the additional texts.</param>
	/// <param name="ElapsedTime">The elapsed time.</param>
	/// <param name="Puzzle">Indicates the puzzle.</param>
	/// <param name="Solution">
	/// Indicates the solution of the puzzle. If the puzzle doesn't contain non-unique solution,
	/// the value will be <see langword="null"/>.
	/// </param>
	/// <param name="SolvingSteps">The solving steps.</param>
	/// <param name="StepGrids">The step grids while solving.</param>
	public sealed record AnalysisResult(
		bool HasSolved, string SolverName, string? Additional, in TimeSpan ElapsedTime, in SudokuGrid Puzzle,
		in SudokuGrid? Solution, IReadOnlyList<TechniqueInfo>? SolvingSteps,
		IReadOnlyList<SudokuGrid>? StepGrids) : IEnumerable<TechniqueInfo>, IFormattable
	{
		/// <summary>
		/// Initializes an instance with some information.
		/// </summary>
		/// <param name="solverName">The name of the solver.</param>
		/// <param name="puzzle">(<see langword="in"/> parameter) The puzzle.</param>
		/// <param name="solution">(<see langword="in"/> parameter) The solution grid.</param>
		/// <param name="hasSolved">Indicates whether the puzzle has been solved.</param>
		/// <param name="elapsedTime">(<see langword="in"/> parameter) The elapsed time while solving.</param>
		/// <param name="additional">The additional message.</param>
		public AnalysisResult(
			string solverName, in SudokuGrid puzzle, in SudokuGrid? solution, bool hasSolved,
			in TimeSpan elapsedTime, string? additional)
			: this(solverName, puzzle, solution, hasSolved, elapsedTime, null, null, additional)
		{
		}

		/// <summary>
		/// Initializes an instance with some information.
		/// </summary>
		/// <param name="solverName">The name of the solver.</param>
		/// <param name="puzzle">(<see langword="in"/> parameter) The puzzle.</param>
		/// <param name="solution">(<see langword="in"/> parameter) The solution grid.</param>
		/// <param name="hasSolved">Indicates whether the puzzle has been solved.</param>
		/// <param name="elapsedTime">(<see langword="in"/> parameter) The elapsed time while solving.</param>
		/// <param name="steps">All steps produced in solving.</param>
		/// <param name="stepGrids">All intermediate grids.</param>
		public AnalysisResult(
			string solverName, in SudokuGrid puzzle, in SudokuGrid? solution, bool hasSolved,
			in TimeSpan elapsedTime, IReadOnlyList<TechniqueInfo>? steps, IReadOnlyList<SudokuGrid>? stepGrids)
			: this(solverName, puzzle, solution, hasSolved, elapsedTime, steps, stepGrids, null)
		{
		}

		/// <summary>
		/// Initializes an instance with some information.
		/// </summary>
		/// <param name="solverName">The name of the solver.</param>
		/// <param name="puzzle">(<see langword="in"/> parameter) The puzzle.</param>
		/// <param name="solution">(<see langword="in"/> parameter) The solution grid.</param>
		/// <param name="hasSolved">Indicates whether the puzzle has been solved.</param>
		/// <param name="elapsedTime">(<see langword="in"/> parameter) The elapsed time while solving.</param>
		/// <param name="steps">All steps produced in solving.</param>
		/// <param name="stepGrids">All intermediate grids.</param>
		/// <param name="additional">The additional message.</param>
		public AnalysisResult(
			string solverName, in SudokuGrid puzzle, in SudokuGrid? solution, bool hasSolved,
			in TimeSpan elapsedTime, IReadOnlyList<TechniqueInfo>? steps, IReadOnlyList<SudokuGrid>? stepGrids,
			string? additional)
			: this(hasSolved, solverName, additional, elapsedTime, puzzle, solution, steps, stepGrids)
		{
		}


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
		public decimal MaxDifficulty
		{
			get
			{
				unsafe
				{
					return SolvingSteps?.None() ?? true ? 20.0M : SolvingSteps.Max(&g);
				}

				static decimal g(in TechniqueInfo info) => info.ShowDifficulty ? info.Difficulty : 0;
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
		public decimal TotalDifficulty
		{
			get
			{
				if (SolvingSteps is null)
				{
					return 0;
				}

				decimal result = 0;
				foreach (var step in SolvingSteps)
				{
					result += step.ShowDifficulty ? step.Difficulty : 0;
				}

				return result;
			}
		}

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
		public decimal PearlDifficulty
		{
			get
			{
				unsafe
				{
					return SolvingSteps?.FirstOrDefault(&p)?.Difficulty ?? 0;
				}

				static bool p(in TechniqueInfo info) => info.ShowDifficulty;
			}
		}

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
					goto NotSolvedOrSolvingStepsIsNull;
				}

				if (HasSolved)
				{
					for (int i = 1, count = SolvingSteps.Count; i < count; i++)
					{
						if (SolvingSteps[i - 1] is { ShowDifficulty: true } info
							&& SolvingSteps[i] is SingleTechniqueInfo)
						{
							return info.Difficulty;
						}
					}
				}

			NotSolvedOrSolvingStepsIsNull:
				return 20M;
			}
		}

		/// <summary>
		/// Indicates the number of all solving steps recorded.
		/// </summary>
		public int SolvingStepsCount => SolvingSteps?.Count ?? 1;

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
				if ((HasSolved, SolvingSteps) is (true, not null))
				{
					foreach (var step in SolvingSteps!)
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
					if (SolvingSteps[i] is not SingleTechniqueInfo and { ShowDifficulty: true } step)
					{
						return step;
					}
				}

				// If code goes to here, all steps are more difficult than single techniques.
				// Get the first one is okay.
				return SolvingSteps[0];
			}
		}


		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="solverName">
		/// (<see langword="out"/> parameter) The solver's name.
		/// </param>
		/// <param name="hasSolved">
		/// (<see langword="out"/> parameter) Indicates whether the puzzle has been solved.
		/// </param>
		public void Deconstruct(out string solverName, out bool hasSolved)
		{
			solverName = SolverName;
			hasSolved = HasSolved;
		}

		/// <inheritdoc cref="DeconstructMethod"/>
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
			out bool hasSolved, out int solvingStepsCount, out IReadOnlyList<TechniqueInfo>? solvingSteps)
		{
			hasSolved = HasSolved;
			solvingStepsCount = SolvingStepsCount;
			solvingSteps = SolvingSteps;
		}

		/// <inheritdoc cref="DeconstructMethod"/>
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
		public void Deconstruct(out decimal? total, out decimal max, out decimal? pearl, out decimal? diamond)
		{
			total = TotalDifficulty;
			max = MaxDifficulty;
			pearl = PearlDifficulty;
			diamond = DiamondDifficulty;
		}

		/// <inheritdoc cref="DeconstructMethod"/>
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
			out SudokuGrid puzzle, out bool hasSolved, out TimeSpan elapsedTime,
			out SudokuGrid? solution, out DifficultyLevel difficultyLevel)
		{
			puzzle = Puzzle;
			hasSolved = HasSolved;
			elapsedTime = ElapsedTime;
			solution = Solution;
			difficultyLevel = DifficultyLevel;
		}

		/// <inheritdoc cref="DeconstructMethod"/>
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
		/// <param name="stepGrids">
		/// (<see langword="out"/> parameter) The grids corresponding to the steps.
		/// </param>
		/// <param name="additional">(<see langword="out"/> parameter) The additional message.</param>
		public void Deconstruct(
			out string solverName, out bool hasSolved, out decimal total, out decimal max, out decimal pearl,
			out decimal diamond, out SudokuGrid puzzle, out SudokuGrid? solution, out TimeSpan elasped,
			out int stepCount, out IReadOnlyList<TechniqueInfo>? steps, out IReadOnlyList<SudokuGrid>? stepGrids,
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
		/// <para>Note that this method won't return <see langword="null"/> anytime.</para>
		/// </summary>
		/// <returns>
		/// An enumerator that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator<TechniqueInfo> GetEnumerator() =>
			(SolvingSteps ?? Array.Empty<TechniqueInfo>()).GetEnumerator();

		/// <inheritdoc/>
		public override string ToString() => ToString(null, null);

		/// <inheritdoc cref="Formattable.ToString(string)"/>
		public string ToString(string format) => ToString(format, null);

		/// <summary>
		/// Get the analysis result string using the specified format and the country code.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <param name="countryCode">The country code.</param>
		/// <returns>The result string.</returns>
		public string ToString(string format, CountryCode countryCode) =>
			new AnalysisResultFormatter(this).ToString(format, null, countryCode);

		/// <inheritdoc/>
		public string ToString(string? format, IFormatProvider? formatProvider) =>
			new AnalysisResultFormatter(this).ToString(format, formatProvider);

		/// <inheritdoc cref="AnalysisResultFormatter.ToString(AnalysisResultFormattingOptions)"/>
		public string ToString(AnalysisResultFormattingOptions options) =>
			new AnalysisResultFormatter(this).ToString(options);

		/// <inheritdoc cref="AnalysisResultFormatter.ToString(AnalysisResultFormattingOptions, CountryCode)"/>
		public string ToString(AnalysisResultFormattingOptions options, CountryCode countryCode) =>
			new AnalysisResultFormatter(this).ToString(options, countryCode);

		/// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
