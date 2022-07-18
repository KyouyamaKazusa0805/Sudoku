namespace Sudoku.Solving.Manual;

/// <summary>
/// Provides a manual solver that solves a sudoku puzzle using the human minds and ways
/// to check and solve a sudoku puzzle.
/// </summary>
public sealed class ManualSolver : IComplexSolver<ManualSolverResult>, IManualSolverOptions
{
	/// <summary>
	/// The backing field of the property <see cref="IsHodokuMode"/>.
	/// </summary>
	/// <seealso cref="IsHodokuMode"/>
	private bool _isHodokuMode = true;


	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <see langword="true"/>.
	/// </remarks>
	/// <exception cref="NotSupportedException">
	/// Throws when the <see langword="value"/> is <see langword="false"/>.
	/// </exception>
	public bool IsHodokuMode
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _isHodokuMode;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => _isHodokuMode = value ? value : throw new NotSupportedException("Other modes are not supported now.");
	}

	/// <inheritdoc/>
	public bool IsFastSearching { get; set; }

	/// <inheritdoc/>
	public bool OptimizedApplyingOrder { get; set; }

	/// <inheritdoc cref="IAlmostLockedCandidatesStepSearcher.CheckAlmostLockedQuadruple"/>
	public bool CheckAlmostLockedQuadruple
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			var searcher = TargetSearcherCollection.OfType<IAlmostLockedCandidatesStepSearcher>().FirstOrDefault();
			if (searcher is not null)
			{
				searcher.CheckAlmostLockedQuadruple = value;
			}
		}
	}

	/// <inheritdoc cref="IBivalueUniversalGraveStepSearcher.SearchExtendedTypes"/>
	public bool SearchBivalueUniversalGraveExtendedTypes
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			var searcher = TargetSearcherCollection.OfType<IBivalueUniversalGraveStepSearcher>().FirstOrDefault();
			if (searcher is not null)
			{
				searcher.SearchExtendedTypes = value;
			}
		}
	}

	/// <inheritdoc cref="IExocetStepSearcher.CheckAdvanced"/>
	public bool CheckAdvancedJuniorExocets
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			var searcher = TargetSearcherCollection.OfType<IJuniorExocetStepSearcher>().FirstOrDefault();
			if (searcher is not null)
			{
				searcher.CheckAdvanced = value;
			}
		}
	}

	/// <inheritdoc cref="IExocetStepSearcher.CheckAdvanced"/>
	public bool CheckAdvancedSeniorExocets
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			var searcher = TargetSearcherCollection.OfType<ISeniorExocetStepSearcher>().FirstOrDefault();
			if (searcher is not null)
			{
				searcher.CheckAdvanced = value;
			}
		}
	}

	/// <inheritdoc cref="ISingleStepSearcher.EnableFullHouse"/>
	public bool EnableFullHouse
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			var searcher = TargetSearcherCollection.OfType<ISingleStepSearcher>().FirstOrDefault();
			if (searcher is not null)
			{
				searcher.EnableFullHouse = value;
			}
		}
	}

	/// <inheritdoc cref="ISingleStepSearcher.EnableLastDigit"/>
	public bool EnableLastDigit
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			var searcher = TargetSearcherCollection.OfType<ISingleStepSearcher>().FirstOrDefault();
			if (searcher is not null)
			{
				searcher.EnableLastDigit = value;
			}
		}
	}

	/// <inheritdoc cref="ISingleStepSearcher.HiddenSinglesInBlockFirst"/>
	public bool HiddenSinglesInBlockFirst
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			var searcher = TargetSearcherCollection.OfType<ISingleStepSearcher>().FirstOrDefault();
			if (searcher is not null)
			{
				searcher.HiddenSinglesInBlockFirst = value;
			}
		}
	}

	/// <inheritdoc cref="ITemplateStepSearcher.TemplateDeleteOnly"/>
	public bool TemplateDeleteOnly
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			var searcher = TargetSearcherCollection.OfType<ITemplateStepSearcher>().FirstOrDefault();
			if (searcher is not null)
			{
				searcher.TemplateDeleteOnly = value;
			}
		}
	}

	/// <inheritdoc cref="IUniqueRectangleStepSearcher.AllowIncompleteUniqueRectangles"/>
	public bool AllowIncompleteUniqueRectangles
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			var searcher = TargetSearcherCollection.OfType<IUniqueRectangleStepSearcher>().FirstOrDefault();
			if (searcher is not null)
			{
				searcher.AllowIncompleteUniqueRectangles = value;
			}
		}
	}

	/// <inheritdoc cref="IUniqueRectangleStepSearcher.SearchForExtendedUniqueRectangles"/>
	public bool SearchForExtendedUniqueRectangles
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			var searcher = TargetSearcherCollection.OfType<IUniqueRectangleStepSearcher>().FirstOrDefault();
			if (searcher is not null)
			{
				searcher.SearchForExtendedUniqueRectangles = value;
			}
		}
	}

	/// <inheritdoc cref="IRegularWingStepSearcher.MaxSize"/>
	public int RegularWingMaxSize
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			var searcher = TargetSearcherCollection.OfType<IRegularWingStepSearcher>().FirstOrDefault();
			if (searcher is not null)
			{
				searcher.MaxSize = value;
			}
		}
	}

	/// <inheritdoc cref="IAlternatingInferenceChainStepSearcher.MaxCapacity"/>
	public int ChainingMaxCapacity
	{
		set
		{
			var searchers = TargetSearcherCollection.OfType<IAlternatingInferenceChainStepSearcher>();
			foreach (var searcher in searchers)
			{
				searcher.MaxCapacity = value;
			}
		}
	}

	/// <inheritdoc cref="IBowmanBingoStepSearcher.MaxLength"/>
	public int BowmanBingoMaxLength
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			var searcher = TargetSearcherCollection.OfType<IBowmanBingoStepSearcher>().FirstOrDefault();
			if (searcher is not null)
			{
				searcher.MaxLength = value;
			}
		}
	}

	/// <inheritdoc cref="IFishStepSearcher.MaxSize"/>
	public int ComplexFishMaxSize
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			var searcher = TargetSearcherCollection.OfType<IComplexFishStepSearcher>().FirstOrDefault();
			if (searcher is not null)
			{
				searcher.MaxSize = value;
			}
		}
	}

	/// <inheritdoc/>
	[DisallowNull]
	public IStepSearcher[]? CustomSearcherCollection { get; set; }

	/// <summary>
	/// Indicates the target step searcher collection.
	/// </summary>
	private IStepSearcher[] TargetSearcherCollection
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (
			from searcher in CustomSearcherCollection ?? StepSearcherPool.Collection
			where searcher.Options.EnabledArea.Flags(EnabledArea.Default)
			select searcher
		).ToArray();
	}


	/// <inheritdoc/>
	public ManualSolverResult Solve(scoped in Grid puzzle, CancellationToken cancellationToken = default)
	{
		var result = new ManualSolverResult(puzzle);
		if (puzzle.ExactlyValidate(out var solution, out bool? sukaku))
		{
			try
			{
				return IsHodokuMode switch
				{
					// Hodoku mode.
					true => Solve_HodokuMode(puzzle, solution, sukaku.Value, result, cancellationToken),

					// Sudoku explainer mode.
					// TODO: Implement a sudoku-explainer mode solving module.
					_ => throw new NotImplementedException()
				};
			}
			catch (Exception ex)
			{
				return ex switch
				{
					NotImplementedException
						=> result with { IsSolved = false, FailedReason = FailedReason.NotImplemented },
					WrongStepException { WrongStep: var ws } castedException
						=> result with { IsSolved = false, FailedReason = FailedReason.WrongStep, WrongStep = ws },
					OperationCanceledException casted when casted.CancellationToken == cancellationToken
						=> result with { IsSolved = false, FailedReason = FailedReason.UserCancelled },
					OperationCanceledException casted => throw casted,
					_ => result with { IsSolved = false, FailedReason = FailedReason.ExceptionThrown, UnhandledException = ex }
				};
			}
		}
		else
		{
			return result with { IsSolved = false, FailedReason = FailedReason.PuzzleIsInvalid };
		}
	}


	/// <summary>
	/// The inner solving operation method.
	/// </summary>
	/// <param name="puzzle">The original puzzle to be solved.</param>
	/// <param name="solution">The solution of the puzzle. Some step searchers will use this value.</param>
	/// <param name="isSukaku">A <see cref="bool"/> value indicating whether the puzzle is a sukaku.</param>
	/// <param name="resultBase">The base solver result already included the base information.</param>
	/// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
	/// <returns>The solver result.</returns>
	/// <exception cref="WrongStepException">Throws when found wrong steps to apply.</exception>
	/// <exception cref="OperationCanceledException">Throws when the operation is canceled.</exception>
	private ManualSolverResult Solve_HodokuMode(
		scoped in Grid puzzle,
		scoped in Grid solution,
		bool isSukaku,
		ManualSolverResult resultBase,
		CancellationToken cancellationToken = default)
	{
		var playground = puzzle;
		var tempSteps = new List<Step>(20);
		var recordedSteps = new List<Step>(100);
		var stepGrids = new List<Grid>(100);
		var stepSearchers = TargetSearcherCollection;

		// Sets the solution grid to some step searchers.
		// Some step searchers will use solution grids to solve a puzzle.
		foreach (var stepSearcher in stepSearchers.OfType<IStepSearcherRequiresSolution>())
		{
			stepSearcher.Solution = solution;
		}

		var stopwatch = new Stopwatch();
		stopwatch.Start();

	TryAgain:
		tempSteps.Clear();

		InitializeMaps(playground);
		for (int i = 0, length = stepSearchers.Length; i < length; i++)
		{
			var searcher = stepSearchers[i];
			if (isSukaku && searcher is IDeadlyPatternStepSearcher || searcher.Options.EnabledArea == EnabledArea.None)
			{
				// Skips on those two cases:
				// 1. Sukaku puzzles can't use deadly pattern techniques.
				// 2. If the searcher is currently disabled, just skip it.
				continue;
			}

			searcher.GetAll(tempSteps, playground, false);
			if (tempSteps.Count == 0)
			{
				// Nothing found.
				continue;
			}

			if (IsFastSearching)
			{
				Step? wrongStep = null;
				foreach (var tempStep in tempSteps)
				{
					if (!AreConclusionsValid(solution, tempStep))
					{
						wrongStep = tempStep;
						break;
					}
				}

				if (wrongStep is null)
				{
					foreach (var step in tempSteps)
					{
						if (
							RecordStep(
								recordedSteps, step, ref playground, stopwatch, stepGrids,
								resultBase, cancellationToken, out var result)
						)
						{
							return result;
						}
					}

					// The puzzle has not been finished, we should turn to the first step finder
					// to continue solving puzzle.
					goto TryAgain;
				}

				throw new WrongStepException(puzzle, wrongStep);
			}
			else
			{
				// If the searcher is only used in the fast mode, just skip it.
				var step = OptimizedApplyingOrder
					? (from info in tempSteps orderby info.Difficulty select info).FirstOrDefault()
					: tempSteps[0];
				if (step is null)
				{
					// If current step can't find any steps,
					// we will turn to the next step finder to
					// continue solving puzzle.
					continue;
				}

				if (AreConclusionsValid(solution, step))
				{
					if (
						RecordStep(
							recordedSteps, step, ref playground, stopwatch, stepGrids,
							resultBase, cancellationToken, out var result)
					)
					{
						return result;
					}

					// The puzzle has not been finished, we should turn to the first step finder
					// to continue solving puzzle.
					goto TryAgain;
				}

				throw new WrongStepException(puzzle, step);
			}
		}

		// All solver can't finish the puzzle...
		// :(
		stopwatch.Stop();
		return resultBase with
		{
			IsSolved = false,
			FailedReason = FailedReason.PuzzleIsTooHard,
			ElapsedTime = stopwatch.Elapsed,
			Steps = ImmutableArray.CreateRange(recordedSteps),
			StepGrids = ImmutableArray.CreateRange(stepGrids)
		};
	}

	/// <summary>
	/// <para>
	/// Records the current found and valid step into the specified collection. This method will also
	/// check the validity of <paramref name="cancellationToken"/>. If user has canceled the operation,
	/// here we'll throw an exception and exit the operation directly.
	/// </para>
	/// <para>
	/// Please note that if the argument <paramref name="result"/> isn't <see langword="null"/>, it'll mean
	/// that the puzzle has been already solved, so this method will stop the stopwatch. Therefore, you don't
	/// need to stop that stopwatch manually like the code <c>stopwatch.Stop();</c>.
	/// </para>
	/// </summary>
	/// <param name="steps">The steps.</param>
	/// <param name="step">The step.</param>
	/// <param name="playground">The playground.</param>
	/// <param name="stopwatch">The stopwatch.</param>
	/// <param name="stepGrids">The step grids.</param>
	/// <param name="resultBase">Indicates the base solver result.</param>
	/// <param name="cancellationToken">The cancellation token that is used to cancel the operation.</param>
	/// <param name="result">The analysis result.</param>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	/// <exception cref="OperationCanceledException">
	/// Throws when the current operation is canceled.
	/// </exception>
	private bool RecordStep(
		ICollection<Step> steps,
		Step step,
		scoped ref Grid playground,
		Stopwatch stopwatch,
		ICollection<Grid> stepGrids,
		ManualSolverResult resultBase,
		CancellationToken cancellationToken,
		[NotNullWhen(true)] out ManualSolverResult? result)
	{
		bool atLeastOneStepIsWorth = false;
		foreach (var (t, c, d) in step.Conclusions)
		{
			switch (t)
			{
				case ConclusionType.Assignment when playground.GetStatus(c) == CellStatus.Empty:
				case ConclusionType.Elimination when playground.Exists(c, d) is true:
				{
					atLeastOneStepIsWorth = true;

					goto FinalCheck;
				}
			}
		}

	FinalCheck:
		if (atLeastOneStepIsWorth)
		{
			stepGrids.Add(playground);
			step.ApplyTo(ref playground);
			steps.Add(step);

			if (playground.IsSolved)
			{
				stopwatch.Stop();

				result = resultBase with
				{
					IsSolved = true,
					ElapsedTime = stopwatch.Elapsed,
					Steps = ImmutableArray.CreateRange(steps),
					StepGrids = ImmutableArray.CreateRange(stepGrids)
				};
				return true;
			}
		}

		cancellationToken.ThrowIfCancellationRequested();

		result = null;
		return false;
	}


	/// <summary>
	/// Peeks the validity of step, to check whether all conclusions are possibly correct.
	/// </summary>
	/// <param name="solution">The solution.</param>
	/// <param name="step">The step to check.</param>
	/// <returns>A <see cref="bool"/> indicating that.</returns>
	private static bool AreConclusionsValid(scoped in Grid solution, Step step)
	{
		foreach (var (t, c, d) in step.Conclusions)
		{
			int digit = solution[c];
			if (t == ConclusionType.Assignment && digit != d || t == ConclusionType.Elimination && digit == d)
			{
				return false;
			}
		}

		return true;
	}
}
