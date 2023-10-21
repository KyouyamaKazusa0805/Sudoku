using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Metadata;
using Sudoku.Analytics.Steps;
using Sudoku.Concepts;
using static Sudoku.Analytics.Strings.StringsAccessor;

namespace Sudoku.Analytics;

/// <summary>
/// Provides the result after <see cref="Analyzer"/> solving a puzzle.
/// </summary>
/// <param name="Puzzle"><inheritdoc cref="IAnalyzerResult{TSolver, TSolverResult}.Puzzle" path="/summary"/></param>
public sealed partial record AnalyzerResult(scoped ref readonly Grid Puzzle) : IAnalyzerResult<Analyzer, AnalyzerResult>, IEnumerable<Step>
{
	/// <inheritdoc/>
	[MemberNotNullWhen(true, nameof(Steps), nameof(StepGrids), nameof(SolvingPath), nameof(PearlStep), nameof(DiamondStep))]
	public required bool IsSolved { get; init; }

	/// <summary>
	/// Indicates whether the puzzle is a pearl puzzle, which means the first step must be an indirect technique usage.
	/// </summary>
	/// <returns>
	/// Returns a <see cref="bool"/>? value indicating the result. The values are:
	/// <list type="table">
	/// <listheader>
	/// <term>Value</term>
	/// <description>Meaning</description>
	/// </listheader>
	/// <item>
	/// <term><see langword="true"/></term>
	/// <description>The puzzle has a unique solution, and the first set step has same difficulty with the whole steps.</description>
	/// </item>
	/// <item>
	/// <term><see langword="false"/></term>
	/// <description>The puzzle has a unique solution, but the first set step does not have same difficulty with the whole steps.</description>
	/// </item>
	/// <item>
	/// <term><see langword="null"/></term>
	/// <description>The puzzle has multiple solutions, or the puzzle has no valid solution.</description>
	/// </item>
	/// </list>
	/// </returns>
	public bool? IsPearl
		=> this switch
		{
			{ IsSolved: true, PearlStep: { Difficulty: var ep } step } => ep == MaxDifficulty && step is not SingleStep,
			_ => null
		};

	/// <summary>
	/// Indicates whether the puzzle is a diamond puzzle, which means the first deletion has the same difficulty
	/// with the maximum difficulty of the whole steps.
	/// </summary>
	/// <returns>
	/// Returns a <see cref="bool"/>? value indicating the result. The values are:
	/// <list type="table">
	/// <listheader>
	/// <term>Value</term>
	/// <description>Meaning</description>
	/// </listheader>
	/// <item>
	/// <term><see langword="true"/></term>
	/// <description>The puzzle has a unique solution, and the first deletion step has same difficulty with the whole steps.</description>
	/// </item>
	/// <item>
	/// <term><see langword="false"/></term>
	/// <description>The puzzle has a unique solution, but the first deletion step does not have same difficulty with the whole steps.</description>
	/// </item>
	/// <item>
	/// <term><see langword="null"/></term>
	/// <description>The puzzle has multiple solutions, or the puzzle has no valid solution.</description>
	/// </item>
	/// </list>
	/// </returns>
	public bool? IsDiamond
		=> this switch
		{
			{ IsSolved: true, PearlStep: { Difficulty: var ep } pStep, DiamondStep: { Difficulty: var ed } dStep }
				=> ed == MaxDifficulty && ep == ed && (pStep, dStep) is (not SingleStep, not SingleStep),
			_ => null
		};

	/// <summary>
	/// Indicates the maximum difficulty of the puzzle.
	/// </summary>
	/// <remarks>
	/// When the puzzle is solved by <see cref="Analyzer"/>,
	/// the value will be the maximum value among all difficulty ratings in solving steps. If the puzzle has not been solved,
	/// or else the puzzle is solved by other solvers, this value will be always <c>20.0M</c>.
	/// </remarks>
	/// <seealso cref="Analyzer"/>
	public unsafe decimal MaxDifficulty => Evaluator(&Max<Step>, 20.0M);

