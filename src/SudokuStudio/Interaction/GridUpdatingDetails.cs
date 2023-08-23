namespace SudokuStudio.Interaction;

/// <summary>
/// A set of grid updating details.
/// </summary>
/// <param name="Method">The method that describes which way to update grid.</param>
/// <param name="ClearStack">A <see cref="bool"/> value indicating whether the undo and redo stacks should be reset.</param>
/// <param name="WhileUndoingOrRedoing">A <see cref="bool"/> value indicating whether the environment is in undoing or redoing operation.</param>
internal sealed record GridUpdatingDetails(PuzzleUpdatingMethod Method, bool ClearStack, bool WhileUndoingOrRedoing);
