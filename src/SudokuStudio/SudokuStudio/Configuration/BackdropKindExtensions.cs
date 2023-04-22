namespace SudokuStudio.Configuration;

/// <summary>
/// Provides with extension methods on <see cref="BackdropKind"/>.
/// </summary>
/// <seealso cref="BackdropKind"/>
public static class BackdropKindExtensions
{
	/// <summary>
	/// Try to get target <see cref="SystemBackdrop"/> instance.
	/// </summary>
	/// <param name="this">The <see cref="BackdropKind"/> instance.</param>
	/// <returns>The target <see cref="SystemBackdrop"/> instance.</returns>
	/// <exception cref="NotSupportedException">Throws when the value is out of range.</exception>
	public static SystemBackdrop? GetBackdrop(this BackdropKind @this)
		=> @this switch
		{
			BackdropKind.Default => null,
			BackdropKind.Mica => new MicaBackdrop(),
			BackdropKind.MicaDeep => new MicaBackdrop { Kind = MicaKind.BaseAlt },
			BackdropKind.Acrylic => new DesktopAcrylicBackdrop(),
			_ => throw new NotSupportedException()
		};
}
