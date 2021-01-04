namespace Sudoku.Solving.Manual
{
	partial class ManualSolver
	{
		/// <summary>
		/// <para>Indicates whether the solver should check ALS cycles.</para>
		/// <para>The value is <see langword="true"/> in default case.</para>
		/// </summary>
		public bool AllowAlsCycles { get; set; } = true;

		/// <summary>
		/// <para>
		/// Indicates whether two ALSes can be overlapped with each other.
		/// </para>
		/// <para>The value is <see langword="true"/> in default case.</para>
		/// </summary>
		public bool AllowOverlappingAlses { get; set; } = true;

		/// <summary>
		/// <para>
		/// Indicates whether all ALSes shows highlight regions
		/// instead of cells.
		/// </para>
		/// <para>The value is <see langword="true"/> in default case.</para>
		/// </summary>
		public bool AlsHighlightRegionInsteadOfCell { get; set; } = true;

		/// <summary>
		/// <para>
		/// Indicates whether the solver should check all technique
		/// information using the strict technique searchers order.
		/// </para>
		/// <para>
		/// If the value is <see langword="true"/>, all technique searchers
		/// will be enabled calculation in order. It ensures the strictness
		/// of difficulty rating that the maximum difficulty searched in
		/// one searcher should be no more than the minimum one searched in
		/// the searcher behind the previous one.
		/// </para>
		/// <para>
		/// The value is <see langword="false"/> in default case. In addition,
		/// if this value is <see langword="true"/>, the option
		/// <see cref="OptimizedApplyingOrder"/> will be disabled. In other
		/// words, this option doesn't work while solving when the condition
		/// is satisfied. However, the time of calculation with this value
		/// <see langword="true"/> will be <b>much slower</b> than that with the
		/// value is <see langword="false"/>.
		/// </para>
		/// </summary>
		/// <seealso cref="OptimizedApplyingOrder"/>
		public bool AnalyzeDifficultyStrictly { get; set; }

		/// <summary>
		/// <para>
		/// Indicates whether the solver should check the technique Almost
		/// Locked Quadruple (ALQ).
		/// </para>
		/// <para>
		/// The value is <see langword="true"/> in debug environment,
		/// and <see langword="false"/> in release environment.
		/// </para>
		/// </summary>
		public bool CheckAlmostLockedQuadruple { get; set; }
#if AUTHOR_RESERVED
		= true;
#endif

		/// <summary>
		/// <para>
		/// Indicates whether the solver will check the validity of the conclusions
		/// after searched them. If the conclusions eliminate the wrong digits or
		/// assign to the wrong cells, it will report the error
		/// (i.e. throw a <see cref="SudokuHandlingException"/>).
		/// </para>
		/// <para>
		/// The value is <see langword="true"/> in default case. If the value is
		/// <see langword="true"/>, all conclusions will be checked before applying
		/// to the grid. The comparer is the solution grid. Computer doesn't know
		/// which conclusions are correct and which ones are incorrect. Therefore,
		/// the best plan is to compare to the solution grid. If not, the solver
		/// won't check the validity of all conclusions. In other words, the solver
		/// doesn't stop the searching until the grid is totally invalid (None of
		/// eliminations or assignments can be searched). However, unfortunately,
		/// the grid has no solution at present.
		/// </para>
		/// </summary>
		/// <seealso cref="SudokuHandlingException"/>
		public bool CheckConclusionValidityAfterSearched { get; set; } = true;

		/// <summary>
		/// <para>
		/// Indicates whether the solver should check Gurth's symmetrical placement
		/// at the initial grid.
		/// </para>
		/// <para>
		/// The value is <see langword="false"/> in default case. In addition,
		/// if the value is <see langword="true"/>, the solver will check the symmetry
		/// of the grid at initial. If the grid is symmetrical grid, the solver
		/// will give you a hint about the technique of symmetrical placement. However,
		/// the hint will influence the difficulty rating during solving the puzzle.
		/// If the puzzle is so easy (in other words, the grid doesn't need check
		/// it), this option will make the difficulty rating of the puzzle much more
		/// higher than that when the option is <see langword="false"/>. In addition,
		/// if the option <see cref="AnalyzeDifficultyStrictly"/> is <see langword="true"/>,
		/// this option will be ignored.
		/// </para>
		/// </summary>
		/// <seealso cref="AnalyzeDifficultyStrictly"/>
		public bool CheckGurthSymmetricalPlacement { get; set; }

		/// <summary>
		/// <para>
		/// Indicates whether the solver should check incomplete uniqueness patterns.
		/// </para>
		/// <para>
		/// The value is <see langword="true"/> in debug environment,
		/// and <see langword="false"/> in release environment.
		/// </para>
		/// </summary>
		public bool CheckIncompleteUniquenessPatterns { get; set; }
#if AUTHOR_RESERVED
		= true;
#endif

		/// <summary>
		/// <para>
		/// Indicates whether the solver should search for extended
		/// unique rectangles.
		/// </para>
		/// <para>
		/// The value is <see langword="true"/> in debug environment,
		/// and <see langword="false"/> in release environment.
		/// </para>
		/// </summary>
		public bool SearchExtendedUniqueRectangles { get; set; } 
#if AUTHOR_RESERVED
		= true;
#endif

		/// <summary>
		/// <para>
		/// Indicates whether the solver will record the step
		/// whose name or kind is full house.
		/// </para>
		/// <para>The value is <see langword="true"/> in default case.</para>
		/// </summary>
		/// <remarks>
		/// <b>Full house</b>s are the techniques that used in a single
		/// region. When the specified region has only one empty cell,
		/// the full house will be found at this empty cell (the last
		/// value in this region).
		/// </remarks>
		public bool EnableFullHouse { get; set; } = true;

		/// <summary>
		/// <para>
		/// Indicates whether the solver enables the garbage collection
		/// after finished searching a technique whose searcher is
		/// high space-complexity.
		/// </para>
		/// <para>This value is <see langword="true"/> in default case.</para>
		/// </summary>
		public bool EnableGarbageCollectionForcedly { get; set; } = true;

		/// <summary>
		/// <para>
		/// Indicates whether the solver will record the step
		/// whose name or kind is last digit.
		/// </para>
		/// <para>The value is <see langword="true"/> in default case.</para>
		/// </summary>
		/// <remarks>
		/// <b>Last digit</b>s are the techniques that used in a single
		/// digit. When the whole grid has 8 same digits, the last
		/// one will be always found and set in the last position,
		/// which is last digit.
		/// </remarks>
		public bool EnableLastDigit { get; set; } = true;

		/// <summary>
		/// <para>
		/// Indicates whether the solver will apply multiple technique usages
		/// at the same time if searched more than one technique.
		/// As for the option <see cref="AnalyzeDifficultyStrictly"/> is
		/// <see langword="true"/>, when searched more than one technique
		/// instance which holds a same difficulty, the searcher will apply
		/// them at the same time; however if the value is <see langword="false"/>,
		/// the solver will apply all same techniques searched at the same time.
		/// </para>
		/// <para>
		/// The value is <see langword="false"/> in default case. If the value
		/// is <see langword="true"/>, the solver will enable this mode to
		/// accelerate the running, but the applied techniques will be added much
		/// more than when the value is <see langword="false"/>.
		/// </para>
		/// </summary>
		public bool FastSearch { get; set; }

		/// <summary>
		/// <para>
		/// Indicates whether the solver will check templates before searching
		/// Hobiwan's fish.
		/// </para>
		/// <para>
		/// The value is <see langword="true"/> in default case. If the value
		/// is <see langword="true"/>, the solver will check templates first,
		/// and get all possible eliminations for each digit. If the digit does
		/// not contain any elimination, the digit won't exist any fish.
		/// </para>
		/// </summary>
		public bool HobiwanFishCheckTemplates { get; set; } = true;

		/// <summary>
		/// <para>
		/// Indicates whether the step accumulator only records template delete,
		/// and template set won't be in this collection (if necessary).
		/// </para>
		/// <para>
		/// If the value is <see langword="true"/>, and the solver has checked
		/// all template steps, only template deletes in these steps will be
		/// recorded into the step accumulator.
		/// </para>
		/// <para>The value is <see langword="true"/> in default case.</para>
		/// </summary>
		public bool OnlyRecordTemplateDelete { get; set; } = true;

		/// <summary>
		/// <para>
		/// Indicates whether the searcher will only display same-level techniques
		/// in Find All Steps tab page.
		/// </para>
		/// <para>The value is <see langword="true"/> in default case.</para>
		/// </summary>
		public bool OnlyShowSameLevelTechniquesInFindAllSteps { get; set; } = true;

		/// <summary>
		/// <para>Indicates whether the solver will optimizes the applying order.</para>
		/// <para>
		/// When the value is <see langword="true"/>, the result to apply to
		/// the grid will be the one which has the minimum difficulty
		/// rating; otherwise, the applying step will be the first one
		/// of all steps being searched.
		/// </para>
		/// <para>
		/// The value is <see langword="false"/> in default case. If the value
		/// is <see langword="true"/>, the option <see cref="AnalyzeDifficultyStrictly"/>
		/// will be disabled.
		/// </para>
		/// </summary>
		/// <seealso cref="AnalyzeDifficultyStrictly"/>
		public bool OptimizedApplyingOrder { get; set; }

		/// <summary>
		/// <para>
		/// Indicates whether the solver should order all technique searchers
		/// by its priority.
		/// </para>
		/// <para>
		/// The value is <see langword="false"/> in default case. In addition,
		/// if you enable the option <see cref="AnalyzeDifficultyStrictly"/>,
		/// this option will be disabled because the solver will enable the
		/// function of count on all steps and get one with the <b>minimum</b>
		/// difficulty of them.
		/// </para>
		/// </summary>
		/// <seealso cref="AnalyzeDifficultyStrictly"/>
		public bool UseCalculationPriority { get; set; }

		/// <summary>
		/// <para>
		/// Indicates whether the solver should use extended BUG checker
		/// to searcher for all true candidates no matter how difficult
		/// the true candidates looking for.
		/// </para>
		/// <para>
		/// The value is <see langword="true"/> in debug environment,
		/// and <see langword="false"/> in release environment.
		/// </para>
		/// </summary>
		public bool UseExtendedBugSearcher { get; set; }
#if AUTHOR_RESERVED
		= true;
#endif

		/// <summary>
		/// <para>
		/// Indicates whether the solver should check advanced eliminations
		/// during finding exocets.
		/// </para>
		/// <para>The value is <see langword="true"/> in default case.</para>
		/// </summary>
		public bool CheckAdvancedInExocet { get; set; } = true;

		/// <summary>
		/// <para>
		/// Indicates whether the solver will show cross hatching information single
		/// techniques).
		/// </para>
		/// <para>The value is <see langword="true"/> in debugger mode, and
		/// <see langword="false"/> in release mode in default case.</para>
		/// </summary>
		public bool ShowDirectLines { get; set; }
#if AUTHOR_RESERVED
		= true;
#endif

		/// <summary>
		/// <para>
		/// Indicates what size of the Hobiwan's fish will be searched for.
		/// </para>
		/// <para>
		/// The value is <c>3</c> in default case. The maximum value supporting
		/// is <c>7</c>.
		/// </para>
		/// </summary>
		public int HobiwanFishMaximumSize { get; set; } = 3;

		/// <summary>
		/// <para>
		/// Indicates how many exo-fins in Hobiwan's fish will be searched for.
		/// </para>
		/// <para>The value is <c>3</c> in default case.</para>
		/// </summary>
		public int HobiwanFishMaximumExofinsCount { get; set; } = 3;

		/// <summary>
		/// <para>
		/// Indicates how many endo-fins in Hobiwan's fish will be searched for.
		/// </para>
		/// <para>The value is <c>1</c> in default case.</para>
		/// </summary>
		public int HobiwanFishMaximumEndofinsCount { get; set; } = 1;

		/// <summary>
		/// <para>
		/// Indicates the number of nodes to be searched for in bowman bingos.
		/// </para>
		/// <para>
		/// The value is <c>32</c> in default case. You can let this value
		/// be higher because this value take a little influence on the solver.
		/// However, each unique solution has more than 17 hints (given digits),
		/// which means you can't set this value more than <c>64</c> (81 - 17 = 64).
		/// </para>
		/// </summary>
		public int BowmanBingoMaximumLength { get; set; } = 32;

		/// <summary>
		/// <para>
		/// Indicates all regular wings with the size less than
		/// or equals to this specified value. This value should
		/// be between 3 and 5.
		/// </para>
		/// <para>The value is <c>5</c> in default case.</para>
		/// </summary>
		/// <remarks>
		/// In fact this value can be 9 at most (i.e. <c>value &gt;&#61; 3
		/// &amp;&amp; value &lt;&#61; 9</c>) theoretically.
		/// </remarks>
		public int CheckRegularWingSize { get; set; } = 5;

		/// <summary>
		/// <para>Indicates the max petals of death blossom to search.</para>
		/// <para>The value is <c>5</c> in default case.</para>
		/// </summary>
		public int MaxPetalsOfDeathBlossom { get; set; } = 5;
	}
}
