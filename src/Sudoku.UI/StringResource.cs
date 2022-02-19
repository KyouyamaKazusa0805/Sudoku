namespace Sudoku.UI;

/// <summary>
/// Defines a <see langword="static class"/> that provides the method
/// to get the <see cref="string"/>-typed resources.
/// </summary>
internal static class StringResource
{
	/// <summary>
	/// Try to get the resource value via the specified string key.
	/// </summary>
	/// <param name="key">The <see cref="string"/>-typed resource key.</param>
	/// <returns>The resource value.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Get(string key) => (string)Application.Current.Resources[key];
}
