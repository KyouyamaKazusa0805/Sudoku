using System;
using Sudoku.Data.Meta;

namespace Sudoku.Data.Stepping
{
	/// <summary>
	/// Encapsulates a reset step.
	/// </summary>
	public sealed class ResetStep : Step
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="oldMasks">The old grid masks.</param>
		/// <param name="newMasks">The new grid masks.</param>
		public ResetStep(short[] oldMasks, short[] newMasks) =>
			(OldMasks, NewMasks) = (oldMasks, newMasks);


		/// <summary>
		/// Indicates the table of new grid masks.
		/// </summary>
		public short[] NewMasks { get; }

		/// <summary>
		/// Indicates the table of old grid masks.
		/// </summary>
		public short[] OldMasks { get; }


		/// <inheritdoc/>
		public override void DoStepTo(UndoableGrid grid) => Array.Copy(OldMasks, grid._masks, 81);

		/// <inheritdoc/>
		public override void UndoStepTo(UndoableGrid grid) => Array.Copy(NewMasks, grid._masks, 81);
	}
}
