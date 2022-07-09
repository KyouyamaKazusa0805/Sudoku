namespace Sudoku.UI;

/// <summary>
/// Provides a set of properties that checks the current runtime,
/// to determine whether the current runtime supports the current operation.
/// </summary>
internal static class Supportability
{
	/// <inheritdoc cref="PrintManager.IsSupported()"/>
	public static bool Printer => PrintManager.IsSupported();

	/// <inheritdoc cref="AppWindowTitleBar.IsCustomizationSupported()"/>
	public static bool TitleBar => AppWindowTitleBar.IsCustomizationSupported();

	/// <inheritdoc cref="MicaController.IsSupported()"/>
	public static bool Mica => MicaController.IsSupported();
}
