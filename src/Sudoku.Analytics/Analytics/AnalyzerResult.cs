namespace Sudoku.Analytics;

/// <summary>
/// Provides the result after <see cref="Analyzer"/> solving a puzzle.
/// </summary>
/// <param name="Puzzle"><inheritdoc cref="IAnalyzerResult{TSolver, TSolverResult}.Puzzle" path="/summary"/></param>
public sealed partial record AnalyzerResult(scoped ref readonly Grid Puzzle) :
	IAnalyzerResult<Analyzer, AnalyzerResult>,
	ICultureFormattable,
	IEnumerable<Step>
{
	/// <summary>
	/// Indicates the maximum rating value in theory.
	/// </summary>
	public const decimal MaximumRatingValueTheory = 20.0M;

	/// <summary>
	/// Indicates the maximum rating value in fact.
	/// </summary>
	public const decimal MaximumRatingValueFact = 12.0M;

	/// <summary>
	/// Indicates the minimum rating value.
	/// </summary>
	public const decimal MinimumRatingValue = 0;

	/// <summary>
	/// Indicates the default options.
	/// </summary>
	private const FormattingOptions DefaultOptions = FormattingOptions.ShowDifficulty
		| FormattingOptions.ShowSeparators
		| FormattingOptions.ShowStepsAfterBottleneck
		| FormattingOptions.ShowSteps
		| FormattingOptions.ShowGridAndSolutionCode
		| FormattingOptions.ShowElapsedTime;


	/// <inheritdoc/>
	[MemberNotNullWhen(true, nameof(Steps), nameof(SteppingGrids))]
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
			{ IsSolved: true, SolvingPath.PearlStep: { Difficulty: var ep } step } => ep == MaxDifficulty && step is not SingleStep,
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
			{ IsSolved: true, SolvingPath.PearlStep: { Difficulty: var ep } pStep, SolvingPath.DiamondStep: { Difficulty: var ed } dStep }
				=> ed == MaxDifficulty && ep == ed && (pStep, dStep) is (not SingleStep, not SingleStep),
			_ => null
		};

	/// <summary>
	/// Indicates the maximum difficulty of the puzzle.
	/// </summary>
	/// <remarks>
	/// When the puzzle is solved by <see cref="Analyzer"/>,
	/// the value will be the maximum value among all difficulty ratings in solving steps. If the puzzle has not been solved,
	/// or else the puzzle is solved by other solvers, this value will be always <c>20.0M</c>,
	/// equal to <see cref="MaximumRatingValueTheory"/>.
	/// </remarks>
	/// <seealso cref="Analyzer"/>
	/// <seealso cref="MaximumRatingValueTheory"/>
	public unsafe decimal MaxDifficulty => StepRatingHelper.EvaluateRatingUnsafe(Steps, &StepRatingHelper.MaxUnsafe, MaximumRatingValueTheory);

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
	public unsafe decimal TotalDifficulty => StepRatingHelper.EvaluateRatingUnsafe(Steps, &StepRatingHelper.SumUnsafe, MinimumRatingValue);

	/// <summary>
	/// Indicates the pearl difficulty rating of the puzzle, calculated during only by <see cref="Analyzer"/>.
	/// </summary>
	/// <remarks>
	/// When the puzzle is solved, the value will be the difficulty rating of the first delete step that cause a set.
	/// </remarks>
	/// <seealso cref="Analyzer"/>
	/// <seealso href="http://forum.enjoysudoku.com/the-hardest-sudokus-new-thread-t6539-690.html#p293738">Concept for EP, ER and ED</seealso>
	public decimal? PearlDifficulty => SolvingPath.PearlStep?.Difficulty;

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
	public decimal? DiamondDifficulty => SolvingPath.DiamondStep?.Difficulty;

	/// <summary>
	/// Indicates why the solving operation is failed.
	/// This property is meaningless when <see cref="IsSolved"/> keeps the <see langword="true"/> value.
	/// </summary>
	/// <seealso cref="IsSolved"/>
	public FailedReason FailedReason { get; init; }

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
	public Grid[]? SteppingGrids { get; init; }

	/// <summary>
	/// <para>Indicates a list of pairs of information about each step.</para>
	/// <para>
	/// If the puzzle cannot be solved due to some reason (invalid puzzle, unhandled exception, etc.),
	/// the return value of the property will be always <see langword="null"/>.
	/// </para>
	/// </summary>
	public SolvingPath SolvingPath => IsSolved ? new(SteppingGrids, Steps) : default;

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
	/// Indicates all solving steps that the solver has recorded.
	/// </summary>
	/// <seealso cref="SteppingGrids"/>
	public Step[]? Steps { get; init; }

	/// <summary>
	/// <inheritdoc cref="IAnalyzerResult{TSolver, TSolverResult}.UnhandledException" path="/summary"/>
	/// </summary>
	/// <remarks>
	/// You can visit the property value if the property <see cref="FailedReason"/>
	/// is <see cref="FailedReason.ExceptionThrown"/> or <see cref="FailedReason.WrongStep"/>.
	/// </remarks>
	/// <seealso cref="FailedReason"/>
	/// <seealso cref="FailedReason.ExceptionThrown"/>
	/// <seealso cref="FailedReason.WrongStep"/>
	public Exception? UnhandledException { get; init; }


	/// <inheritdoc/>
	/// <remarks>
	/// <b>This method only checks for initial grid puzzle.</b>
	/// This is by design: We only check for grids between two <see cref="AnalyzerResult"/> instances,
	/// because the target value will be same if the base grid are same.
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals([NotNullWhen(true)] AnalyzerResult? other) => other is not null && Puzzle == other.Puzzle;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => Puzzle.GetHashCode();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() => ToString(DefaultOptions, GlobalizedConverter.InvariantCultureConverter);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(CultureInfo? culture = null)
		=> ToString(culture is null ? GlobalizedConverter.InvariantCultureConverter : GlobalizedConverter.GetConverter(culture));

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(CoordinateConverter converter) => ToString(DefaultOptions, converter);

	/// <summary>
	/// Returns a string that represents the current object, with the specified formatting options.
	/// </summary>
	/// <param name="options">The formatting options.</param>
	/// <param name="converter">The converter to be used.</param>
	/// <returns>A string that represents the current object.</returns>
	public string ToString(FormattingOptions options, CoordinateConverter converter)
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
				Steps: { Length: var stepsCount } steps
			})
		{
			throw new();
		}

		var culture = converter.CurrentCulture ?? CultureInfo.CurrentUICulture;

		// Print header.
		scoped var sb = new StringHandler();
		if (options.HasFlag(FormattingOptions.ShowGridAndSolutionCode))
		{
			sb.Append(ResourceDictionary.Get("AnalysisResultPuzzle", culture));
			sb.Append(puzzle.ToString("#"));
			sb.AppendLine();
		}

		// Print solving steps (if worth).
		if (options.HasFlag(FormattingOptions.ShowSteps) && steps is not null)
		{
			sb.Append(ResourceDictionary.Get("AnalysisResultSolvingSteps", culture));
			sb.AppendLine();

			if (getBottleneck() is var (bIndex, bInfo))
			{
				for (var i = 0; i < steps.Length; i++)
				{
					if (i > bIndex && !options.HasFlag(FormattingOptions.ShowStepsAfterBottleneck))
					{
						sb.Append(ResourceDictionary.Get("Ellipsis", culture));
						sb.AppendLine();

						break;
					}

					var info = steps[i];
					var infoStr = options.HasFlag(FormattingOptions.ShowSimple) ? info.ToSimpleString(culture) : info.ToString(culture);
					var showDiff = options.HasFlag(FormattingOptions.ShowDifficulty);

					var d = $"({info.Difficulty,5:0.0}";
					var s = $"{i + 1,4}";
					var labelInfo = (options.HasFlag(FormattingOptions.ShowStepLabel), showDiff) switch
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

				if (options.HasFlag(FormattingOptions.ShowBottleneck))
				{
					a(ref sb, options.HasFlag(FormattingOptions.ShowSeparators));

					sb.Append(ResourceDictionary.Get("AnalysisResultBottleneckStep", culture));

					if (options.HasFlag(FormattingOptions.ShowStepLabel))
					{
						sb.Append(ResourceDictionary.Get("AnalysisResultInStep", culture));
						sb.Append(bIndex + 1);
						sb.Append(ResourceDictionary.Get("Colon", culture));
					}

					sb.Append(' ');
					sb.Append(bInfo);
					sb.AppendLine();
				}

				a(ref sb, options.HasFlag(FormattingOptions.ShowSeparators));
			}
		}

		// Print solving step statistics (if worth).
		if (steps is not null)
		{
			sb.Append(ResourceDictionary.Get("AnalysisResultTechniqueUsed", culture));
			sb.AppendLine();

			if (options.HasFlag(FormattingOptions.ShowStepDetail))
			{
				sb.Append(ResourceDictionary.Get("AnalysisResultMin", culture), 6);
				sb.Append(',');
				sb.Append(' ');
				sb.Append(ResourceDictionary.Get("AnalysisResultTotal", culture), 6);
				sb.Append(ResourceDictionary.Get("AnalysisResultTechniqueUsing", culture));
			}

			foreach (var solvingStepsGroup in from s in steps orderby s.Difficulty group s by s.Name)
			{
				if (options.HasFlag(FormattingOptions.ShowStepDetail))
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

			if (options.HasFlag(FormattingOptions.ShowStepDetail))
			{
				sb.Append("  (---");
				sb.Append(total, 8);
				sb.Append(')');
				sb.Append(' ');
			}

			sb.Append(stepsCount, 3);
			sb.Append(' ');
			sb.Append(ResourceDictionary.Get(stepsCount == 1 ? "AnalysisResultStepSingular" : "AnalysisResultStepPlural", culture));
			sb.AppendLine();

			a(ref sb, options.HasFlag(FormattingOptions.ShowSeparators));
		}

		// Print detail data.
		sb.Append(ResourceDictionary.Get("AnalysisResultPuzzleRating", culture));
		sb.Append(max, "0.0");
		sb.Append('/');
		sb.Append(pearl ?? MaximumRatingValueTheory, "0.0");
		sb.Append('/');
		sb.Append(diamond ?? MaximumRatingValueTheory, "0.0");
		sb.AppendLine();

		// Print the solution (if not null and worth).
		if (!solution.IsUndefined && options.HasFlag(FormattingOptions.ShowGridAndSolutionCode))
		{
			sb.Append(ResourceDictionary.Get("AnalysisResultPuzzleSolution", culture));
			sb.Append($"{solution:!}");
			sb.AppendLine();
		}

		// Print the elapsed time.
		sb.Append(ResourceDictionary.Get("AnalysisResultPuzzleHas", culture));
		if (!isSolved)
		{
			sb.Append(ResourceDictionary.Get("AnalysisResultNot", culture));
		}
		sb.Append(ResourceDictionary.Get("AnalysisResultBeenSolved", culture));
		sb.AppendLine();
		if (options.HasFlag(FormattingOptions.ShowElapsedTime))
		{
			sb.Append(ResourceDictionary.Get("AnalysisResultTimeElapsed", culture));
			sb.Append($@"{elapsed:hh\:mm\:ss\.fff}");
			sb.AppendLine();
		}

		a(ref sb, options.HasFlag(FormattingOptions.ShowSeparators));
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

		(int, Step)? getBottleneck()
		{
			if (this is not { IsSolved: true, Steps: { Length: var stepsCount } steps })
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
	public ReadOnlySpan<Step>.Enumerator GetEnumerator() => SolvingPath.Steps.GetEnumerator();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator IEnumerable.GetEnumerator() => SolvingPath.Steps.ToArray().GetEnumerator();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator<Step> IEnumerable<Step>.GetEnumerator() => ((IEnumerable<Step>)SolvingPath.Steps.ToArray()).GetEnumerator();
}
