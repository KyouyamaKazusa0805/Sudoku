namespace Sudoku.UI.Windows.Commands;

/// <summary>
/// Defines a base type that handles a <see cref="ICommand"/>.
/// </summary>
/// <seealso cref="ICommand"/>
public abstract class Command : ICommand
{
	/// <summary>
	/// The inner field to check the status.
	/// </summary>
	private readonly Predicate<object?>? _canExecute;

	/// <summary>
	/// The inner field to handle the operation.
	/// </summary>
	private readonly Action<object?>? _action;


	/// <summary>
	/// Initializes an instance via two methods specified as function pointers.
	/// The first one is aiming to check whether the later operation can be executed
	/// and the second one is aiming to execute the operation.
	/// </summary>
	/// <param name="canExecute">
	/// The predicate to check whether the later operation can be executed at the current status.
	/// </param>
	/// <param name="action">The operation to be handled.</param>
	public unsafe Command(delegate*<object?, bool> canExecute, delegate*<object?, void> action)
	{
		_canExecute = f;
		_action = g;


		bool f(object? parameter) => canExecute(parameter);
		void g(object? parameter) => action(parameter);
	}

	/// <summary>
	/// Initializes an instance via two methods.
	/// The first one is aiming to check whether the later operation can be executed
	/// and the second one is aiming to execute the operation.
	/// </summary>
	/// <param name="canExecute">
	/// The predicate to check whether the later operation can be executed at the current status.
	/// </param>
	/// <param name="action">The operation to be handled.</param>
	/// <exception cref="ArgumentException">
	/// Throws when the argument <paramref name="canExecute"/> or <paramref name="action"/>
	/// holds more than 1 method to be invoked.
	/// </exception>
	public Command(Predicate<object?>? canExecute, Action<object?>? action)
	{
		f(canExecute);
		f(action);

		_canExecute = canExecute;
		_action = action;


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void f<T>(T? t, [CallerArgumentExpression("t")] string? p = null) where T : Delegate
		{
			if (t is not null && t.GetInvocationList().Length != 1)
			{
				throw new ArgumentException("The delegate cannot hold more than 1 invocation method.", p);
			}
		}
	}


	/// <inheritdoc/>
	public event EventHandler? CanExecuteChanged;


	/// <summary>
	/// Raise the event <see cref="CanExecuteChanged"/>.
	/// </summary>
	/// <seealso cref="CanExecuteChanged"/>
	public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

	/// <inheritdoc/>
	public void Execute(object? parameter) => _action?.Invoke(parameter);

	/// <inheritdoc/>
	public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;
}