	/// <summary>
	/// Indicates the total difficulty rating of the puzzle.
	/// </summary>
	/// <remarks>
	/// When the puzzle is solved by <see cref="Analyzer"/>, the value will be the sum of all difficulty ratings of steps.
	/// If the puzzle has not been solved, the value will be the sum of all difficulty ratings of steps recorded in <see cref="Steps"/>.
	/// However, if the puzzle is solved by other solvers, this value will be <c>0</c>.
	/// </remarks>
	/// <seealso cref="Analyzer"/>
	/// <seealso cref="Steps"/>
	public unsafe decimal TotalDifficulty => Evaluator(&Sum<Step>, 0);

	/// <summary>
	/// Indicates the pearl difficulty rating of the puzzle, calculated during only by <see cref="Analyzer"/>.
	/// </summary>
	/// <remarks>
	/// When the puzzle is solved, the value will be the difficulty rating of the first delete step that cause a set.
	/// </remarks>
	/// <seealso cref="Analyzer"/>
	/// <seealso href="http://forum.enjoysudoku.com/the-hardest-sudokus-new-thread-t6539-690.html#p293738">Concept for EP, ER and ED</seealso>
	[NotNullIfNotNull(nameof(PearlStep))]
	public decimal? PearlDifficulty => PearlStep?.Difficulty;

	/// <summary>
	/// <para>
	/// Indicates the pearl difficulty rating of the puzzle, calculated during only by <see cref="Analyzer"/>.
	/// </para>
	/// <para>
	/// When the puzzle is solved, the value will be the difficulty rating of the first delete step.
	/// </para>
	/// </summary>
	/// <seealso cref="Analyzer"/>
	/// <seealso href="http://forum.enjoysudoku.com/the-hardest-sudokus-new-thread-t6539-690.html#p293738">Concept for EP, ER and ED</seealso>
	[NotNullIfNotNull(nameof(DiamondStep))]
	public decimal? DiamondDifficulty => DiamondStep?.Difficulty;

	/// <summary>
	/// Indicates the number of all solving steps recorded.
	/// </summary>
	public Count SolvingStepsCount => Steps?.Length ?? 1;

	/// <summary>
	/// Indicates why the solving operation is failed.
	/// This property is meaningless when <see cref="IsSolved"/> keeps the <see langword="true"/> value.
	/// </summary>
	/// <seealso cref="IsSolved"/>
	public AnalyzerFailedReason FailedReason { get; init; }

