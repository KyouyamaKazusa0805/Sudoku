using System.Collections.Generic;
using System.Drawing;
using Sudoku.Solving;
using Sudoku.Solving.Manual;

namespace Sudoku.Forms
{
	partial class Settings
	{
		/// <summary>
		/// <para>Indicates whether the form shows candidates.</para>
		/// <para>The value is <see langword="true"/> in default case.</para>
		/// </summary>
		public bool ShowCandidates { get; set; } = true;

		/// <summary>
		/// <para>
		/// Indicates whether the program ask you "Do you want to quit?" while
		/// you clicked the close button.
		/// </para>
		/// <para>The value is <see langword="true"/> in default case.</para>
		/// </summary>
		public bool AskWhileQuitting { get; set; } = true;

		/// <summary>
		/// <para>
		/// Indicates whether the program use Sudoku Explainer mode to check
		/// the difficulty rating of each puzzle strictly.
		/// </para>
		/// <para>
		/// The value is <see langword="false"/> in default case. If the value
		/// is <see langword="false"/>, the mode will be Hodoku compatible mode,
		/// where all puzzles will be solved preferring techniques instead of
		/// their difficulty ratings.
		/// </para>
		/// </summary>
		public bool AnalyzeDifficultyStrictly { get; set; } = false;

		/// <summary>
		/// <para>
		/// Indicates whether the searcher will use fast search to search all
		/// steps. This option is equivalent to <see cref="ManualSolver.FastSearch"/>.
		/// </para>
		/// <para>The value is <see langword="false"/> in default case.</para>
		/// </summary>
		/// <seealso cref="ManualSolver.FastSearch"/>
		public bool FastSearch { get; set; } = false;

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
		/// Indicates whether two ALSes can be overlapped with each other.
		/// </para>
		/// <para>The value is <see langword="true"/> in default case.</para>
		/// </summary>
		public bool AllowOverlapAlses { get; set; } = true;

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
		/// Indicates whether the solver will check the chain finally forms a
		/// continuous nice loop. If so, the structure may eliminate more candidates
		/// than those of normal AICs.
		/// </para>
		/// <para>The value is <see langword="true"/> in default case.</para>
		/// </summary>
		public bool CheckContinuousNiceLoop { get; set; } = true;

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

		/// <summary>
		/// <para>
		/// Indicates whether the solver will solve and analyze the puzzle
		/// from the current grid, if the current grid has applied some steps.
		/// </para>
		/// <para>The value is <see langword="false"/> in default case.</para>
		/// </summary>
		public bool SolveFromCurrent { get; set; } = false;

		/// <summary>
		/// <para>
		/// Indicates whether the text format of the sudoku grid will use zero
		/// character '<c>0</c>' to be placeholders instead of dot character
		/// '<c>.</c>'.
		/// </para>
		/// <para>
		/// The value is <see langword="true"/> in default case. If the value is
		/// <see langword="true"/>, the placeholders will be '<c>.</c>'.
		/// </para>
		/// </summary>
		public bool TextFormatPlaceholdersAreZero { get; set; } = true;

		/// <summary>
		/// <para>
		/// Indicates whether the text format of pencil-marked grid will use
		/// compatible mode to output (without given and modifiable values
		/// notations).
		/// </para>
		/// <para>The value is <see langword="false"/> in default case.</para>
		/// </summary>
		public bool PmGridCompatible { get; set; } = false;

		/// <summary>
		/// <para>
		/// Indicates the grid line width of the sudoku grid to render.
		/// </para>
		/// <para>The value is <c>1.5F</c> in default case.</para>
		/// </summary>
		public float GridLineWidth { get; set; } = 1.5F;

		/// <summary>
		/// <para>
		/// Indicates the block line width of the sudoku grid to render.
		/// </para>
		/// <para>The value is <c>5F</c> in default case.</para>
		/// </summary>
		public float BlockLineWidth { get; set; } = 5F;

		/// <summary>
		/// <para>
		/// Indicates the maximum length of a chain to search.
		/// </para>
		/// <para>The value is <c>10</c> in default case.</para>
		/// </summary>
		public int AicMaximumLength { get; set; } = 10;

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
		/// Indicates which item you selected the generating combo box.
		/// </para>
		/// <para>The default value is <c>0</c>.</para>
		/// </summary>
		public int GeneratingModeComboBoxSelectedIndex { get; set; } = 0;

		/// <summary>
		/// <para>
		/// Indicates which item you selected the generating symmetry combo box.
		/// </para>
		/// <para>The default value is <c>0</c>.</para>
		/// </summary>
		public int GeneratingSymmetryModeComboBoxSelectedIndex { get; set; } = 0;

