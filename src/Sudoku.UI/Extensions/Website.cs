namespace Sudoku.UI;

/// <summary>
/// Provides methods on website visiting.
/// </summary>
public static class Website
{
	/// <summary>
	/// Visit the specified website.
	/// </summary>
	/// <param name="uri">The URI website.</param>
	/// <returns>The process.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Process? Visit(string uri) =>
		Process.Start(new ProcessStartInfo(uri) { UseShellExecute = true });
}
