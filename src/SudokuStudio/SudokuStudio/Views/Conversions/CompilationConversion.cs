namespace SudokuStudio.Views.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about compilation information.
/// </summary>
internal static class CompilationConversion
{
	public static string GetCompilationInfo()
		=> $"{GetString("AboutPage_Version")} {((App)Application.Current).RunningContext.AssemblyVersion.ToString(2)} | x64 | Release";

	public static string License(string input) => $"{input} {GetString("AboutPage_License")}";

	public static string ForReference(bool input) => input ? GetString("AboutPage_ForReference") : string.Empty;
}
