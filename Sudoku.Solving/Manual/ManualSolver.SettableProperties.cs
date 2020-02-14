namespace Sudoku.Solving.Manual
{
	partial class ManualSolver
	{
		

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
		/// <see langword="true"/> will be much slower than that with the
		/// value is <see langword="false"/>.
		/// </para>
		/// </summary>
		/// <seealso cref="OptimizedApplyingOrder"/>
		public bool AnalyzeDifficultyStrictly { get; set; } = false;

		#region BowmanBingoMaximumLength
		/// <summary>
		/// The field bound with <see cref="BowmanBingoMaximumLength"/>.
		/// </summary>
		/// <seealso cref="BowmanBingoMaximumLength"/>
		private int _bowmanBingoMaximumLength = 10;

		/// <summary>
		/// <para>
		/// Indicates the number of nodes to be searched for in bowman bingos.
		/// </para>
		/// <para>The value is <c>10</c> in default case.</para>
		/// </summary>
		public int BowmanBingoMaximumLength
		{
			get => _bowmanBingoMaximumLength;
			set
			{
				if (value > 0 && value < 576)
				{
					_bowmanBingoMaximumLength = value;
				}
			}
		}
		#endregion

		/// <summary>
		/// <para>
		/// Indicates whether the solver should check
		/// incompleted uniqueness patterns.
		/// </para>
		/// <para>The value is <see langword="false"/> in default case.</para>
		/// </summary>
		/// <example>
		/// For example, An basic UR pattern should be
		/// <code>
		/// ab ab
		/// ab abc
		/// </code>
		/// But sometimes, some digits will be missed in the pattern
		/// like this:
		/// <code>
		/// ab ab
		/// ab ac
		/// </code>
		/// The candidate <c>a</c> is also can be eliminated.
		/// </example>
		public bool CheckIncompletedUniquenessPatterns { get; set; } = false;

		#region CheckRegularWingSize
		/// <summary>
		/// The field bound with <see cref="CheckRegularWingSize"/>.
		/// </summary>
		/// <seealso cref="CheckRegularWingSize"/>
		private int _checkRegularWingSize = 5;

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
		public int CheckRegularWingSize
		{
			get => _checkRegularWingSize;
			set => _checkRegularWingSize = value >= 3 && value <= 5 ? value : 5;
		}
		#endregion

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
		/// <para>The value is <see langword="true"/> in default case.</para>
		/// </summary>
		public bool EnablePatternOverlayMethod { get; set; } = true;

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
		/// Indicates whether the step accumulator only records templete delete,
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
		/// <para>Indicates the solver will optimizes the applying order.</para>
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
		/// Indicates whether the solver should use extended BUG checker
		/// to searcher for all true candidates no matter how difficult
		/// the true candidates looking for.
		/// </para>
		/// <para>The value is <see langword="false"/> in default case.</para>
		/// </summary>
		public bool UseExtendedBugSearcher { get; set; } = false;
	}
}
