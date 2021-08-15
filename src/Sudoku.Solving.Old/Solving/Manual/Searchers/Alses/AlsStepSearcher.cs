namespace Sudoku.Solving.Manual.Alses;

/// <summary>
/// Encapsulates an <b>almost locked set</b> (ALS) technique searcher.
/// </summary>
public abstract class AlsStepSearcher : StepSearcher
{
	/// <summary>
	/// Indicates whether the ALSes can be overlapped with each other.
	/// </summary>
	public bool AllowOverlapping { get; set; }

	/// <summary>
	/// Indicates whether the ALSes shows their regions rather than cells.
	/// </summary>
	public bool AlsShowRegions { get; set; }

	/// <summary>
	/// Indicates whether the solver will check ALS cycles.
	/// </summary>
	public bool AllowAlsCycles { get; set; }
}
