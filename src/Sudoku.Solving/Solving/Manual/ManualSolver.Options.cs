namespace Sudoku.Solving.Manual;

partial class ManualSolver : IManualSolverOptions
{
	/// <inheritdoc/>
	public bool IsHodokuMode { get; set; }

	/// <inheritdoc/>
	public bool IsFastSearching { get; set; }

	/// <inheritdoc/>
	public bool OptimizedApplyingOrder { get; set; }
}
