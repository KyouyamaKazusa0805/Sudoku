namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Provides with an <b>Anonymous Deadly Pattern</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Anonymous Deadly Pattern Type 1</item>
/// <item>Anonymous Deadly Pattern Type 2</item>
/// <item>Anonymous Deadly Pattern Type 3</item>
/// <item>Anonymous Deadly Pattern Type 4</item>
/// </list>
/// </summary>
[StepSearcher(
	"StepSearcherName_AnonymousDeadlyPatternStepSearcher",
	Technique.AnonymousDeadlyPatternType1, Technique.AnonymousDeadlyPatternType2,
	Technique.AnonymousDeadlyPatternType3, Technique.AnonymousDeadlyPatternType4)]
public sealed partial class AnonymousDeadlyPatternStepSearcher : StepSearcher
{
	/// <inheritdoc/>
	protected internal override Step? Collect(ref StepAnalysisContext context)
	{
		if (Collect_8Cells(ref context) is { } eightCellsStep)
		{
			return eightCellsStep;
		}
		if (Collect_9Cells(ref context) is { } nineCellsStep)
		{
			return nineCellsStep;
		}
		return null;
	}

	/// <remarks>
	/// There're 2 patterns:
	/// <code><![CDATA[
	/// (1) UL + XR, 3 digits
	/// 12 .  .  |  23 .  .  |  31 .  .
	/// .  21 .  |  32 .  .  |  13 .  .
	/// .  .  .  |  .  .  .  |  .  .  .
	/// --------- ----------- ---------
	/// 21 12 .  |  .  .  .  |  .  .  .
	/// .  .  .  |  .  .  .  |  .  .  .
	/// .  .  .  |  .  .  .  |  .  .  .
	///
	/// (2) XR + UR, 3 digits
	/// 12 23 .  |  31 .  .  |  .  .  .
	/// .  .  31 |  13 .  .  |  .  .  .
	/// .  .  .  |  .  .  .  |  .  .  .
	/// --------- ----------- ---------
	/// 21 32 13 |  .  .  .  |  .  .  .
	/// .  .  .  |  .  .  .  |  .  .  .
	/// .  .  .  |  .  .  .  |  .  .  .
	/// ]]></code>
	/// </remarks>
	private AnonymousDeadlyPatternStep? Collect_8Cells(ref StepAnalysisContext context)
	{
		return null;
	}

	/// <remarks>
	/// There're 3 patterns:
	/// <code><![CDATA[
	/// (1) 2ULs, 3 digits
	/// 12 .  .  |  21 .  .  |  .  .  .
	/// .  23 .  |  32 .  .  |  .  .  .
	/// .  .  31 |  13 .  .  |  .  .  .
	/// --------- ----------- ---------
	/// 21 32 13 |  .  .  .  |  .  .  .
	/// .  .  .  |  .  .  .  |  .  .  .
	/// .  .  .  |  .  .  .  |  .  .  .
	///
	/// (2) XR + UL, 4 digits
	/// 12 24 .  |  41 .  .  |  .  .  .
	/// 31 .  43 |  14 .  .  |  .  .  .
	/// .  .  .  |  .  .  .  |  .  .  .
	/// --------- ----------- ---------
	/// 23 42 34 |  .  .  .  |  .  .  .
	/// .  .  .  |  .  .  .  |  .  .  .
	/// .  .  .  |  .  .  .  |  .  .  .
	///
	/// (3) UR + UL, 3 digits
	/// 12 .  .  |  21 .  .  |  .  .  .
	/// 23 .  .  |  .  .  .  |  32 .  .
	/// .  31 .  |  12 .  .  |  23 .  .
	/// --------- ----------- ---------
	/// 31 13 .  |  .  .  .  |  .  .  .
	/// .  .  .  |  .  .  .  |  .  .  .
	/// .  .  .  |  .  .  .  |  .  .  .
	/// ]]></code>
	/// </remarks>
	private AnonymousDeadlyPatternStep? Collect_9Cells(ref StepAnalysisContext context)
	{
		return null;
	}
}
