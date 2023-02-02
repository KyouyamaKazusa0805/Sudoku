namespace SudokuStudio.Interaction.Commands;

/// <summary>
/// Defines a command to unfix a grid.
/// </summary>
public sealed class UnfixGridCommand : ButtonCommand
{
	/// <inheritdoc/>
	public override void Execute(object? parameter)
	{
		if (parameter is not SudokuPane { Puzzle: var modified } pane)
		{
			return;
		}

		modified.Unfix();
		pane.Puzzle = modified;
	}
}
