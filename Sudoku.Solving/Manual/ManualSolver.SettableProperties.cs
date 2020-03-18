namespace Sudoku.Solving.Manual
{
	partial class ManualSolver
	{
		/// <summary>
		/// <para>
		/// Indicates the maximum length of a chain to search.
		/// </para>
		/// <para>The value is <c>10</c> in default case.</para>
		/// </summary>
		public int AicMaximumLength { get; set; } = 10;

		/// <summary>
		/// <para>
		/// Indicates whether two ALSes can be overlapped with each other.
		/// </para>
		/// <para>The value is <see langword="true"/> in default case.</para>
		/// </summary>
		public bool AllowOverlapAlses { get; set; } = true;

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
		/// words, this option does not work while solving when the condition
		/// is satisfied. However, the time of calculation with this value
		/// <see langword="true"/> will be <b>much slower</b> than that with the
		/// value is <see langword="false"/>.
		/// </para>
		/// </summary>
		/// <seealso cref="OptimizedApplyingOrder"/>
		public bool AnalyzeDifficultyStrictly { get; set; } = false;

		/// <summary>
		/// <para>
		/// Indicates the number of nodes to be searched for in bowman bingos.
		/// </para>
		/// <para>
		/// The value is <c>32</c> in default case. You can let this value
		/// be higher because this value take a little influence on the solver.
		/// However, each unique solution has more than 17 hints (given digits),
		/// which means you cannot set this value more than <c>64</c> (81 - 17 = 64).
		/// </para>
		/// </summary>
		public int BowmanBingoMaximumLength { get; set; } = 32;

		/// <summary>
		/// <para>
		/// Indicates whether the solver should check the technique Almost
		/// Locked Quadruple (ALQ).
		/// </para>
		/// <para>
		/// The value is <see langword="false"/> in default case.
		/// </para>
		/// </summary>
		public bool CheckAlmostLockedQuadruple { get; set; } = false;

		/// <summary>
		/// <para>
		/// Indicates whether the solver will check the validity of the conclusions
		/// after searched them. If the conclusions eliminate the wrong digits or
		/// assign to the wrong cells, it will report the error
		/// (i.e. throw a <see cref="WrongHandlingException"/>).
		/// </para>
		/// <para>
		/// The value is <see langword="true"/> in default case. If the value is
		/// <see langword="true"/>, all conclusions will be checked before applying
		/// to the grid. The comparer is the solution grid. Computer does not know
		/// which conclusions are correct and which ones are incorrect. Therefore,
		/// the best plan is to compare to the solution grid. If not, the solver
		/// will not check the validity of all conclusions. In other words, the solver
		/// does not stop the searching until the grid is totally invalid (None of
		/// eliminations or assignments can be searched). However, unfortunately,
		/// the grid has no solution at present.
		/// </para>
		/// </summary>
		/// <seealso cref="WrongHandlingException"/>
		public bool CheckConclusionValidityAfterSearched { get; set; } = true;

		/// <summary>
		/// <para>
		/// Indicates whether the solver will check the chain finally forms a
		/// continuous nice loop. If so, the structure may eliminate more candidates
		/// than those of normal AICs.
		/// </para>
		/// <para>The value is <see langword="true"/> in default case.</para>
		/// </summary>
		public bool CheckContinuousNiceLoop { get; set; } = true;

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
		/// If the puzzle is so easy (in other words, the grid does not need check
		/// it), this option will make the difficulty rating of the puzzle much more
		/// higher than that when the option is <see langword="false"/>. In addition,
		/// if the option <see cref="AnalyzeDifficultyStrictly"/> is <see langword="true"/>,
		/// this option will be ignored.
		/// </para>
		/// </summary>
		/// <seealso cref="AnalyzeDifficultyStrictly"/>
		public bool CheckGurthSymmetricalPlacement { get; set; } = false;

		/// <summary>
		/// <para>
		/// Indicates whether the solver will check head collision in searching for
		/// AICs.
		/// </para>
		/// <para>
		/// If the value is <see langword="true"/>, the searcher will search for
		/// AICs whose head nodes are same as tail nodes. In this case, the AIC
		/// will raise a conclusion that the head node is absolutely true.
		/// </para>
		/// <para>The value is <see langword="true"/> in default case.</para>
		/// </summary>
		public bool CheckHeadCollision { get; set; } = true;

		/// <summary>
		/// <para>
		/// Indicates whether the solver should check
		/// uncompleted uniqueness patterns.
		/// </para>
		/// <para>The value is <see langword="false"/> in default case.</para>
		/// </summary>
		public bool CheckIncompletedUniquenessPatterns { get; set; } = false;

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
		/// &amp;&amp; value &lt;&#61; 9</c>) theoretically, however the searching
		/// is too low so I do not allow them.
		/// </remarks>
		public int CheckRegularWingSize { get; set; } = 5;

		/// <summary>
		/// <para>
		/// Indicates whether the solver will disable all technique searchers
		/// whose running is extremely slow.
		/// </para>
		/// <para>
		/// If the technique searcher marks the attribute <see cref="SlowAttribute"/>,
		/// the searcher will be regarded as slow one. However, if the technique
		/// searcher marked this attribute, but its property
		/// <see cref="SlowAttribute.SlowButNecessary"/> is <see langword="true"/>,
		/// the searcher will not be disabled.
		/// </para>
		/// <para>The value is <see langword="true"/> in default case.</para>
		/// </summary>
		/// <seealso cref="SlowAttribute"/>
		public bool DisableSlowTechniques { get; set; } = true;

		/// <summary>
		/// <para>
		/// Indicates whether the solver should check all bowman bingos.
		/// </para>
		/// <para>The value is <see langword="true"/> in default case.</para>
		/// </summary>
		public bool EnableBowmanBingo { get; set; } = true;

		/// <summary>
		/// <para>
		/// Indicates whether the solver should add brute force technique information
		/// when the puzzle has a unique solution but cannot be found any manual
		/// techniques.
		/// </para>
		/// <para>The value is <see langword="true"/> in default case.</para>
		/// </summary>
		public bool EnableBruteForce { get; set; } = true;

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
		/// Indicates whether the solver will record all pattern overlay
		/// method technique steps.
		/// </para>
		/// <para>
		/// The value is <see langword="false"/> in default case. Note that
		/// this option makes the solver search single-digit patterns
		/// <c>much more slower</c> because it will use enumeration to iterate on
		/// all possible patterns of a single digit (46656 patterns in total).
		/// </para>
		/// </summary>
		public bool EnablePatternOverlayMethod { get; set; } = false;

		/// <summary>
		/// <para>
		/// Indicates whether the solver will record template techniques
		/// if worth.
		/// </para>
		/// <para>
		/// The value is <see langword="false"/> in default case. In addition,
		/// if the value is <see langword="true"/>, the option
		/// <see cref="OnlyRecordTemplateDelete"/> will be disabled.
		/// </para>
		/// </summary>
		/// <seealso cref="OnlyRecordTemplateDelete"/>.
		public bool EnableTemplate { get; set; } = false;

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
		public bool FastSearch { get; set; } = false;

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
		/// Indicates whether the solver will check templates before searching
		/// Hobiwan's fish.
		/// </para>
		/// <para>
		/// The value is <see langword="true"/> in default case. If the value
		/// is <see langword="true"/>, the solver will check templates first,
		/// and get all possible eliminations for each digit. If the digit does
		/// not contain any elimination, the digit will not exist any fish.
		/// </para>
		/// </summary>
		public bool HobiwanFishCheckTemplates { get; set; } = true;

		/// <summary>
		/// <para>
		/// Indicates whether the step accumulator only records template delete,
		/// and template set will not be in this collection (if necessary).
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
		/// Indicates whether the searcher will save the shortest path in AICs only.
		/// </para>
		/// <para>
		/// The value is <see langword="false"/> in default case. If the value
		/// is <see langword="true"/>, the searcher will check all chains had stored
		/// in the list and get the shortest one, but the speed is slow.
		/// </para>
		/// </summary>
		public bool OnlySaveShortestPathAic { get; set; } = false;

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
		public bool OptimizedApplyingOrder { get; set; } = false;

		/// <summary>
		/// <para>
		/// Indicates whether the solver will record only one AIC
		/// when searched AICs that contain same head node and tail node,
		/// but different path.
		/// </para>
		/// <para>
		/// The value is <see langword="true"/> in default case. If the value
		/// is <see langword="true"/>, the solver will save only one chain with
		/// the condition above, but the length of the chain may not be the shortest
		/// one.
		/// </para>
		/// </summary>
		public bool ReductDifferentPathAic { get; set; } = true;

		/// <summary>
		/// <para>
		/// Indicates whether the solver should order all technique searchers
		/// by its priority.
		/// </para>
		/// <para>
		/// The value is <see langword="true"/> in default case. In addition,
		/// if you enable the option <see cref="AnalyzeDifficultyStrictly"/>,
		/// this option will be disabled because the solver will enable the
		/// function of count on all steps and get one with the <b>minimum</b>
		/// difficulty of them.
		/// </para>
		/// </summary>
		/// <seealso cref="AnalyzeDifficultyStrictly"/>
		public bool UseCalculationPriority { get; set; } = true;

		/// <summary>
		/// <para>
		/// Indicates whether the solver should use extended BUG checker
		/// to searcher for all true candidates no matter how difficult
		/// the true candidates looking for.
		/// </para>
		/// <para>The value is <see langword="false"/> in default case.</para>
		/// </summary>
		public bool UseExtendedBugSearcher { get; set; } = false;
	}
}
