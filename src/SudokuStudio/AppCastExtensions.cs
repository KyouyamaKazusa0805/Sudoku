namespace SudokuStudio;

/// <summary>
/// Provides with extension methods on <see cref="App"/> and <see cref="Application"/>.
/// </summary>
/// <seealso cref="App"/>
/// <seealso cref="Application"/>
public static class AppCastExtensions
{
	/// <summary>
	/// Converts the current instance into an <see cref="App"/> instance;
	/// throw <see cref="InvalidCastException"/> if the current object is not an <see cref="App"/> instance.
	/// </summary>
	/// <param name="this">The current instance.</param>
	/// <returns>The result casted.</returns>
	public static App AsApp(this Application @this) => (App)@this;
}
