namespace Sudoku.Analytics;

/// <summary>
/// Provides the result after <see cref="Analyzer"/> solving a puzzle.
/// </summary>
/// <param name="Puzzle"><inheritdoc cref="IAnalyzerResult{TSolver, TSolverResult}.Puzzle" path="/summary"/></param>
public sealed partial record AnalyzerResult(scoped in Grid Puzzle) :
	IAnalyzerResult<Analyzer, AnalyzerResult>,
	IEnumerable,
	IEnumerable<Step>,
	ISelectClauseProvider<Step>,
	IWhereClauseProvider<Step>
{
	/// <inheritdoc/>
	[MemberNotNullWhen(true, nameof(Steps), nameof(StepGrids), nameof(SolvingPath))]
	public required bool IsSolved { get; init; }

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
	/// When the puzzle is solved, the value will be the difficulty rating of the first solving step.
	/// If the puzzle has not solved or the puzzle is solved by other solvers, this value will be always <c>0</c>.
	/// </remarks>
	/// <seealso cref="Analyzer"/>
	public decimal PearlDifficulty => this switch { { IsSolved: true, Steps: [{ Difficulty: var d }, ..] } => d, _ => 0 };

	/// <summary>
	/// <para>
	/// Indicates the pearl difficulty rating of the puzzle, calculated during only by <see cref="Analyzer"/>.
	/// </para>
	/// <para>
	/// When the puzzle is solved, the value will be the difficulty rating of the first step before the first one
	/// whose conclusion is <see cref="Assignment"/>.
	/// If the puzzle has not solved or solved by other solvers, this value will be <c>20.0M</c>.
	/// </para>
	/// </summary>
	/// <seealso cref="Analyzer"/>
	/// <seealso cref="Assignment"/>
	public decimal DiamondDifficulty
	{
		get
		{
			if (Steps is { Length: var l })
			{
				for (var i = 0; i < l; i++)
				{
					if (Steps[i] is SingleStep { Difficulty: var diff })
					{
						if (i == 0)
						{
							return diff;
						}

						var max = 0.0M;
						for (var j = 0; j < i; j++)
						{
							var difficulty = Steps[j].Difficulty;
							if (difficulty >= max)
							{
								max = difficulty;
							}
						}

						return max;
					}
				}
			}

			return 20.0M;
		}
	}

	/// <summary>
	/// Indicates the number of all solving steps recorded.
	/// </summary>
	public int SolvingStepsCount => Steps?.Length ?? 1;

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
	public Step? WrongStep { get; init; }

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
	/// <exception cref="InvalidOperationException">Throws when the <see cref="Steps"/> is <see langword="null"/> or empty.</exception>
	/// <exception cref="IndexOutOfRangeException">Throws when the index is out of range.</exception>
	/// <seealso cref="Steps"/>
	public Step this[int index]
		=> Steps switch
		{
			null
				=> throw new InvalidOperationException("You can't extract any elements because of being null or empty."),
			{ Length: var length } when index < 0 || index >= length
				=> throw new IndexOutOfRangeException($"Parameter '{nameof(index)}' is out of range."),
			_
				=> Steps[index]
		};

	/// <summary>
	/// Gets the first <see cref="Step"/> instance that matches the specified technique.
	/// </summary>
	/// <param name="code">The technique code to check and fetch.</param>
	/// <returns>The step information instance as the result.</returns>
	/// <exception cref="InvalidOperationException">
	/// Throws when the list doesn't contain any valid instance to get.
	/// </exception>
	public Step this[Technique code]
		=> IsSolved
			? Array.Find(Steps, step => step.Code == code)!
			: throw new InvalidOperationException("The specified instance can't get the result.");


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString()
	{
		var flags = AnalyzerResultFormattingOptions.None;
		flags |= AnalyzerResultFormattingOptions.ShowSeparators;
		flags |= AnalyzerResultFormattingOptions.ShowDifficulty;
		flags |= AnalyzerResultFormattingOptions.ShowStepsAfterBottleneck;
		flags |= AnalyzerResultFormattingOptions.ShowSteps;
		flags |= AnalyzerResultFormattingOptions.ShowGridAndSolutionCode;
		flags |= AnalyzerResultFormattingOptions.ShowElapsedTime;

		return ToString(flags);
	}

	/// <summary>
	/// Returns a string that represents the current object, with the specified formatting options.
	/// </summary>
	/// <param name="options">The formatting options.</param>
	/// <returns>A string that represents the current object.</returns>
	public string ToString(AnalyzerResultFormattingOptions options)
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
		if (options.Flags(AnalyzerResultFormattingOptions.ShowGridAndSolutionCode))
		{
			sb.Append(R["AnalysisResultPuzzle"]!);
			sb.Append(puzzle.ToString("#"));
			sb.AppendLine();
		}

		// Print solving steps (if worth).
		if (options.Flags(AnalyzerResultFormattingOptions.ShowSteps) && steps is not null)
		{
			sb.Append(R["AnalysisResultSolvingSteps"]!);
			sb.AppendLine();

			if (getBottleneck() is var (bIndex, bInfo))
			{
				for (var i = 0; i < steps.Length; i++)
				{
					if (i > bIndex && !options.Flags(AnalyzerResultFormattingOptions.ShowStepsAfterBottleneck))
					{
						sb.Append(R.EmitPunctuation(Punctuation.Ellipsis));
						sb.AppendLine();

						break;
					}

					var info = steps[i];
					var infoStr = options.Flags(AnalyzerResultFormattingOptions.ShowSimple) ? info.ToSimpleString() : info.ToString();
					var showDiff = options.Flags(AnalyzerResultFormattingOptions.ShowDifficulty);

					var d = $"({info.Difficulty,5:0.0}";
					var s = $"{i + 1,4}";
					var labelInfo = (options.Flags(AnalyzerResultFormattingOptions.ShowStepLabel), showDiff) switch
					{
						(true, true) => $"{s}, {d}) ",
						(true, false) => $"{s} ",
						(false, true) => $"{d}) ",
						_ => string.Empty,
					};

					sb.Append(labelInfo);
					sb.Append(infoStr);
					sb.AppendLine();
				}

				if (options.Flags(AnalyzerResultFormattingOptions.ShowBottleneck))
				{
					a(ref sb, options.Flags(AnalyzerResultFormattingOptions.ShowSeparators));

					sb.Append(R["AnalysisResultBottleneckStep"]!);

					if (options.Flags(AnalyzerResultFormattingOptions.ShowStepLabel))
					{
						sb.Append(R["AnalysisResultInStep"]!);
						sb.Append(bIndex + 1);
						sb.Append(R.EmitPunctuation(Punctuation.Colon));
					}

					sb.Append(' ');
					sb.Append(bInfo);
					sb.AppendLine();
				}

				a(ref sb, options.Flags(AnalyzerResultFormattingOptions.ShowSeparators));
			}
		}

		// Print solving step statistics (if worth).
		if (steps is not null)
		{
			sb.Append(R["AnalysisResultTechniqueUsed"]!);
			sb.AppendLine();

			if (options.Flags(AnalyzerResultFormattingOptions.ShowStepDetail))
			{
				sb.Append(R["AnalysisResultMin"]!, 6);
				sb.Append(',');
				sb.Append(' ');
				sb.Append(R["AnalysisResultTotal"]!, 6);
				sb.Append(R["AnalysisResultTechniqueUsing"]!);
			}

			foreach (var solvingStepsGroup in from s in steps orderby s.Difficulty group s by s.Name)
			{
				if (options.Flags(AnalyzerResultFormattingOptions.ShowStepDetail))
				{
					var currentTotal = 0M;
					var currentMinimum = decimal.MaxValue;
					foreach (var solvingStep in solvingStepsGroup)
					{
						var difficulty = solvingStep.Difficulty;
						currentTotal += difficulty;
						currentMinimum = Min(currentMinimum, difficulty);
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

			if (options.Flags(AnalyzerResultFormattingOptions.ShowStepDetail))
			{
				sb.Append("  (---");
				sb.Append(total, 8);
				sb.Append(')');
				sb.Append(' ');
			}

			sb.Append(stepsCount, 3);
			sb.Append(' ');
			sb.Append(R[stepsCount == 1 ? "AnalysisResultStepSingular" : "AnalysisResultStepPlural"]!);
			sb.AppendLine();

			a(ref sb, options.Flags(AnalyzerResultFormattingOptions.ShowSeparators));
		}

		// Print detail data.
		sb.Append(R["AnalysisResultPuzzleRating"]!);
		sb.Append(max, "0.0");
		sb.Append('/');
		sb.Append(pearl, "0.0");
		sb.Append('/');
		sb.Append(diamond, "0.0");
		sb.AppendLine();

		// Print the solution (if not null and worth).
		if (!solution.IsUndefined && options.Flags(AnalyzerResultFormattingOptions.ShowGridAndSolutionCode))
		{
			sb.Append(R["AnalysisResultPuzzleSolution"]!);
			sb.Append(solution.ToString("!"));
		}

		// Print the elapsed time.
		sb.Append(R["AnalysisResultPuzzleHas"]!);
		sb.AppendWhen(!isSolved, R["AnalysisResultNot"]!);
		sb.Append(R["AnalysisResultBeenSolved"]!);
		sb.AppendLine();
		if (options.Flags(AnalyzerResultFormattingOptions.ShowElapsedTime))
		{
			sb.Append(R["AnalysisResultTimeElapsed"]!);
			sb.Append(elapsed.ToString("""hh\:mm\:ss\.fff"""));
			sb.AppendLine();
		}

		a(ref sb, options.Flags(AnalyzerResultFormattingOptions.ShowSeparators));

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
	public StepsEnumerator GetEnumerator() => new(Steps);

	/// <summary>
	/// Filters the current collection, preserving <see cref="Step"/> instances that are satisfied the specified condition.
	/// </summary>
	/// <param name="condition">The condition to be satisfied.</param>
	/// <returns>An array of <see cref="Step"/> instances.</returns>
	public Step[]? Where(Func<Step, bool> condition)
	{
		if (Steps is null)
		{
			return null;
		}

		var result = new List<Step>(SolvingStepsCount);
		foreach (var step in Steps)
		{
			if (condition(step))
			{
				result.Add(step);
			}
		}

		return result.ToArray();
	}

	/// <summary>
	/// Filters the current collection, preserving <see cref="Step"/> instances that are satisfied the specified condition, with index.
	/// </summary>
	/// <param name="condition">The condition to be satisfied.</param>
	/// <returns>An array of <see cref="Step"/> instances.</returns>
	public Step[]? Where(Func<Step, int, bool> condition)
	{
		if (Steps is null)
		{
			return null;
		}

		var result = new List<Step>(SolvingStepsCount);
		for (var i = 0; i < SolvingStepsCount; i++)
		{
			var current = Steps[i];
			if (condition(current, i))
			{
				result.Add(current);
			}
		}

		return result.ToArray();
	}

	/// <summary>
	/// Projects the collection, to an immutable result of target type.
	/// </summary>
	/// <typeparam name="TResult">The type of the result.</typeparam>
	/// <param name="selector">
	/// The selector to project the <see cref="Step"/> instance into type <typeparamref name="TResult"/>.
	/// </param>
	/// <returns>The projected collection of element type <typeparamref name="TResult"/>.</returns>
	public TResult[]? Select<TResult>(Func<Step, TResult> selector)
	{
		if (Steps is null)
		{
			return null;
		}

		var arr = new TResult[SolvingStepsCount];
		var i = 0;
		foreach (var step in Steps)
		{
			arr[i++] = selector(step);
		}

		return arr;
	}

	/// <summary>
	/// Projects the collection, to an immutable result of target type, with index.
	/// </summary>
	/// <typeparam name="TResult">The type of the result.</typeparam>
	/// <param name="selector">
	/// The selector to project the <see cref="Step"/> instance into type <typeparamref name="TResult"/>.
	/// </param>
	/// <returns>The projected collection of element type <typeparamref name="TResult"/>.</returns>
	public TResult[]? Select<TResult>(Func<Step, int, TResult> selector)
	{
		if (Steps is null)
		{
			return null;
		}

		var arr = new TResult[SolvingStepsCount];
		var targetIndex = 0;
		for (var i = 0; i < SolvingStepsCount; i++)
		{
			var step = Steps[i];
			arr[targetIndex++] = selector(step, i);
		}

		return arr;
	}

	/// <summary>
	/// Filters the current collection, preserving steps that are of type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The type of the step you want to get.</typeparam>
	/// <returns>An array of <typeparamref name="T"/> instances.</returns>
	public T[] OfType<T>() where T : Step
	{
		if (Steps is null)
		{
			return Array.Empty<T>();
		}

		var list = new List<T>(SolvingStepsCount);
		foreach (var element in Steps)
		{
			if (element is T current)
			{
				list.Add(current);
			}
		}

		return list.ToArray();
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator IEnumerable.GetEnumerator() => (Steps?.ToArray() ?? Array.Empty<Step>()).GetEnumerator();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator<Step> IEnumerable<Step>.GetEnumerator() => ((IEnumerable<Step>)(Steps?.ToArray() ?? Array.Empty<Step>())).GetEnumerator();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerable<TResult> ISelectClauseProvider<Step>.Select<TResult>(Func<Step, TResult> selector)
		=> Select(selector) ?? Enumerable.Empty<TResult>();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerable<Step> IWhereClauseProvider<Step>.Where(Func<Step, bool> condition) => Where(condition) ?? Enumerable.Empty<Step>();

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
		return Steps is null ? d : executor(Steps, &f);


		static decimal f(Step step) => step.Difficulty;
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
