namespace SudokuStudio.Views.Commands;

/// <summary>
/// Defines a command to fix a grid.
/// </summary>
public sealed class FixGridCommand : ButtonCommand
{
	/// <inheritdoc/>
	public override void Execute(object? parameter)
	{
		if (parameter is not SudokuPane { Puzzle: var modified } pane)
		{
			return;
		}

		modified.Fix();
		pane.Puzzle = modified;
	}
}