	/// <summary>
	/// Indicates the difficulty level of the puzzle.
	/// If the puzzle has not solved or solved by other solvers, this value will be <see cref="DifficultyLevel.Unknown"/>.
	/// </summary>
	public DifficultyLevel DifficultyLevel
	{
		get
		{
			var maxLevel = DifficultyLevel.Unknown;
			if (IsSolved)
			{
				foreach (var step in Steps)
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

	/// <inheritdoc/>
	public Grid Solution { get; init; }

	/// <inheritdoc/>
	public TimeSpan ElapsedTime { get; init; }

	/// <summary>
	/// Indicates a list, whose element is the intermediate grid for each step.
	/// </summary>
	/// <seealso cref="Steps"/>
	public Grid[]? StepGrids { get; init; }

	/// <summary>
	/// <para>
	/// Indicates the wrong step found. In general cases, if the property <see cref="IsSolved"/> keeps
	/// <see langword="false"/> value, it'll mean the puzzle is invalid to solve, or the solver has found
	/// one error step to apply, that causes the original puzzle <see cref="Puzzle"/> become invalid.
	/// In this case we can check this property to get the wrong information to debug the error,
	/// or tell the author himself directly, with the inner value of this property held.
	/// </para>
	/// <para>
	/// However, if the puzzle is successful to be solved, the property won't contain any value,
	/// so it'll keep the <see langword="null"/> reference. Therefore, please check the nullability
	/// of this property before using.
	/// </para>
	/// <para>
	/// In general, this table will tell us the nullability of this property:
	/// <list type="table">
	/// <listheader>
	/// <term>Nullability</term>
	/// <description>Description</description>
	/// </listheader>
	/// <item>
	/// <term>Not <see langword="null"/></term>
	/// <description>The puzzle is failed to solve, and the solver has found an invalid step to apply.</description>
	/// </item>
	/// <item>
	/// <term><see langword="null"/></term>
	/// <description>Other cases.</description>
	/// </item>
	/// </list>
	/// </para>
	/// </summary>
	/// <seealso cref="IsSolved"/>
	/// <seealso cref="Puzzle"/>
	public Step? WrongStep => (UnhandledException as WrongStepException)?.WrongStep;

	/// <summary>
	/// Gets the bottleneck during the whole grid solving. Returns <see langword="null"/> if the property
	/// <see cref="Steps"/> is default case (not initialized or empty).
	/// </summary>
	/// <seealso cref="Steps"/>
	public Step? Bottleneck
	{
		get
		{
			switch (Steps)
			{
				default:
				{
					return null;
				}
				case [var firstStep, ..]:
				{
					foreach (var step in Steps.EnumerateReversely())
					{
						if (step is not SingleStep)
						{
							return step;
						}
					}

					// If code goes to here, all steps are more difficult than single techniques. Get the first one.
					return firstStep;
				}
			}
		}
	}

	/// <summary>
	/// Indicates the pearl step.
	/// </summary>
	public Step? PearlStep
	{
		get
		{
			if (!IsSolved)
			{
				return null;
			}

			for (var i = 0; i < Steps.Length; i++)
			{
				if (Steps[i] is SingleStep)
				{
					return i < 1
						? Steps[0]
						: (from step in Steps[..i] select (Step: step, step.Difficulty)).MaxBy(static pair => pair.Difficulty).Step;
				}
			}

			throw new InvalidOperationException("The puzzle keeps a wrong state.");
		}
	}

	/// <summary>
	/// Indicates the diamond step.
	/// </summary>
	public Step? DiamondStep
	{
		get
		{
			if (!IsSolved)
			{
				return null;
			}

			foreach (var step in Steps)
			{
				if (step is not SingleStep)
				{
					return step;
				}
			}

			throw new InvalidOperationException("The puzzle keeps a wrong state.");
		}
	}

	/// <summary>
	/// Indicates all solving steps that the solver has recorded.
	/// </summary>
	/// <seealso cref="StepGrids"/>
	public Step[]? Steps { get; init; }

	/// <summary>
	/// <inheritdoc cref="IAnalyzerResult{TSolver, TSolverResult}.UnhandledException" path="/summary"/>
	/// </summary>
	/// <remarks>
	/// You can visit the property value if the property <see cref="FailedReason"/>
	/// is <see cref="AnalyzerFailedReason.ExceptionThrown"/> or <see cref="AnalyzerFailedReason.WrongStep"/>.
	/// </remarks>
	/// <seealso cref="FailedReason"/>
	/// <seealso cref="AnalyzerFailedReason.ExceptionThrown"/>
	/// <seealso cref="AnalyzerFailedReason.WrongStep"/>
	public Exception? UnhandledException { get; init; }

	/// <summary>
	/// <para>Indicates a list of pairs of information about each step.</para>
	/// <para>
	/// If the puzzle cannot be solved due to some reason (invalid puzzle, unhandled exception, etc.),
	/// the return value of the property will be always <see langword="null"/>.
	/// </para>
	/// </summary>
	public (Grid SteppingGrid, Step Step)[]? SolvingPath => IsSolved ? StepGrids.Zip(Steps) : null;


	/// <summary>
	/// Gets the <see cref="Step"/> instance at the specified index.
	/// </summary>
	/// <param name="index">The index.</param>
	/// <returns>The step information.</returns>
	/// <exception cref="InvalidOperationException">Throws when the puzzle is not solved.</exception>
	public Step this[Offset index]
		=> IsSolved ? Steps[index] : throw new InvalidOperationException("The puzzle must have been solved before you use this indexer.");

	/// <summary>
	/// Gets the found <see cref="Step"/> instance whose corresponding candidates are same with the specified argument <paramref name="grid"/>.
	/// </summary>
	/// <param name="grid">The grid to be matched.</param>
	/// <returns>The found <see cref="Step"/> instance.</returns>
	/// <exception cref="InvalidOperationException">
	/// Throws when the puzzle is not solved (i.e. <see cref="IsSolved"/> property returns <see langword="false"/>).
	/// </exception>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the specified puzzle cannot correspond to a paired <see cref="Step"/> instance.
	/// </exception>
	public Step this[scoped ref readonly Grid grid]
	{
		get
		{
			if (!IsSolved)
			{
				throw new InvalidOperationException("The puzzle must have been solved before you use this indexer.");
			}

			foreach (var (g, s) in SolvingPath)
			{
				if (g == grid)
				{
					return s;
				}
			}

			throw new ArgumentOutOfRangeException("The specified step is not found.");
		}
	}

	/// <summary>
	/// Gets the first found <see cref="Step"/> whose name is specified one, or nearly same as the specified one.
	/// </summary>
	/// <param name="techniqueName">Technique name.</param>
	/// <returns>The first found step.</returns>
	public (Grid StepGrid, Step Step)? this[string techniqueName]
	{
		get
		{
			if (!IsSolved)
			{
				return null;
			}

			foreach (var pair in SolvingPath)
			{
				var (_, step) = pair;

				var name = step.Name;
				if (nameEquality(name))
				{
					return pair;
				}

				var aliases = step.Code.GetAliases();
				if (aliases is not null && Array.Exists(aliases, nameEquality))
				{
					return pair;
				}

				var abbr = step.Code.GetAbbreviation();
				if (abbr is not null && nameEquality(abbr))
				{
					return pair;
				}
			}
			return null;


			bool nameEquality(string name) => name == techniqueName || name.Contains(techniqueName, StringComparison.OrdinalIgnoreCase);
		}
	}

	/// <summary>
	/// Gets a list of <see cref="Step"/>s that has the same difficulty rating value as argument <paramref name="difficultyRating"/>. 
	/// </summary>
	/// <param name="difficultyRating">The specified difficulty rating value.</param>
	/// <returns>
	/// A list of <see cref="Step"/>s found. If the puzzle cannot be solved (i.e. <see cref="IsSolved"/> returns <see langword="false"/>),
	/// the return value will be <see langword="null"/>. If the puzzle is solved, but the specified value is not found,
	/// the return value will be an empty array, rather than <see langword="null"/>. The nullability of the return value
	/// only depends on property <see cref="IsSolved"/>.
	/// </returns>
	/// <seealso cref="IsSolved"/>
	public Step[]? this[decimal difficultyRating] => IsSolved ? Array.FindAll(Steps, step => step.Difficulty == difficultyRating) : null;

	/// <summary>
	/// Gets a list of <see cref="Step"/>s that matches the specified technique.
	/// </summary>
	/// <param name="code">The specified technique code.</param>
	/// <returns>
	/// <inheritdoc cref="this[decimal]" path="/returns"/>
	/// </returns>
	/// <seealso cref="IsSolved"/>
	public Step[]? this[Technique code] => IsSolved ? Array.FindAll(Steps, step => step.Code == code) : null;

	/// <summary>
	/// Gets a list of <see cref="Step"/>s that has the same difficulty level as argument <paramref name="difficultyLevel"/>. 
	/// </summary>
	/// <param name="difficultyLevel">The specified difficulty level.</param>
	/// <returns>
	/// <inheritdoc cref="this[decimal]" path="/returns"/>
	/// </returns>
	/// <seealso cref="IsSolved"/>
	public Step[]? this[DifficultyLevel difficultyLevel]
		=> IsSolved ? Array.FindAll(Steps, step => step.DifficultyLevel == difficultyLevel) : null;


	/// <summary>
	/// Determine whether the analyzer result instance contains any step with specified technique.
	/// </summary>
	/// <param name="technique">The technique you want to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	/// <exception cref="InvalidOperationException">Throws when the puzzle has not been solved.</exception>
	public bool HasTechnique(Technique technique)
	{
		if (!IsSolved)
		{
			throw new InvalidOperationException("The puzzle must be solved before call this method.");
		}

		foreach (var step in Steps)
		{
			if (step.Code == technique)
			{
				return true;
			}
		}

		return false;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString()
	{
		var flags = FormattingOptions.None;
		flags |= FormattingOptions.ShowSeparators;
		flags |= FormattingOptions.ShowDifficulty;
		flags |= FormattingOptions.ShowStepsAfterBottleneck;
		flags |= FormattingOptions.ShowSteps;
		flags |= FormattingOptions.ShowGridAndSolutionCode;
		flags |= FormattingOptions.ShowElapsedTime;

		return ToString(flags);
	}

	/// <summary>
	/// Returns a string that represents the current object, with the specified formatting options.
	/// </summary>
	/// <param name="options">The formatting options.</param>
	/// <returns>A string that represents the current object.</returns>
	public string ToString(FormattingOptions options)
	{
		if (this is not
			{
				IsSolved: var isSolved,
				TotalDifficulty: var total,
				MaxDifficulty: var max,
				PearlDifficulty: var pearl,
				DiamondDifficulty: var diamond,
				Puzzle: var puzzle,
				Solution: var solution,
				ElapsedTime: var elapsed,
				SolvingStepsCount: var stepsCount,
				Steps: var steps
			})
		{
			throw new();
		}

		// Print header.
		scoped var sb = new StringHandler();
		if (options.Flags(FormattingOptions.ShowGridAndSolutionCode))
		{
			sb.Append(GetString("AnalysisResultPuzzle")!);
			sb.Append(puzzle.ToString("#"));
			sb.AppendLine();
		}

		// Print solving steps (if worth).
		if (options.Flags(FormattingOptions.ShowSteps) && steps is not null)
		{
			sb.Append(GetString("AnalysisResultSolvingSteps")!);
			sb.AppendLine();

			if (getBottleneck() is var (bIndex, bInfo))
			{
				for (var i = 0; i < steps.Length; i++)
				{
					if (i > bIndex && !options.Flags(FormattingOptions.ShowStepsAfterBottleneck))
					{
						sb.Append(GetString("Ellipsis")!);
						sb.AppendLine();

						break;
					}

					var info = steps[i];
					var infoStr = options.Flags(FormattingOptions.ShowSimple) ? info.ToSimpleString() : info.ToString();
					var showDiff = options.Flags(FormattingOptions.ShowDifficulty);

					var d = $"({info.Difficulty,5:0.0}";
					var s = $"{i + 1,4}";
					var labelInfo = (options.Flags(FormattingOptions.ShowStepLabel), showDiff) switch
					{
						(true, true) => $"{s}, {d}) ",
						(true, false) => $"{s} ",
						(false, true) => $"{d}) ",
						_ => string.Empty
					};

					sb.Append(labelInfo);
					sb.Append(infoStr);
					sb.AppendLine();
				}

				if (options.Flags(FormattingOptions.ShowBottleneck))
				{
					a(ref sb, options.Flags(FormattingOptions.ShowSeparators));

					sb.Append(GetString("AnalysisResultBottleneckStep")!);

					if (options.Flags(FormattingOptions.ShowStepLabel))
					{
						sb.Append(GetString("AnalysisResultInStep")!);
						sb.Append(bIndex + 1);
						sb.Append(GetString("Colon")!);
					}

					sb.Append(' ');
					sb.Append(bInfo);
					sb.AppendLine();
				}

				a(ref sb, options.Flags(FormattingOptions.ShowSeparators));
			}
		}

		// Print solving step statistics (if worth).
		if (steps is not null)
		{
			sb.Append(GetString("AnalysisResultTechniqueUsed")!);
			sb.AppendLine();

			if (options.Flags(FormattingOptions.ShowStepDetail))
			{
				sb.Append(GetString("AnalysisResultMin")!, 6);
				sb.Append(',');
				sb.Append(' ');
				sb.Append(GetString("AnalysisResultTotal")!, 6);
				sb.Append(GetString("AnalysisResultTechniqueUsing")!);
			}

			foreach (var solvingStepsGroup in from s in steps orderby s.Difficulty group s by s.Name)
			{
				if (options.Flags(FormattingOptions.ShowStepDetail))
				{
					var currentTotal = 0M;
					var currentMinimum = decimal.MaxValue;
					foreach (var solvingStep in solvingStepsGroup)
					{
						var difficulty = solvingStep.Difficulty;
						currentTotal += difficulty;
						currentMinimum = Math.Min(currentMinimum, difficulty);
					}

					sb.Append(currentMinimum, 6, "0.0");
					sb.Append(',');
					sb.Append(' ');
					sb.Append(currentTotal, 6, "0.0");
					sb.Append(')');
					sb.Append(' ');
				}

				sb.Append(solvingStepsGroup.Count(), 3);
				sb.Append(" * ");
				sb.Append(solvingStepsGroup.Key);
				sb.AppendLine();
			}

			if (options.Flags(FormattingOptions.ShowStepDetail))
			{
				sb.Append("  (---");
				sb.Append(total, 8);
				sb.Append(')');
				sb.Append(' ');
			}

			sb.Append(stepsCount, 3);
			sb.Append(' ');
			sb.Append(GetString(stepsCount == 1 ? "AnalysisResultStepSingular" : "AnalysisResultStepPlural")!);
			sb.AppendLine();

			a(ref sb, options.Flags(FormattingOptions.ShowSeparators));
		}

		// Print detail data.
		sb.Append(GetString("AnalysisResultPuzzleRating")!);
		sb.Append(max, "0.0");
		sb.Append('/');
		sb.Append(pearl ?? 20.0M, "0.0");
		sb.Append('/');
		sb.Append(diamond ?? 20.0M, "0.0");
		sb.AppendLine();

		// Print the solution (if not null and worth).
		if (!solution.IsUndefined && options.Flags(FormattingOptions.ShowGridAndSolutionCode))
		{
			sb.Append(GetString("AnalysisResultPuzzleSolution")!);
			sb.Append(solution.ToString("!"));
			sb.AppendLine();
		}

		// Print the elapsed time.
		sb.Append(GetString("AnalysisResultPuzzleHas")!);
		if (!isSolved)
		{
			sb.Append(GetString("AnalysisResultNot")!);
		}
		sb.Append(GetString("AnalysisResultBeenSolved")!);
		sb.AppendLine();
		if (options.Flags(FormattingOptions.ShowElapsedTime))
		{
			sb.Append(GetString("AnalysisResultTimeElapsed")!);
			sb.Append(elapsed.ToString("""hh\:mm\:ss\.fff"""));
			sb.AppendLine();
		}

		a(ref sb, options.Flags(FormattingOptions.ShowSeparators));
		return sb.ToStringAndClear();


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void a(scoped ref StringHandler sb, bool showSeparator)
		{
			if (showSeparator)
			{
				sb.Append('-', 10);
				sb.AppendLine();
			}
		}

		(Offset, Step)? getBottleneck()
		{
			if (this is not { IsSolved: true, Steps: var steps, SolvingStepsCount: var stepsCount })
			{
				return null;
			}

			for (var i = stepsCount - 1; i >= 0; i--)
			{
				if (steps[i] is var step and not SingleStep)
				{
					return (i, step);
				}
			}

			// If code goes to here, all steps are more difficult than single techniques.
			// Get the first one is okay.
			return (0, steps[0]);
		}
	}

	/// <summary>
	/// Gets the enumerator of the current instance in order to use <see langword="foreach"/> loop.
	/// </summary>
	/// <returns>The enumerator instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ReadOnlySpan<Step>.Enumerator GetEnumerator() => ((ReadOnlySpan<Step>)(Steps ?? [])).GetEnumerator();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator IEnumerable.GetEnumerator() => (Steps ?? []).GetEnumerator();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator<Step> IEnumerable<Step>.GetEnumerator() => ((IEnumerable<Step>)(Steps ?? [])).GetEnumerator();

	/// <summary>
	/// The inner executor to get the difficulty value (total, average).
	/// </summary>
	/// <param name="executor">The execute method.</param>
	/// <param name="d">
	/// The default value as the return value when <see cref="Steps"/> is <see langword="null"/> or empty.
	/// </param>
	/// <returns>The result.</returns>
	/// <seealso cref="Steps"/>
	private unsafe decimal Evaluator(delegate*<IEnumerable<Step>, delegate*<Step, decimal>, decimal> executor, decimal d)
	{
		static decimal f(Step step) => step.Difficulty;
		return Steps is null ? d : executor(Steps, &f);
	}


	/// <inheritdoc cref="Enumerable.Max(IEnumerable{decimal})"/>
	private static unsafe decimal Max<T>(IEnumerable<T> collection, delegate*<T, decimal> selector)
	{
		var result = decimal.MinValue;
		foreach (var element in collection)
		{
			var converted = selector(element);
			if (converted >= result)
			{
				result = converted;
			}
		}

		return result;
	}

	/// <inheritdoc cref="Enumerable.Sum{TSource}(IEnumerable{TSource}, Func{TSource, decimal})"/>
	private static unsafe decimal Sum<T>(IEnumerable<T> collection, delegate*<T, decimal> selector)
	{
		var result = 0M;
		foreach (var element in collection)
		{
			result += selector(element);
		}

		return result;
	}
}
