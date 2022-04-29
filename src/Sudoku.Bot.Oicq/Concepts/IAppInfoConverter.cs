namespace Sudoku.Bot.Oicq.Concepts;

/// <summary>
/// Defines an <see cref="AppInfo"/> converter instance.
/// </summary>
public interface IAppInfoConverter
{
	/// <summary>
	/// Converts for an <see cref="AppInfo"/> instance into a <see cref="string"/> representation.
	/// </summary>
	/// <param name="info">The <see cref="AppInfo"/> instance.</param>
	/// <returns>The converted string value.</returns>
	public string Convert(AppInfo info);
}
