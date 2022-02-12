using Sudoku.UI.Models;

namespace Sudoku.UI;

/// <summary>
/// Provides a set of methods to convert the information.
/// </summary>
internal static class BindingConversions
{
	/// <summary>
	/// Indicates the license displaying value on <see cref="RepositoryInfo.OpenSourceLicense"/>.
	/// </summary>
	/// <param name="input">The license name.</param>
	/// <returns>The converted result string.</returns>
	/// <seealso cref="RepositoryInfo.OpenSourceLicense"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string License(string input) =>
		$"{input} {Application.Current.Resources["AboutPage_License"]}";

	/// <summary>
	/// Indicates the conversion on <see cref="RepositoryInfo.IsForReference"/>.
	/// </summary>
	/// <param name="input">The input value.</param>
	/// <returns>The converted result string value.</returns>
	/// <seealso cref="RepositoryInfo.IsForReference"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ForReference(bool input) =>
		input ? (string)Application.Current.Resources["AboutPage_ForReference"] : string.Empty;
}
