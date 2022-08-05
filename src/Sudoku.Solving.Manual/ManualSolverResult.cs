namespace Sudoku.Solving.Manual;

/// <summary>
/// Provides the solver result after <see cref="ManualSolver"/> solving a puzzle.
/// </summary>
/// <param name="Puzzle"><inheritdoc/></param>
public sealed unsafe record ManualSolverResult(scoped in Grid Puzzle) :
	IComplexSolverResult<ManualSolver, ManualSolverResult>
{
	/// <inheritdoc/>
	public bool IsSolved { get; init; }

	/// <summary>
	/// <para>Indicates the maximum difficulty of the puzzle.</para>
	/// <para>
	/// When the puzzle is solved by <see cref="ManualSolver"/>,
	/// the value will be the maximum value among all difficulty
	/// ratings in solving steps. If the puzzle has not been solved,
	/// or else the puzzle is solved by other solvers, this value will
	/// be always <c>20.0M</c>.
	/// </para>
	/// </summary>
	/// <seealso cref="ManualSolver"/>
	public decimal MaxDifficulty => Evaluator(&EnumerableExtensions.Max<IStep>, 20.0M);

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
	public decimal TotalDifficulty => Evaluator(&EnumerableExtensions.Sum<IStep>, 0);

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
		=> Steps.IsDefaultOrEmpty
			? 0
			: Steps.FirstOrDefault(static info => info.ShowDifficulty)?.Difficulty ?? 0;

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
			if (!Steps.IsDefaultOrEmpty)
			{
				for (int i = 0, length = Steps.Length; i < length; i++)
				{
					var step = Steps[i];
					if (step.HasTag(TechniqueTags.Singles))
					{
						if (i == 0)
						{
							return step.Difficulty;
						}
						else
						{
							decimal max = 0;
							for (int j = 0; j < i; j++)
							{
								decimal difficulty = Steps[j].Difficulty;
								if (difficulty >= max)
								{
									max = difficulty;
								}
							}

							return max;
						}
					}
				}
			}

			return 20.0M;
		}
	}

	/// <summary>
	/// Indicates the number of all solving steps recorded.
	/// </summary>
	public int SolvingStepsCount => Steps.IsDefault ? 1 : Steps.Length;

	/// <summary>
	/// Indicates why the solving operation is failed. This property is useless when <see cref="IsSolved"/>
	/// keeps the <see langword="true"/> value.
	/// </summary>
	/// <seealso cref="IsSolved"/>
	public SearcherFailedReason FailedReason { get; init; }

	/// <summary>
	/// Indicates the difficulty level of the puzzle.
	/// If the puzzle has not solved or solved by other solvers,
	/// this value will be <see cref="DifficultyLevel.Unknown"/>.
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
					if (step.ShowDifficulty && step.DifficultyLevel > maxLevel)
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
	public ImmutableArray<Grid> StepGrids { get; init; }

	/// <summary>
	/// Indicates all solving steps that the solver has recorded.
	/// </summary>
	/// <seealso cref="StepGrids"/>
	public ImmutableArray<IStep> Steps { get; init; }

	/// <summary>
	/// <para>Indicates a list of pairs of information about each step.</para>
	/// <para>
	/// If the puzzle cannot be solved due to some reason (invalid puzzle, unhandled exception, etc.),
	/// the return value of the property will be always the <see langword="default"/> expression of type
	/// <see cref="ImmutableArray{T}"/>, of <see cref="ValueTuple{T1, T2}"/>
	/// of types <see cref="Grid"/> and <see cref="IStep"/>.
	/// </para>
	/// </summary>
	public ImmutableArray<(Grid SteppingGrid, IStep Step)> SolvingPath
		=> IsSolved ? StepGrids.Zip(Steps) : default(ImmutableArray<(Grid, IStep)>);

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
	public IStep? WrongStep { get; init; }

	/// <summary>
	/// Gets the bottleneck during the whole grid solving. Returns <see langword="null"/> if the property
	/// <see cref="Steps"/> is default case (not initialized or empty).
	/// </summary>
	/// <seealso cref="Steps"/>
	public IStep? Bottleneck
	{
		get
		{
			if (Steps.IsDefault)
			{
				return null;
			}

			if (Steps is not [var firstStep, ..])
			{
				return null;
			}

			for (int i = Steps.Length - 1; i >= 0; i--)
			{
				var step = Steps[i];
				if (step.ShowDifficulty && !step.HasTag(TechniqueTags.Singles))
				{
					return step;
				}
			}

			// If code goes to here, all steps are more difficult than single techniques.
			// Get the first one is okay.
			return firstStep;
		}
	}

	/// <inheritdoc/>
	/// <remarks>
	/// You can visit the property value
	/// if the property <see cref="FailedReason"/> is <see cref="SearcherFailedReason.ExceptionThrown"/>
	/// or <see cref="SearcherFailedReason.WrongStep"/>.
	/// </remarks>
	/// <seealso cref="FailedReason"/>
	/// <seealso cref="SearcherFailedReason.ExceptionThrown"/>
	/// <seealso cref="SearcherFailedReason.WrongStep"/>
	public Exception? UnhandledException { get; init; }


	/// <summary>
	/// Gets the <see cref="IStep"/> instance at the specified index.
	/// </summary>
	/// <param name="index">The index.</param>
	/// <returns>The step information.</returns>
	/// <exception cref="InvalidOperationException">
	/// Throws when the <see cref="Steps"/> is <see langword="null"/> or empty.
	/// </exception>
	/// <exception cref="IndexOutOfRangeException">Throws when the index is out of range.</exception>
	/// <seealso cref="Steps"/>
	public IStep this[int index]
		=> Steps switch
		{
			{ IsDefaultOrEmpty: true }
				=> throw new InvalidOperationException("You can't extract any elements because of being null or empty."),
			{ Length: var length } when index < 0 || index >= length
				=> throw new IndexOutOfRangeException($"Parameter '{nameof(index)}' is out of range."),
			_ => Steps[index]
		};

	/// <summary>
	/// Gets the first <see cref="IStep"/> instance that matches the specified technique.
	/// </summary>
	/// <param name="code">The technique code to check and fetch.</param>
	/// <returns>The step information instance as the result.</returns>
	/// <exception cref="InvalidOperationException">
	/// Throws when the list doesn't contain any valid instance to get.
	/// </exception>
	public IStep this[Technique code]
		=> IsSolved
			? Steps.First(step => step.TechniqueCode == code)
			: throw new InvalidOperationException("The specified instance can't get the result.");


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString()
		=> ToString(
			SolverResultFormattingOptions.ShowStepsAfterBottleneck
				| SolverResultFormattingOptions.ShowSeparators
				| SolverResultFormattingOptions.ShowDifficulty
				| SolverResultFormattingOptions.ShowSteps
		);

	/// <summary>
	/// Returns a string that represents the current object, with the specified formatting options.
	/// </summary>
	/// <param name="options">The formatting options.</param>
	/// <returns>A string that represents the current object.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(SolverResultFormattingOptions options)
	{
		// Get all information.
		if (
			this is not
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
			}
		)
		{
			throw new();
		}

		// Print header.
		scoped var sb = new StringHandler();
		sb.Append(R["AnalysisResultPuzzle"]!);
		sb.Append(puzzle.ToString("#"));
		sb.AppendLine();

		// Print solving steps (if worth).
		if (options.Flags(SolverResultFormattingOptions.ShowSteps) && !steps.IsDefaultOrEmpty)
		{
			sb.Append(R["AnalysisResultSolvingSteps"]!);
			sb.AppendLine();

			if (getBottleneck() is var (bIndex, bInfo))
			{
				for (int i = 0, count = steps.Length; i < count; i++)
				{
					if (i > bIndex && options.Flags(SolverResultFormattingOptions.ShowStepsAfterBottleneck))
					{
						sb.Append(R.EmitPunctuation(Punctuation.Ellipsis));
						sb.AppendLine();

						break;
					}

					var info = steps[i];
					string infoStr = options.Flags(SolverResultFormattingOptions.ShowSimple)
						? info.ToSimpleString()
						: info.Formatize();
					bool showDiff = options.Flags(SolverResultFormattingOptions.ShowDifficulty)
						&& info.ShowDifficulty;

					string d = $"({info.Difficulty,5:0.0}";
					string s = $"{i + 1,4}";
					string labelInfo = (options.Flags(SolverResultFormattingOptions.ShowStepLabel), showDiff) switch
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

				if (options.Flags(SolverResultFormattingOptions.ShowBottleneck))
				{
					a(ref sb, options.Flags(SolverResultFormattingOptions.ShowSeparators));

					sb.Append(R["AnalysisResultBottleneckStep"]!);

					if (options.Flags(SolverResultFormattingOptions.ShowStepLabel))
					{
						sb.Append(R["AnalysisResultInStep"]!);
						sb.Append(bIndex + 1);
						sb.Append(R.EmitPunctuation(Punctuation.Colon));
					}

					sb.Append(' ');
					sb.Append(bInfo);
					sb.AppendLine();
				}

				a(ref sb, options.Flags(SolverResultFormattingOptions.ShowSeparators));
			}
		}

		// Print solving step statistics (if worth).
		if (!steps.IsDefault)
		{
			sb.Append(R["AnalysisResultTechniqueUsed"]!);
			sb.AppendLine();

			if (options.Flags(SolverResultFormattingOptions.ShowStepDetail))
			{
				sb.Append(R["AnalysisResultMin"]!, 6);
				sb.Append(',');
				sb.Append(' ');
				sb.Append(R["AnalysisResultTotal"]!, 6);
				sb.Append(R["AnalysisResultTechniqueUsing"]!);
			}

			foreach (var solvingStepsGroup in from s in steps orderby s.Difficulty group s by s.Name)
			{
				if (options.Flags(SolverResultFormattingOptions.ShowStepDetail))
				{
					decimal currentTotal = 0, currentMinimum = decimal.MaxValue;
					foreach (var solvingStep in solvingStepsGroup)
					{
						decimal difficulty = solvingStep.Difficulty;
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

			if (options.Flags(SolverResultFormattingOptions.ShowStepDetail))
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

			a(ref sb, options.Flags(SolverResultFormattingOptions.ShowSeparators));
		}

		// Print detail data.
		sb.Append(R["AnalysisResultPuzzleRating"]!);
		sb.Append(max, "0.0");
		sb.Append('/');
		sb.Append(pearl, "0.0");
		sb.Append('/');
		sb.Append(diamond, "0.0");
		sb.AppendLine();

		// Print the solution (if not null).
		if (!solution.IsUndefined)
		{
			sb.Append(R["AnalysisResultPuzzleSolution"]!);
			sb.Append(solution.ToString("!"));
		}

		// Print the elapsed time.
		sb.Append(R["AnalysisResultPuzzleHas"]!);
		sb.AppendWhen(!isSolved, R["AnalysisResultNot"]!);
		sb.Append(R["AnalysisResultBeenSolved"]!);
		sb.AppendLine();
		sb.Append(R["AnalysisResultTimeElapsed"]!);
		sb.Append($@"{elapsed:hh\:mm\:ss\.fff}");
		sb.AppendLine();

		a(ref sb, options.Flags(SolverResultFormattingOptions.ShowSeparators));

		return sb.ToStringAndClear();


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void a(scoped ref scoped StringHandler sb, bool showSeparator)
		{
			if (showSeparator)
			{
				sb.Append('-', 10);
				sb.AppendLine();
			}
		}

		(int, IStep)? getBottleneck()
		{
			if (this is not { IsSolved: true, Steps: var steps, SolvingStepsCount: var stepsCount })
			{
				return null;
			}

			for (int i = stepsCount - 1; i >= 0; i--)
			{
				if (steps[i] is { ShowDifficulty: true } step and not SingleStep)
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
	public ImmutableArray<IStep>.Enumerator GetEnumerator() => Steps.GetEnumerator();


	/// <summary>
	/// The inner executor to get the difficulty value (total, average).
	/// </summary>
	/// <param name="executor">The execute method.</param>
	/// <param name="d">
	/// The default value as the return value when <see cref="Steps"/> is <see langword="null"/> or empty.
	/// </param>
	/// <returns>The result.</returns>
	/// <seealso cref="Steps"/>
	private decimal Evaluator(
		delegate*<IEnumerable<IStep>, delegate*<IStep, decimal>, decimal> executor,
		decimal d)
	{
		return Steps.IsDefaultOrEmpty ? d : executor(Steps, &f);


		static decimal f(IStep step) => step.ShowDifficulty ? step.Difficulty : 0;
	}
}
