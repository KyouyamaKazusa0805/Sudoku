namespace Nano.Commands;

/// <summary>
/// Provides with the type that handles a command.
/// </summary>
public sealed class DelegateCommand : ICommand
{
	/// <summary>
	/// Indicates the property holding a method that checks whether the current state
	/// can execute the command or not.
	/// </summary>
	public Predicate<object?>? CanExecuteCommand { get; set; }

	/// <summary>
	/// Indicates the property holding a method that handles the command.
	/// </summary>
	public Action<object?>? ExecuteCommand { get; set; }


	/// <inheritdoc/>
	public event EventHandler? CanExecuteChanged;


	/// <inheritdoc/>
	public void Execute(object? parameter) => ExecuteCommand?.Invoke(parameter);

	/// <inheritdoc/>
	public bool CanExecute(object? parameter) => CanExecuteCommand?.Invoke(parameter) ?? true;
}