		/// <summary>
		/// <para>
		/// Indicates the combo box of backdoor depth you selected.
		/// </para>
		/// <para>The default value is <c>0</c>.</para>
		/// </summary>
		public int GeneratingBackdoorSelectedIndex { get; set; } = 0;

		/// <summary>
		/// <para>
		/// Indicates the current puzzle number used in the database.
		/// </para>
		/// <para>
		/// The default value is <c>-1</c>, which means the database is unavailable.
		/// </para>
		/// </summary>
		public int CurrentPuzzleNumber { get; set; } = -1;

		/// <summary>
		/// <para>Indicates the scale of values.</para>
		/// <para>The value is <c>0.8M</c> in default case.</para>
		/// </summary>
		public decimal ValueScale { get; set; } = .8M;

		/// <summary>
		/// <para>Indicates the scale of candidates.</para>
		/// <para>The value is <c>0.3M</c> in default case.</para>
		/// </summary>
		public decimal CandidateScale { get; set; } = .3M;

		/// <summary>
		/// <para>
		/// Indicates the font of given digits to render.
		/// </para>
		/// <para>The value is <c>"Arial"</c> in default case.</para>
		/// </summary>
		public string GivenFontName { get; set; } = "Arial";

		/// <summary>
		/// <para>
		/// Indicates the font of modifiable digits to render.
		/// </para>
		/// <para>The value is <c>"Arial"</c> in default case.</para>
		/// </summary>
		public string ModifiableFontName { get; set; } = "Arial";

		/// <summary>
		/// <para>
		/// Indicates the font of candidate digits to render.
		/// </para>
		/// <para>The value is <c>"Arial"</c> in default case.</para>
		/// </summary>
		public string CandidateFontName { get; set; } = "Arial";

		/// <summary>
		/// <para>
		/// Indicates the current puzzle database used.
		/// </para>
		/// <para>The default value is <c><see langword="null"/></c>.</para>
		/// </summary>
		public string? CurrentPuzzleDatabase { get; set; } = null;

		/// <summary>
		/// <para>
		/// Indicates the background color of the sudoku grid to render.
		/// </para>
		/// <para>The value is <see cref="Color.White"/> in default case.</para>
		/// </summary>
		public Color BackgroundColor { get; set; } = Color.White;

		/// <summary>
		/// <para>
		/// Indicates the grid line color of the sudoku grid to render.
		/// </para>
		/// <para>The value is <see cref="Color.Black"/></para> in default case.
		/// </summary>
		public Color GridLineColor { get; set; } = Color.Black;

		/// <summary>
		/// <para>
		/// Indicates the block line color of the sudoku grid to render.
		/// </para>
		/// <para>The value is <see cref="Color.Black"/> in default case.</para>
		/// </summary>
		public Color BlockLineColor { get; set; } = Color.Black;

		/// <summary>
		/// <para>Indicates the given digits to render.</para>
		/// <para>The value is <see cref="Color.Black"/> in default case.</para>
		/// </summary>
		public Color GivenColor { get; set; } = Color.Black;

		/// <summary>
		/// <para>Indicates the modifiable digits to render.</para>
		/// <para>The value is <see cref="Color.Blue"/> in default case.</para>
		/// </summary>
		public Color ModifiableColor { get; set; } = Color.Blue;

		/// <summary>
		/// <para>Indicates the candidate digits to render.</para>
		/// <para>The value is <see cref="Color.DimGray"/> in default case.</para>
		/// </summary>
		public Color CandidateColor { get; set; } = Color.DimGray;

		/// <summary>
		/// <para>Indicates the color used for painting for focused cells.</para>
		/// <para>
		/// The value is <c>#20FFFF00</c> (<see cref="Color.Yellow"/>
		/// with alpha <c>32</c>) in default case.
		/// </para>
		/// </summary>
		public Color FocusedCellColor { get; set; } = Color.FromArgb(32, Color.Yellow);

		/// <summary>
		/// Indicates the elimination color.
		/// </summary>
		public Color EliminationColor { get; set; } = Color.FromArgb(255, 118, 132);

		/// <summary>
		/// Indicates the cannibalism color.
		/// </summary>
		public Color CannibalismColor { get; set; } = Color.FromArgb(235, 0, 0);

		/// <summary>
		/// Indicates the chain color.
		/// </summary>
		public Color ChainColor { get; set; } = Color.Red;

		/// <summary>
		/// Indicates the color 1.
		/// </summary>
		public Color Color1 { get; set; } = Color.FromArgb(63, 218, 101);

