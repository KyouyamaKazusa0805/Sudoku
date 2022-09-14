namespace System.Windows.Input;

/// <summary>
/// Provides with extension methods on <see cref="ICommand"/>.
/// </summary>
/// <seealso cref="ICommand"/>
public static class CommandExtensions
{
	/// <summary>
	/// Execute the specified command if it can be executed.
	/// </summary>
	/// <param name="this">The command.</param>
	/// <param name="commandParameter">The command parameter.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool ExecuteIfCan(this ICommand @this, object? commandParameter)
	{
		if (@this.CanExecute(commandParameter))
		{
			@this.Execute(commandParameter);
			return true;
		}

		return false;
	}
}
