namespace Sudoku.Concepts.Solving;

/// <summary>
/// Defines a solving path instance that can be used for checking steps to solve a sudoku puzzle.
/// </summary>
[AutoImplementsEnumerable(typeof((Grid Grid, Step Step)), nameof(Pairs), UseExplicitImplementation = true, Pattern = "@.*")]
public readonly unsafe partial struct SolvingPath :
	IEnumerable<(Grid Grid, Step Step)>
{
	/// <summary>
	/// Indicates the grids used.
	/// </summary>
	private readonly Grid[] _grids;

	/// <summary>
	/// Indicates the steps used.
	/// </summary>
	private readonly Step[] _steps;


	/// <summary>
	/// Initializes a <see cref="SolvingPath"/> instance via the specified stepping grids and the corresponding steps.
	/// </summary>
	/// <param name="grids">The grids.</param>
	/// <param name="steps">The steps.</param>
	/// <exception cref="ArgumentException">
	/// Throws when the arrays of arguments <paramref name="grids"/> and <paramref name="steps"/> are not same of length.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public SolvingPath(Grid[] grids, Step[] steps)
		=> (_grids, _steps) = grids.Length != steps.Length
			? throw new ArgumentException("Two arrays must of a same length.")
			: ((Grid[])grids.Clone(), (Step[])steps.Clone());

	/// <summary>
	/// Initializes a <see cref="SolvingPath"/> instance via the specified pair of information.
	/// </summary>
	/// <param name="steps">The steps.</param>
	public SolvingPath((Grid Grid, Step Step)[] steps)
	{
		(_grids, _steps) = (new Grid[steps.Length], new Step[steps.Length]);

		for (int i = 0; i < steps.Length; i++)
		{
			(_grids[i], _steps[i]) = steps[i];
		}
	}


	/// <summary>
	/// Indicates the length of the solving path instance, meaning the number of steps stored in this data structure.
	/// </summary>
	public int Length
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _grids.Length;
	}

	/// <summary>
	/// Indicates the maximum difficulty of the current solving path instance recorded.
	/// </summary>
	public decimal MaxDifficulty => Evaluator(&EnumerableExtensions.Max<Step>, 20.0M);

	/// <summary>
	/// Indicates the total difficulty rating of the current solving path instance recorded.
	/// </summary>
	public decimal TotalDifficulty => Evaluator(&EnumerableExtensions.Sum<Step>, 0);

	/// <summary>
	/// <para>
	/// Indicates the pearl difficulty rating of the current solving path instance.
	/// </para>
	/// <para>
	/// When the puzzle is solved, the value will be the difficulty rating
	/// of the first solving step. If the puzzle has not solved or
	/// the puzzle is solved by other solvers, this value will be always <c>0</c>.
	/// </para>
	/// </summary>
	public decimal PearlDifficulty
		=> _steps.Length == 0 ? 0 : _steps.FirstOrDefault(static info => info.ShowDifficulty)?.Difficulty ?? 0;

	/// <summary>
	/// <para>
	/// Indicates the pearl difficulty rating of the puzzle.
	/// </para>
	/// <para>
	/// When the puzzle is solved, the value will be the difficulty rating
	/// of the first step before the first one whose conclusion is
	/// <see cref="ConclusionType.Assignment"/>. If the puzzle has not solved
	/// or solved by other solvers, this value will be <c>20.0M</c>.
	/// </para>
	/// </summary>
	/// <seealso cref="ConclusionType"/>
	public decimal DiamondDifficulty
	{
		get
		{
			if (_steps.Length != 0)
			{
				for (int i = 0, length = _steps.Length; i < length; i++)
				{
					var step = _steps[i];
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
								decimal difficulty = _steps[j].Difficulty;
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
	/// Indicates the difficulty level of the puzzle.
	/// If the puzzle has not solved or solved by other solvers,
	/// this value will be <see cref="DifficultyLevel.Unknown"/>.
	/// </summary>
	public DifficultyLevel DifficultyLevel
	{
		get
		{
			var maxLevel = DifficultyLevel.Unknown;
			foreach (var step in Steps)
			{
				if (step.ShowDifficulty && step.DifficultyLevel > maxLevel)
				{
					maxLevel = step.DifficultyLevel;
				}
			}

			return maxLevel;
		}
	}

	/// <summary>
	/// Gets the grids used in this current solving path.
	/// </summary>
	public ImmutableArray<Grid> Grids
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _grids.ToImmutableArray();
	}

	/// <summary>
	/// Gets the steps used in this current solving path.
	/// </summary>
	public ImmutableArray<Step> Steps
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _steps.ToImmutableArray();
	}

	/// <summary>
	/// Gets the bottleneck during the whole grid solving. Returns <see langword="null"/> if the property
	/// <see cref="Steps"/> is default case (not initialized or empty).
	/// </summary>
	/// <seealso cref="Steps"/>
	public Step? Bottleneck
	{
		get
		{
			if (Steps.IsDefault || Steps is not [var firstStep, ..])
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

	/// <summary>
	/// Indicates the inner property that provides with the pair of information.
	/// </summary>
	private IEnumerable<(Grid Grid, Step Step)> Pairs
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Enumerable.Zip(_grids, _steps);
	}


	/// <summary>
	/// Gets the step information and the corresponding stepping grid at the specified index.
	/// </summary>
	/// <param name="index">The index</param>
	/// <returns>A pair of information at the specified index.</returns>
	public (Grid Grid, Step Step) this[int index]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (_grids[index], _steps[index]);
	}

	/// <summary>
	/// Gets the first found <see cref="Step"/> instance whose <see cref="Step.TechniqueCode"/> is the specified one.
	/// </summary>
	/// <param name="technique">The technique code.</param>
	/// <returns>The <see cref="Step"/> instance found; otherwise, <see langword="null"/>.</returns>
	/// <seealso cref="Step.TechniqueCode"/>
	public Step? this[Technique technique]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Array.Find(_steps, step => step.TechniqueCode == technique);
	}

	/// <summary>
	/// Gets the first found <see cref="Step"/> instance whose <see cref="Step.TechniqueTags"/>
	/// matches the specified <see cref="TechniqueTags"/> instance.
	/// </summary>
	/// <param name="techniqueTags">The <see cref="TechniqueTags"/> instance.</param>
	/// <returns>The <see cref="Step"/> instance found; otherwise, <see langword="null"/>.</returns>
	/// <seealso cref="Step.TechniqueTags"/>
	public Step? this[TechniqueTags techniqueTags]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Array.Find(_steps, step => step.HasTag(techniqueTags));
	}

	/// <summary>
	/// Gets the first found <see cref="Step"/> instance whose <see cref="Step.TechniqueGroup"/>
	/// matches the specified <see cref="TechniqueGroup"/> instance.
	/// </summary>
	/// <param name="techniqueGroup">The <see cref="TechniqueGroup"/> instance.</param>
	/// <returns>The <see cref="Step"/> instance found; otherwise, <see langword="null"/>.</returns>
	/// <seealso cref="Step.TechniqueGroup"/>
	public Step? this[TechniqueGroup techniqueGroup]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Array.Find(_steps, step => step.TechniqueGroup == techniqueGroup);
	}


	/// <summary>
	/// Slices the current instance, to get the specified range of the steps and grids.
	/// </summary>
	/// <param name="start">The desired start index.</param>
	/// <param name="count">The desired number of elements from the start index.</param>
	/// <returns>The sliced result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public (Grid Grid, Step Step)[] Slice(int start, int count)
		=> Enumerable.Zip(_grids[start..(count + start)], _steps[start..(count + start)]).ToArray();

	/// <summary>
	/// Convertes the current instance to an array of pairs of <see cref="Grid"/> and its corresponding step.
	/// </summary>
	/// <returns>An array of pairs of <see cref="Grid"/> and <see cref="Step"/> instances.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public (Grid Grid, Step Step)[] ToArray() => Pairs.ToArray();

	/// <summary>
	/// Converts the current instance to a pair of arrays that stores the grids and their corresponding steps.
	/// </summary>
	/// <returns>A pair of two arrays.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public (Grid[] Grids, Step[] Steps) ToArrayUnzipped() => ((Grid[])_grids.Clone(), (Step[])_steps.Clone());

	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Enumerator GetEnumerator() => new(this);

	/// <summary>
	/// Enumerates the grids used in this solving path.
	/// </summary>
	/// <returns>The grids.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ImmutableArray<Grid>.Enumerator EnumerateGrids() => Grids.GetEnumerator();

	/// <summary>
	/// Enumerates the steps used in this solving path.
	/// </summary>
	/// <returns>The steps.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ImmutableArray<Step>.Enumerator EnumerateSteps() => Steps.GetEnumerator();

	/// <summary>
	/// The inner executor to get the difficulty value (total, average).
	/// </summary>
	/// <param name="executor">The execute method.</param>
	/// <param name="d">
	/// The default value as the return value when steps are <see langword="null"/> or empty.
	/// </param>
	/// <returns>The result.</returns>
	private decimal Evaluator(delegate*<IEnumerable<Step>, delegate*<Step, decimal>, decimal> executor, decimal d)
	{
		return _steps.Length == 0 ? d : executor(_steps, &f);


		static decimal f(Step step) => step.ShowDifficulty ? step.Difficulty : 0;
	}
}
