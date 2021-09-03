namespace Sudoku.UI.Interactions;

/// <summary>
/// Provides with the data context that binds with <see cref="SettingsPage"/>.
/// </summary>
/// <seealso cref="SettingsPage"/>
public sealed class SettingsPageDataContext : IDataContext<SettingsPageDataContext>
{
	/// <inheritdoc/>
	public static SettingsPageDataContext CreateInstance() => new();
}
