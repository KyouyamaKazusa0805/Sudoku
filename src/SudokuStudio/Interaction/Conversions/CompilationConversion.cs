namespace SudokuStudio.Interaction.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about compilation information.
/// </summary>
internal static class CompilationConversion
{
	public static string GetCompilationInfo() => $"{ResourceDictionary.Get("AboutPage_Version", App.CurrentCulture)} {App.AssemblyVersion.ToString(3)} | x64 | Release";

	public static string License(string input) => $"{input} {ResourceDictionary.Get("AboutPage_License", App.CurrentCulture)}";

	public static string ForReference(bool input) => input ? ResourceDictionary.Get("AboutPage_ForReference", App.CurrentCulture) : string.Empty;
}
