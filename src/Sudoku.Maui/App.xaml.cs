namespace Sudoku.Maui;

/// <summary>
/// Defines an application.
/// </summary>
public sealed partial class App : Application
{
	/// <summary>
	/// Initializes an <see cref="App"/> instance.
	/// </summary>
	public App()
	{
		InitializeComponent();

		MainPage = new AppShell();

		R.AddExternalResourceFetecher(typeof(App).Assembly, ResourceTextFetcherNullable);
	}


	/// <summary>
	/// The internal application resource text fetcher.
	/// </summary>
	/// <param name="key">The key.</param>
	/// <returns>The target value fetched.</returns>
	internal static string ResourceTextFetcherCommon(string key)
		=> Current switch
		{
			{ Resources: var resources } when resources.TryGetValue(key, out var raw) && raw is string result => result,
			_ => string.Empty
		};

	/// <summary>
	/// The internal application resource text fetcher, with <see langword="null"/> returned if not having been found.
	/// </summary>
	/// <param name="key">The key.</param>
	/// <returns>The target value fetched.</returns>
	internal static string? ResourceTextFetcherNullable(string key)
		=> Current switch
		{
			{ Resources: var resources } when resources.TryGetValue(key, out var raw) && raw is string result => result,
			_ => null
		};
}
