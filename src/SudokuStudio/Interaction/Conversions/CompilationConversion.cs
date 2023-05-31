namespace SudokuStudio.Interaction.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about compilation information.
/// </summary>
internal static class CompilationConversion
{
	public static string GetCompilationInfo() => $"{GetString("AboutPage_Version")} {App.AssemblyVersion.ToString(3)} | x64 | Release";

	public static string License(string input) => $"{input} {GetString("AboutPage_License")}";

	public static string ForReference(bool input) => input ? GetString("AboutPage_ForReference") : string.Empty;
}
