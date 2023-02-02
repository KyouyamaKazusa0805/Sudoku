#pragma warning disable CS0067

namespace SudokuStudio.Interaction.Commands;

/// <summary>
/// Defines a button command.
/// </summary>
public abstract class ButtonCommand : ICommand
{
	/// <inheritdoc/>
	public event EventHandler? CanExecuteChanged;


	/// <inheritdoc/>
	public virtual bool CanExecute(object? parameter) => true;

	/// <inheritdoc/>
	public abstract void Execute(object? parameter);
}