		/// <summary>
		/// Indicates the color 2.
		/// </summary>
		public Color Color2 { get; set; } = Color.FromArgb(255, 192, 89);

		/// <summary>
		/// Indicates the color 3.
		/// </summary>
		public Color Color3 { get; set; } = Color.FromArgb(127, 187, 255);

		/// <summary>
		/// Indicates the color 4.
		/// </summary>
		public Color Color4 { get; set; } = Color.FromArgb(216, 178, 255);

		/// <summary>
		/// Indicates the color 5.
		/// </summary>
		public Color Color5 { get; set; } = Color.FromArgb(197, 232, 140);

		/// <summary>
		/// Indicates the color 6.
		/// </summary>
		public Color Color6 { get; set; } = Color.FromArgb(255, 203, 203);

		/// <summary>
		/// Indicates the color 7.
		/// </summary>
		public Color Color7 { get; set; } = Color.FromArgb(178, 223, 223);

		/// <summary>
		/// Indicates the color 8.
		/// </summary>
		public Color Color8 { get; set; } = Color.FromArgb(252, 220, 165);

		/// <summary>
		/// Indicates the color 9.
		/// </summary>
		public Color Color9 { get; set; } = Color.FromArgb(255, 255, 150);

		/// <summary>
		/// Indicates the color 10.
		/// </summary>
		public Color Color10 { get; set; } = Color.FromArgb(247, 222, 143);

		/// <summary>
		/// Indicates the color 11.
		/// </summary>
		public Color Color11 { get; set; } = Color.FromArgb(220, 212, 252);

		/// <summary>
		/// Indicates the color 12.
		/// </summary>
		public Color Color12 { get; set; } = Color.FromArgb(255, 210, 210);

		/// <summary>
		/// Indicates the color 13.
		/// </summary>
		public Color Color13 { get; set; } = Color.FromArgb(206, 251, 237);

		/// <summary>
		/// Indicates the color 14.
		/// </summary>
		public Color Color14 { get; set; } = Color.FromArgb(215, 255, 215);

		/// <summary>
		/// Indicates the color 15.
		/// </summary>
		public Color Color15 { get; set; } = Color.FromArgb(192, 192, 192);

		/// <summary>
		/// The main manual solver.
		/// </summary>
		public ManualSolver MainManualSolver { get; set; } = new ManualSolver();

		/// <summary>
		/// The color table for each difficulty level.
		/// </summary>
		public IDictionary<DifficultyLevel, (Color _foreground, Color _background)> DiffColors
		{
			get
			{
				return new Dictionary<DifficultyLevel, (Color, Color)>
				{
					[DifficultyLevel.Unknown] = (Color.Black, Color.Gray),
					[DifficultyLevel.VeryEasy] = (Color.Black, Color.FromArgb(204, 204, 255)),
					[DifficultyLevel.Easy] = (Color.Black, Color.FromArgb(204, 204, 255)),
					[DifficultyLevel.Moderate] = (Color.Black, Color.FromArgb(100, 255, 100)),
					[DifficultyLevel.Advanced] = (Color.Black, Color.FromArgb(100, 255, 100)),
					[DifficultyLevel.Hard] = (Color.Black, Color.FromArgb(255, 255, 100)),
					[DifficultyLevel.VeryHard] = (Color.Black, Color.FromArgb(255, 255, 100)),
					[DifficultyLevel.Fiendish] = (Color.Black, Color.FromArgb(255, 150, 80)),
					[DifficultyLevel.Diabolical] = (Color.Black, Color.FromArgb(255, 150, 80)),
					[DifficultyLevel.Crazy] = (Color.Black, Color.FromArgb(255, 100, 100)),
					[DifficultyLevel.Nightmare] = (Color.Black, Color.FromArgb(255, 100, 100)),
					[DifficultyLevel.BeyondNightmare] = (Color.Black, Color.FromArgb(255, 100, 100)),
					[DifficultyLevel.LastResort] = (Color.Black, Color.FromArgb(255, 100, 100))
				};
			}
		}

		/// <summary>
		/// Indicates the palette colors.
		/// </summary>
		public IDictionary<int, Color> PaletteColors
		{
			get
			{
				return new Dictionary<int, Color>
				{
					[-4] = Color8, [-3] = Color7, [-2] = Color6, [-1] = Color5,
					[0] = Color1, [1] = Color2, [2] = Color3,
					[3] = Color4, [4] = Color9, [5] = Color10,
					[6] = Color11, [7] = Color12, [8] = Color13,
					[9] = Color14, [10] = Color15,
				};
			}
		}
	}
}
