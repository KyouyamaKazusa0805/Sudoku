namespace Sudoku.Data.Stepping;

/// <summary>
/// Encapsulates a reset step.
/// </summary>
/// <param name="OldMasks">Indicates the table of new grid masks.</param>
/// <param name="NewMasks">Indicates the table of old grid masks.</param>
/// <remarks>
/// Note that <see langword="record"/>s doesn't support pointers at present. Therefore,
/// I use <see cref="IntPtr"/> instead.
/// </remarks>
/// <seealso cref="IntPtr"/>
[Obsolete("In the future, this type won't be used.", false)]
public sealed unsafe record ResetStep(IntPtr OldMasks, IntPtr NewMasks) : IStep
{
	/// <inheritdoc/>
	public unsafe void DoStepTo(UndoableGrid grid)
	{
		fixed (short* pGrid = grid)
		{
			Unsafe.CopyBlock(pGrid, (short*)OldMasks, sizeof(short) * 81);
		}
	}

	/// <inheritdoc/>
	public unsafe void UndoStepTo(UndoableGrid grid)
	{
		fixed (short* pGrid = grid)
		{
			Unsafe.CopyBlock(pGrid, (short*)NewMasks, sizeof(short) * 81);
		}
	}
}
