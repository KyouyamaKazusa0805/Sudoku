namespace Sudoku.Solving.Manual;

/// <summary>
/// Provides a manual solver that solves a sudoku puzzle using the human minds and ways
/// to check and solve a sudoku puzzle.
/// </summary>
public sealed class ManualSolver : IComplexSolver<ManualSolver, ManualSolverResult>, IManualSolverOptions
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
	/// <remarks>
	/// The default value is <see langword="false"/>.
	/// </remarks>
	public bool IsFullApplying { get; set; }

	/// <inheritdoc cref="IAlmostLockedSetsXzStepSearcher.AllowCollision"/>
	public bool AllowCollisionOnAlsXz
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			var searcher = TargetSearcherCollection.GetOfType<IAlmostLockedSetsXzStepSearcher>();
			if (searcher is not null)
			{
				searcher.AllowCollision = value;
			}
		}
	}

	/// <inheritdoc cref="IAlmostLockedSetsXyWingStepSearcher.AllowCollision"/>
	public bool AllowCollisionOnAlsXyWing
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			var searcher = TargetSearcherCollection.GetOfType<IAlmostLockedSetsXyWingStepSearcher>();
			if (searcher is not null)
			{
				searcher.AllowCollision = value;
			}
		}
	}

	/// <inheritdoc cref="IAlmostLockedSetsXzStepSearcher.AllowLoopedPatterns"/>
	public bool AllowLoopedPatternsOnAlsXz
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			var searcher = TargetSearcherCollection.GetOfType<IAlmostLockedSetsXzStepSearcher>();
			if (searcher is not null)
			{
				searcher.AllowLoopedPatterns = value;
			}
		}
	}

	/// <inheritdoc cref="IAlmostLockedCandidatesStepSearcher.CheckAlmostLockedQuadruple"/>
	public bool CheckAlmostLockedQuadruple
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			var searcher = TargetSearcherCollection.GetOfType<IAlmostLockedCandidatesStepSearcher>();
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
			var searcher = TargetSearcherCollection.GetOfType<IBivalueUniversalGraveStepSearcher>();
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
			var searcher = TargetSearcherCollection.GetOfType<IJuniorExocetStepSearcher>();
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
			var searcher = TargetSearcherCollection.GetOfType<ISeniorExocetStepSearcher>();
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
			var searcher = TargetSearcherCollection.GetOfType<ISingleStepSearcher>();
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
			var searcher = TargetSearcherCollection.GetOfType<ISingleStepSearcher>();
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
			var searcher = TargetSearcherCollection.GetOfType<ISingleStepSearcher>();
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
			var searcher = TargetSearcherCollection.GetOfType<ITemplateStepSearcher>();
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
			var searcher = TargetSearcherCollection.GetOfType<IUniqueRectangleStepSearcher>();
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
			var searcher = TargetSearcherCollection.GetOfType<IUniqueRectangleStepSearcher>();
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
			var searcher = TargetSearcherCollection.GetOfType<IRegularWingStepSearcher>();
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
			var searchers = TargetSearcherCollection.GetOfType<IAlternatingInferenceChainStepSearcher>(true);
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
			var searcher = TargetSearcherCollection.GetOfType<IBowmanBingoStepSearcher>();
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
			var searcher = TargetSearcherCollection.GetOfType<IComplexFishStepSearcher>();
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
					_ => throw new NotSupportedException("I'm sorry that Sudoku Explainer mode is not implemented at present.")
				};
			}
			catch (OperationCanceledException ex) when (ex.CancellationToken != cancellationToken)
			{
				throw;
			}
			catch (Exception ex)
			{
				return ex switch
				{
					NotImplementedException or NotSupportedException => result with
					{
						IsSolved = false,
						FailedReason = FailedReason.NotImplemented
					},
					WrongStepException { WrongStep: var ws } castedException => result with
					{
						IsSolved = false,
						FailedReason = FailedReason.WrongStep,
						WrongStep = ws,
						UnhandledException = castedException,
					},
					OperationCanceledException => result with
					{
						IsSolved = false,
						FailedReason = FailedReason.UserCancelled
					},
					_ => result with
					{
						IsSolved = false,
						FailedReason = FailedReason.ExceptionThrown,
						UnhandledException = ex
					}
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
		InitializeMaps(playground);
		for (int i = 0, length = stepSearchers.Length; i < length; i++)
		{
			var searcher = stepSearchers[i];
			if (isSukaku && searcher is IDeadlyPatternStepSearcher
				|| searcher.Options.EnabledArea == EnabledArea.None)
			{
				// Skips on those two cases:
				// 1. Sukaku puzzles can't use deadly pattern techniques.
				// 2. If the searcher is currently disabled, just skip it.
				continue;
			}

			if (IsFullApplying && searcher is not IBruteForceStepSearcher)
			{
				var accumulator = new List<Step>();
				searcher.GetAll(accumulator, playground, false);
				if (accumulator.Count == 0)
				{
					continue;
				}

				foreach (var foundStep in accumulator)
				{
					if (verifyConclusionValidity(solution, foundStep))
					{
						if (
							recordStep(
								recordedSteps, foundStep, ref playground, stopwatch, stepGrids,
								resultBase, cancellationToken, out var result)
						)
						{
							return result;
						}
					}
					else
					{
						throw new WrongStepException(playground, foundStep);
					}
				}

				// The puzzle has not been finished, we should turn to the first step finder
				// to continue solving puzzle.
				goto TryAgain;
			}
			else
			{
				var foundStep = searcher.GetAll(null!, playground, true);
				if (foundStep is null)
				{
					continue;
				}

				if (verifyConclusionValidity(solution, foundStep))
				{
					if (
						recordStep(
							recordedSteps, foundStep, ref playground, stopwatch, stepGrids,
							resultBase, cancellationToken, out var result)
					)
					{
						return result;
					}
				}
				else
				{
					throw new WrongStepException(playground, foundStep);
				}

				// The puzzle has not been finished, we should turn to the first step finder
				// to continue solving puzzle.
				goto TryAgain;
			}
		}

		// All solver can't finish the puzzle...
		// :(
		stopwatch.Stop();

		return resultBase with
		{
			IsSolved = false,
#pragma warning disable CS0618
			FailedReason = FailedReason.PuzzleIsTooHard,
#pragma warning restore CS0618
			ElapsedTime = stopwatch.Elapsed,
			Steps = ImmutableArray.CreateRange(recordedSteps),
			StepGrids = ImmutableArray.CreateRange(stepGrids)
		};


		static bool recordStep(
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

		static bool verifyConclusionValidity(scoped in Grid solution, Step step)
		{
			foreach (var (t, c, d) in step.Conclusions)
			{
				int digit = solution[c];
				if (t == ConclusionType.Assignment && digit != d
					|| t == ConclusionType.Elimination && digit == d)
				{
					return false;
				}
			}

			return true;
		}
	}
}
