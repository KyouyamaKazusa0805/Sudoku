namespace Sudoku.Solving.Manual.Exocets;

/// <summary>
/// Encapsulates a <b>junior exocet</b> (JE) technique searcher.
/// </summary>
public sealed partial class JeStepSearcher : ExocetStepSearcher
{
	/// <summary>
	/// Indicates the searcher properties.
	/// </summary>
	/// <remarks>
	/// Please note that all technique searches should contain
	/// this static property in order to display on settings window. If the searcher doesn't contain,
	/// when we open the settings window, it'll throw an exception to report about this.
	/// </remarks>
	public static TechniqueProperties Properties { get; } = new(34, nameof(Technique.Je))
	{
		DisplayLevel = 4
	};


	/// <inheritdoc/>
	public override unsafe void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
	{
		// TODO: Extend JE eliminations checking.

		// We'll introduce the algorithm using this puzzle:
		// .-------------------------.---------------------.------------------------.
		// | 123578  23567     13568 | 4      567 B  267 B | 235679   236789  23589 |
		// | 4       23567 TQ  356   | 2567   8      9     | 1    TR  2367    235   |
		// | 2578    9     TQ  568   | 2567   3      1     | 2567 TR  24678   2458  |
		// :-------------------------+---------------------+------------------------:
		// | 123     234       1349  | 1278   1479   5     | 2379     123789  6     |
		// | 125     8         159   | 3      1679   267   | 4        1279    129   |
		// | 6       234       7     | 128    149    248   | 239      5       12389 |
		// :-------------------------+---------------------+------------------------:
		// | 357     1         3456  | 9      4567   3467  | 8        2346    2345  |
		// | 3578    34567     2     | 15678  14567  34678 | 3569     13469   13459 |
		// | 9       3456      34568 | 1568   2      3468  | 356      1346    7     |
		// '-------------------------'---------------------'------------------------'

		foreach (var exocet in Patterns)
		{
			var (b1, b2, tq1, tq2, tr1, tr2, s, mq1, mq2, mr1, mr2, _, targetMap) = exocet;

			// Base cells should be empty:
			// {567} and {267} are empty cells.
			if (grid.GetStatus(b1) != CellStatus.Empty || grid.GetStatus(b2) != CellStatus.Empty)
			{
				continue;
			}

			// The number of different candidates in base cells can't be greater than 5:
			// {267} | {567} = {2567}, Count{2567} = 4, which is <= 5.
			short baseCands = (short)(grid.GetCandidates(b1) | grid.GetCandidates(b2));
			if (PopCount((uint)baseCands) > 5)
			{
				continue;
			}

			// At least one cell in the target cells should be empty:
			// {23567}, {2567} are empty cells.
			if ((targetMap & EmptyMap).IsEmpty)
			{
				continue;
			}

			// Then check target eliminations.
			// Here 'nonBaseQ' and 'nonBaseR' are the conjugate pair (or AHS) digits
			// in target Q and target R cells pair.
			if (!CheckTarget(grid, tq1, tq2, baseCands, out short nonBaseQ)
				|| !CheckTarget(grid, tr1, tr2, baseCands, out short nonBaseR))
			{
				continue;
			}

			// Get all locked members.
			int[] mq1o = mq1.ToArray(), mq2o = mq2.ToArray(), mr1o = mr1.ToArray(), mr2o = mr2.ToArray();
			int v1 = grid.GetCandidates(mq1o[0]) | grid.GetCandidates(mq1o[1]);
			int v2 = grid.GetCandidates(mq2o[0]) | grid.GetCandidates(mq2o[1]);
			short halfNeedChecking = (short)(v1 | v2), needChecking = (short)(baseCands & halfNeedChecking);

			v1 = grid.GetCandidates(mr1o[0]) | grid.GetCandidates(mr1o[1]);
			v2 = grid.GetCandidates(mr2o[0]) | grid.GetCandidates(mr2o[1]);
			halfNeedChecking = (short)(v1 | v2);
			needChecking &= halfNeedChecking;

			// Check crossline.
			if (!CheckCrossline(s, needChecking))
			{
				continue;
			}

			// Gather highlight cells and candidates.
			var cellOffsets = new List<DrawingInfo> { new(0, b1), new(0, b2) };
			var candidateOffsets = new List<DrawingInfo>();
			foreach (int digit in grid.GetCandidates(b1))
			{
				candidateOffsets.Add(new(0, b1 * 9 + digit));
			}
			foreach (int digit in grid.GetCandidates(b2))
			{
				candidateOffsets.Add(new(0, b2 * 9 + digit));
			}

			// Check target eliminations.
			var targetElimsMap = Candidates.Empty;
			short baseCandsWithAhsOrConjugatePair = (short)(nonBaseQ > 0 ? baseCands | nonBaseQ : baseCands);

			// Here we can't replace the operator '|' with '||', because two methods both should be called.
			if (GatherBasic(grid, ref targetElimsMap, tq1, baseCands, baseCandsWithAhsOrConjugatePair)
				| GatherBasic(grid, ref targetElimsMap, tq2, baseCands, baseCandsWithAhsOrConjugatePair)
				&& nonBaseQ > 0
				&& grid.GetStatus(tq1) == CellStatus.Empty ^ grid.GetStatus(tq2) == CellStatus.Empty)
			{
				int conjugatPairDigit = TrailingZeroCount(nonBaseQ);
				if (grid.Exists(tq1, conjugatPairDigit) is true)
				{
					candidateOffsets.Add(new(1, tq1 * 9 + conjugatPairDigit));
				}
				if (grid.Exists(tq2, conjugatPairDigit) is true)
				{
					candidateOffsets.Add(new(1, tq2 * 9 + conjugatPairDigit));
				}
			}

			baseCandsWithAhsOrConjugatePair = (short)(nonBaseR > 0 ? baseCands | nonBaseR : baseCands);
			if (GatherBasic(grid, ref targetElimsMap, tr1, baseCands, baseCandsWithAhsOrConjugatePair)
				| GatherBasic(grid, ref targetElimsMap, tr2, baseCands, baseCandsWithAhsOrConjugatePair)
				&& nonBaseR > 0
				&& grid.GetStatus(tr1) == CellStatus.Empty ^ grid.GetStatus(tr2) == CellStatus.Empty)
			{
				int conjugatPairDigit = TrailingZeroCount(nonBaseR);
				if (grid.Exists(tr1, conjugatPairDigit) is true)
				{
					candidateOffsets.Add(new(1, tr1 * 9 + conjugatPairDigit));
				}
				if (grid.Exists(tr2, conjugatPairDigit) is true)
				{
					candidateOffsets.Add(new(1, tr2 * 9 + conjugatPairDigit));
				}
			}

			// Check whether the result contains any valid eliminations.
			if (targetElimsMap.IsEmpty)
			{
				continue;
			}

			// In the end, we should append the highlight cells, and then add into the accumulator.
			cellOffsets.Add(new(1, tq1));
			cellOffsets.Add(new(1, tq2));
			cellOffsets.Add(new(1, tr1));
			cellOffsets.Add(new(1, tr2));
			foreach (int cell in s)
			{
				cellOffsets.Add(new(2, cell));
			}

			var targetElims = new Elimination(targetElimsMap, EliminatedReason.Basic);
			accumulator.Add(
				new JeStepInfo(
					new View[] { new() { Cells = cellOffsets, Candidates = candidateOffsets } },
					exocet,
					baseCands.GetAllSets().ToArray(),
					null,
					null,
					new Elimination[] { targetElims }
				)
			);
		}
	}

	private partial bool GatherBasic(in SudokuGrid grid, ref Candidates elims, int cell, short baseCands, short baseCandsWithAhsOrConjugatePair);

	private partial bool CheckCrossline(in Cells crossline, short needChecking);
	private unsafe partial bool CheckTarget(in SudokuGrid grid, int pos1, int pos2, int baseCands, out short ahsOrConjugatePairCands);
}
