namespace Microsoft.UI.Xaml;

/// <summary>
/// Provides extension methods on <see cref="Application"/> about <see cref="Application.RequestedTheme"/>.
/// </summary>
/// <seealso cref="Application"/>
/// <seealso cref="Application.RequestedTheme"/>
/// <remarks><i>
/// For more information you can visit
/// <see href="https://docs.microsoft.com/en-us/windows/winui/api/microsoft.ui.xaml.application.requestedtheme">
/// this page
/// </see>
/// learn about details.
/// </i></remarks>
internal static class ApplicationRequestedThemes
{
	/// <summary>
	/// Gets the requested theme color that applies as the foreground color of a control.
	/// </summary>
	/// <returns>
	/// A <see cref="Color"/> instance:
	/// <list type="table">
	/// <listheader>
	/// <term><see cref="Application.RequestedTheme"/> value</term>
	/// <description>The result <see cref="Color"/></description>
	/// </listheader>
	/// <item>
	/// <term><see cref="ApplicationTheme.Light"/></term>
	/// <description><see cref="Colors.Black"/></description>
	/// </item>
	/// <item>
	/// <term><see cref="ApplicationTheme.Dark"/></term>
	/// <description><see cref="Colors.White"/></description>
	/// </item>
	/// </list>
	/// </returns>
	/// <seealso cref="Colors.Black"/>
	/// <seealso cref="Colors.White"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Color GetForegroundColor() =>
		Application.Current.RequestedTheme == ApplicationTheme.Light ? Colors.Black : Colors.White;

	/// <summary>
	/// Gets the requested theme color that applies as the background color of a control.
	/// </summary>
	/// <returns>
	/// A <see cref="Color"/> instance:
	/// <list type="table">
	/// <listheader>
	/// <term><see cref="Application.RequestedTheme"/> value</term>
	/// <description>The result <see cref="Color"/></description>
	/// </listheader>
	/// <item>
	/// <term><see cref="ApplicationTheme.Light"/></term>
	/// <description><see cref="Colors.White"/></description>
	/// </item>
	/// <item>
	/// <term><see cref="ApplicationTheme.Dark"/></term>
	/// <description><see cref="Colors.Black"/></description>
	/// </item>
	/// </list>
	/// </returns>
	/// <seealso cref="Colors.Black"/>
	/// <seealso cref="Colors.White"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Color GetBackgroundColor() =>
		Application.Current.RequestedTheme == ApplicationTheme.Light ? Colors.White : Colors.Black;
}
