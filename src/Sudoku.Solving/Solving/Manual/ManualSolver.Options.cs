namespace Sudoku.Solving.Manual;

partial class ManualSolver : IManualSolverOptions
{
	/// <inheritdoc/>
	public bool IsHodokuMode { get; set; } = true;

	/// <inheritdoc/>
	public bool IsFastSearching { get; set; }

	/// <inheritdoc/>
	public bool OptimizedApplyingOrder { get; set; }
}
