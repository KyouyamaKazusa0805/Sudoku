using System;
using System.Collections;
using System.Collections.Generic;
using System.Extensions;
using System.Linq;
using Sudoku.Data;
using Sudoku.DocComments;
using Sudoku.Globalization;
using Sudoku.Solving.Manual;
using Sudoku.Techniques;

namespace Sudoku.Solving
{
	/// <summary>
	/// Provides an analysis result after a puzzle solved.
	/// </summary>
	/// <param name="SolverName">Indicates the solver name.</param>
	/// <param name="Puzzle">Indicates the puzzle.</param>
	/// <param name="IsSolved">Indicates whether the puzzle has been solved.</param>
	/// <param name="ElapsedTime">The elapsed time.</param>
	public sealed partial record AnalysisResult(
		string SolverName, in SudokuGrid Puzzle, bool IsSolved, in TimeSpan ElapsedTime
	) : IEnumerable<StepInfo>, IFormattable
	{
		/// <summary>
		/// Indicates the additional texts that we should describe.
		/// </summary>
		public object? Additional { get; init; }

		/// <summary>
		/// Indicates the solution of the puzzle. If the puzzle doesn't contain non-unique solution,
		/// the value will be <see langword="null"/>.
		/// </summary>
		public SudokuGrid? Solution { get; init; }

		/// <summary>
		/// Indicates a list, whose element is the intermediate grid for each step.
		/// </summary>
		public IReadOnlyList<SudokuGrid>? StepGrids { get; init; }

		/// <summary>
		/// Indicates all solving steps that the solver has recorded.
		/// </summary>
		public IReadOnlyList<StepInfo>? Steps { get; init; }

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
		public decimal MaxDifficulty => Steps is { Count: not 0 } /*length-pattern*/
			? Steps.Max(static info => info.ShowDifficulty ? info.Difficulty : 0)
			: 20.0M;

		/// <summary>
		/// <para>Indicates the total difficulty rating of the puzzle.</para>
		/// <para>
		/// When the puzzle is solved by <see cref="ManualSolver"/>,
		/// the value will be the sum of all difficulty ratings of steps. If
		/// the puzzle has not been solved, the value will be the sum of all
		/// difficulty ratings of steps recorded in <see cref="Steps"/>.
		/// However, if the puzzle is solved by other solvers, this value will
		/// be <c>0</c>.
		/// </para>
		/// </summary>
		/// <seealso cref="ManualSolver"/>
		/// <seealso cref="Steps"/>
		public decimal TotalDifficulty
		{
			get
			{
				if (Steps is null)
				{
					return 0;
				}

				decimal result = 0;
				foreach (var step in Steps)
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
		public decimal PearlDifficulty =>
			Steps?.FirstOrDefault(static info => info.ShowDifficulty)?.Difficulty ?? 0;

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
				if (Steps is null)
				{
					return 20.0M;
				}

				if (IsSolved)
				{
					int index = Steps.FindIndexOf(static info => info.HasTag(TechniqueTags.Singles));
					return index switch
					{
						-1 => 20.0M,
						0 => Steps[0].Difficulty,
						_ => Steps.Slice(0, index).Max(static step => step.Difficulty)
					};
				}

				return 20.0M;
			}
		}

		/// <summary>
		/// Indicates the number of all solving steps recorded.
		/// </summary>
		public int SolvingStepsCount => Steps?.Count ?? 1;

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
				if (IsSolved && Steps is not null)
				{
					foreach (var step in Steps)
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
		public StepInfo? Bottleneck
		{
			get
			{
				if (Steps is null)
				{
					return null;
				}

				for (int i = Steps.Count - 1; i >= 0; i--)
				{
					var step = Steps[i];
					if (step.ShowDifficulty && !step.HasTag(TechniqueTags.Singles))
					{
						return step;
					}
				}

				// If code goes to here, all steps are more difficult than single techniques.
				// Get the first one is okay.
				return Steps[0];
			}
		}


		/// <summary>
		/// Gets the step information at the specified index.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns>The step information.</returns>
		/// <exception cref="InvalidOperationException">Throws when the result list is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Throws when the index is out of range.</exception>
		public StepInfo this[int index] => Steps is not { Count: not 0 }
			? throw new InvalidOperationException("You can't extract any elements because of being null.")
			: index >= Steps.Count || index < 0
			? throw new ArgumentOutOfRangeException(nameof(index))
			: Steps[index];


		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="hasSolved">Indicates whether the puzzle has been solved.</param>
		/// <param name="solvingStepsCount">The total number of all solving steps.</param>
		/// <param name="solvingSteps">The all solving steps.</param>
		public void Deconstruct(
			out bool hasSolved, out int solvingStepsCount, out IReadOnlyList<StepInfo>? solvingSteps)
		{
			hasSolved = IsSolved;
			solvingStepsCount = SolvingStepsCount;
			solvingSteps = Steps;
		}

		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="solverName">The solver name.</param>
		/// <param name="hasSolved">Indicates whether the solver has solved the puzzle.</param>
		/// <param name="total">The total difficulty.</param>
		/// <param name="max">The maximum difficulty of all steps.</param>
		/// <param name="pearl">The pearl difficulty of the puzzle.</param>
		/// <param name="diamond">The diamond difficulty of the puzzle.</param>
		/// <param name="puzzle">The puzzle.</param>
		/// <param name="solution">The solution.</param>
		/// <param name="elasped">The time elapsed.</param>
		/// <param name="stepCount">The number of all steps.</param>
		/// <param name="steps">The steps.</param>
		/// <param name="stepGrids">
		/// The grids corresponding to the steps.
		/// </param>
		/// <param name="additional">The additional message.</param>
		public void Deconstruct(
			out string solverName, out bool hasSolved, out decimal total, out decimal max, out decimal pearl,
			out decimal diamond, out SudokuGrid puzzle, out SudokuGrid? solution, out TimeSpan elasped,
			out int stepCount, out IReadOnlyList<StepInfo>? steps, out IReadOnlyList<SudokuGrid>? stepGrids,
			out object? additional)
		{
			solverName = SolverName;
			hasSolved = IsSolved;
			total = TotalDifficulty;
			max = MaxDifficulty;
			pearl = PearlDifficulty;
			diamond = DiamondDifficulty;
			puzzle = Puzzle;
			solution = Solution;
			elasped = ElapsedTime;
			stepCount = SolvingStepsCount;
			steps = Steps;
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
		public IEnumerator<StepInfo> GetEnumerator() => (Steps ?? Array.Empty<StepInfo>()).GetEnumerator();

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
			new Formatter(this).ToString(format, null, countryCode);

		/// <inheritdoc/>
		public string ToString(string? format, IFormatProvider? formatProvider) =>
			new Formatter(this).ToString(format, formatProvider);

		/// <inheritdoc cref="Formatter.ToString(AnalysisResultFormattingOptions)"/>
		public string ToString(AnalysisResultFormattingOptions options) =>
			new Formatter(this).ToString(options);

		/// <inheritdoc cref="Formatter.ToString(AnalysisResultFormattingOptions, CountryCode)"/>
		public string ToString(AnalysisResultFormattingOptions options, CountryCode countryCode) =>
			new Formatter(this).ToString(options, countryCode);

		/// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
