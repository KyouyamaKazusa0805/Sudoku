namespace Sudoku.Analytics;

using static Helper;

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
	[MemberNotNullWhen(true, nameof(InterimSteps), nameof(InterimGrids), nameof(BottleneckSteps), nameof(PearlStep), nameof(DiamondStep))]
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
	/// or else the puzzle is solved by other solvers, this value will be always <c>20.0M</c>,
	/// equal to <see cref="MaximumRatingValueTheory"/>.
	/// </remarks>
	/// <seealso cref="Analyzer"/>
	/// <seealso cref="MaximumRatingValueTheory"/>
	public unsafe decimal MaxDifficulty => EvaluateRatingUnsafe(InterimSteps, &MaxUnsafe, MaximumRatingValueTheory);

	/// <summary>
	/// Indicates the total difficulty rating of the puzzle.
	/// </summary>
	/// <remarks>
	/// When the puzzle is solved by <see cref="Analyzer"/>, the value will be the sum of all difficulty ratings of steps.
	/// If the puzzle has not been solved, the value will be the sum of all difficulty ratings of steps recorded in <see cref="InterimSteps"/>.
	/// However, if the puzzle is solved by other solvers, this value will be <c>0</c>.
	/// </remarks>
	/// <seealso cref="Analyzer"/>
	/// <seealso cref="InterimSteps"/>
	public unsafe decimal TotalDifficulty => EvaluateRatingUnsafe(InterimSteps, &SumUnsafe, MinimumRatingValue);

	/// <summary>
	/// Indicates the pearl difficulty rating of the puzzle, calculated during only by <see cref="Analyzer"/>.
	/// </summary>
	/// <remarks>
	/// When the puzzle is solved, the value will be the difficulty rating of the first delete step that cause a set.
	/// </remarks>
	/// <seealso cref="Analyzer"/>
	/// <seealso href="http://forum.enjoysudoku.com/the-hardest-sudokus-new-thread-t6539-690.html#p293738">Concept for EP, ER and ED</seealso>
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
	public decimal? DiamondDifficulty => DiamondStep?.Difficulty;

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
				foreach (var step in InterimSteps)
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
	/// <seealso cref="InterimSteps"/>
	public Grid[]? InterimGrids { get; init; }

	/// <summary>
	/// Returns a <see cref="ReadOnlySpan{T}"/> of <see cref="Grid"/> instances,
	/// whose internal values come from <see cref="InterimGrids"/>.
	/// </summary>
	/// <seealso cref="InterimGrids"/>
	public ReadOnlySpan<Grid> GridsSpan => InterimGrids;

	/// <summary>
	/// Returns a <see cref="ReadOnlySpan{T}"/> of <see cref="Step"/> instances,
	/// whose internal values come from <see cref="InterimSteps"/>.
	/// </summary>
	/// <seealso cref="InterimSteps"/>
	public ReadOnlySpan<Step> StepsSpan => InterimSteps;

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
	/// Indicates the bottleneck steps.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The bottleneck steps will be considered as "hardest" ones,
	/// checking their difficulty rating (i.e. property <see cref="Step.Difficulty"/>) and difficulty level
	/// (i.e. <see cref="Step.DifficultyLevel"/>).
	/// </para>
	/// <para>
	/// The puzzle can contain multiple bottleneck steps. If multiple steps with same difficulty level and difficulty rating,
	/// they all will be considered as bottleneck steps.
	/// </para>
	/// </remarks>
	/// <seealso cref="Step.Difficulty"/>
	/// <seealso cref="Step.DifficultyLevel"/>
	public Step[]? BottleneckSteps
	{
		get
		{
			return this switch
			{
				{ IsSolved: true, DifficultyLevel: var difficultyLevel } => difficultyLevel switch
				{
					DifficultyLevel.Easy => bottleneckEasy(InterimSteps),
					_ => bottleneckNotEasy(InterimSteps)
				},
				_ => null
			};


			static Step[] bottleneckEasy(Step[] steps)
			{
				var maxStep = default(Step);
				foreach (var step in steps)
				{
					// If the puzzle only contains hidden single and full house, we will consider this puzzle has no bottleneck.
					// Otherwise, the hardest technique used is the bottleneck.
					if (step.Difficulty >= (maxStep?.Difficulty ?? 0) && step is not (FullHouseStep or HiddenSingleStep))
					{
						maxStep = step;
					}
				}

				// Checks whether 'maxStep' is null or not. If not null, a step that is neither full house nor hidden single.
				return maxStep is not null ? Array.FindAll(steps, s => s.Code == maxStep.Code) : [];
			}

			static Step[] bottleneckNotEasy(Step[] steps)
			{
				var maxStep = steps.MaxBy(static step => step.Difficulty);
				return maxStep is not null ? Array.FindAll(steps, s => s.Difficulty == maxStep.Difficulty) : [];
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

			for (var i = 0; i < InterimSteps.Length; i++)
			{
				if (InterimSteps[i] is SingleStep)
				{
					static decimal keySelector((Step, decimal Difficulty) pair) => pair.Difficulty;
					return i < 1 ? InterimSteps[0] : (from step in InterimSteps[..i] select (Step: step, step.Difficulty)).MaxBy(keySelector).Step;
				}
			}

			throw new InvalidOperationException(ResourceDictionary.ExceptionMessage("GridInvalid"));
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

			if (StepsSpan.All(static (scoped ref readonly Step step) => step is FullHouseStep or HiddenSingleStep { House: < 9 }))
			{
				// No diamond step exist in all steps are hidden singles in block.
				return null;
			}

			if (StepsSpan.AllAre<Step, SingleStep>())
			{
				// If a puzzle can be solved using only singles, just check for the first step not hidden single in block.
				foreach (var step in InterimSteps)
				{
					if (step is not HiddenSingleStep { House: < 9 })
					{
						return step;
					}
				}
			}
			else
			{
				// Otherwise, an deletion step should be chosen.
				foreach (var step in InterimSteps)
				{
					if (step is not SingleStep)
					{
						return step;
					}
				}
			}

			return null;
		}
	}

	/// <summary>
	/// Indicates all solving steps that the solver has recorded.
	/// </summary>
	/// <seealso cref="InterimGrids"/>
	public Step[]? InterimSteps { get; init; }

	/// <summary>
	/// Indicates the techniques used during the solving operation.
	/// </summary>
	public TechniqueSet TechniquesUsed => [.. from step in StepsSpan select step.Code];

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

	/// <summary>
	/// Determine whether the analyzer result instance contains any step with specified technique.
	/// </summary>
	/// <param name="technique">The technique you want to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	/// <exception cref="InvalidOperationException">Throws when the puzzle has not been solved.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool HasTechnique(Technique technique) => TechniquesUsed.Contains(technique);

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
				InterimSteps: { Length: var stepsCount } steps
			})
		{
			throw new();
		}

		var culture = converter.CurrentCulture ?? CultureInfo.CurrentUICulture;

		// Print header.
		var sb = new StringBuilder();
		if (options.HasFlag(FormattingOptions.ShowGridAndSolutionCode))
		{
			sb.AppendLine($"{ResourceDictionary.Get("AnalysisResultPuzzle", culture)}{puzzle:#}");
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
						sb.AppendLine(ResourceDictionary.Get("Ellipsis", culture));
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
					a(sb, options.HasFlag(FormattingOptions.ShowSeparators));

					sb.Append(ResourceDictionary.Get("AnalysisResultBottleneckStep", culture));

					if (options.HasFlag(FormattingOptions.ShowStepLabel))
					{
						sb.Append(ResourceDictionary.Get("AnalysisResultInStep", culture));
						sb.Append(bIndex + 1);
						sb.Append(ResourceDictionary.Get("Colon", culture));
					}

					sb.Append(' ');
					sb.AppendValue(bInfo);
					sb.AppendLine();
				}

				a(sb, options.HasFlag(FormattingOptions.ShowSeparators));
			}
		}

		// Print solving step statistics (if worth).
		if (steps is not null)
		{
			sb.AppendLine(ResourceDictionary.Get("AnalysisResultTechniqueUsed", culture));

			if (options.HasFlag(FormattingOptions.ShowStepDetail))
			{
				sb
					.Append($"{ResourceDictionary.Get("AnalysisResultMin", culture),6}, ")
					.Append($"{ResourceDictionary.Get("AnalysisResultTotal", culture),6}")
					.Append(ResourceDictionary.Get("AnalysisResultTechniqueUsing", culture));
			}

			foreach (var solvingStepsGroup in from s in steps.AsReadOnlySpan() orderby s.Difficulty group s by s.GetName())
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

					sb.Append($"{currentMinimum,6:0.0}, {currentTotal,6:0.0}) ");
				}

				sb.AppendLine($"{solvingStepsGroup.Length,3} * {solvingStepsGroup.Key}");
			}

			if (options.HasFlag(FormattingOptions.ShowStepDetail))
			{
				sb.Append($"  (---{total,8}) ");
			}

			sb.Append($"{stepsCount,3} ");
			sb.AppendLine(ResourceDictionary.Get(stepsCount == 1 ? "AnalysisResultStepSingular" : "AnalysisResultStepPlural", culture));

			a(sb, options.HasFlag(FormattingOptions.ShowSeparators));
		}

		// Print detail data.
		sb.Append(ResourceDictionary.Get("AnalysisResultPuzzleRating", culture));
		sb.AppendLine($"{max:0.0}/{pearl ?? MaximumRatingValueTheory:0.0}/{diamond ?? MaximumRatingValueTheory:0.0}");

		// Print the solution (if not null and worth).
		if (!solution.IsUndefined && options.HasFlag(FormattingOptions.ShowGridAndSolutionCode))
		{
			sb.AppendLine($"{ResourceDictionary.Get("AnalysisResultPuzzleSolution", culture)}{solution:!}");
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
			sb.AppendLine($@"{ResourceDictionary.Get("AnalysisResultTimeElapsed", culture)}{elapsed:hh\:mm\:ss\.fff}");
		}

		a(sb, options.HasFlag(FormattingOptions.ShowSeparators));
		return sb.ToString();


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void a(StringBuilder sb, bool showSeparator)
		{
			if (showSeparator)
			{
				sb.Append('-', 10);
				sb.AppendLine();
			}
		}

		(int, Step)? getBottleneck()
		{
			if (this is not { IsSolved: true, InterimSteps: { Length: var stepsCount } steps })
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
	public ReadOnlySpan<Step>.Enumerator GetEnumerator() => StepsSpan.GetEnumerator();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator IEnumerable.GetEnumerator() => StepsSpan.ToArray().GetEnumerator();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator<Step> IEnumerable<Step>.GetEnumerator() => ((IEnumerable<Step>)StepsSpan.ToArray()).GetEnumerator();
}

/// <summary>
/// The helper type.
/// </summary>
file static class Helper
{
	/// <summary>
	/// The inner executor to get the difficulty value (total, average).
	/// </summary>
	/// <param name="steps">The steps to be calculated.</param>
	/// <param name="executor">The execute method.</param>
	/// <param name="d">
	/// The default value as the return value when <see cref="Steps"/> is <see langword="null"/> or empty.
	/// </param>
	/// <returns>The result.</returns>
	/// <seealso cref="Steps"/>
	public static unsafe decimal EvaluateRatingUnsafe(Step[]? steps, StepRatingEvaluatorFuncPtr executor, decimal d)
	{
		static decimal f(Step step) => step.Difficulty;
		return steps is null ? d : executor(steps, &f);
	}

	/// <inheritdoc cref="Enumerable.Max(IEnumerable{decimal})"/>
	public static unsafe decimal MaxUnsafe<T>(T[] collection, delegate*<T, decimal> selector)
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
	public static unsafe decimal SumUnsafe<T>(T[] collection, delegate*<T, decimal> selector)
	{
		var result = 0M;
		foreach (var element in collection)
		{
			result += selector(element);
		}

		return result;
	}
}
