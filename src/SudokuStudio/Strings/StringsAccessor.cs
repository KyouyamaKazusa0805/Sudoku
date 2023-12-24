namespace SudokuStudio.Strings;

/// <summary>
/// Defines an easy entry to get <see cref="string"/> resources.
/// </summary>
internal static class StringsAccessor
{
	/// <inheritdoc cref="GetString(string)"/>
	[return: NotNullIfNotNull(nameof(key))]
	public static string? GetStringNullable(string? key) => key is null ? null : Resources.ResourceManager.GetString(key, App.CurrentCulture);

	/// <summary>
	/// Try to fetch the target resource via the specified key.
	/// </summary>
	/// <param name="key">The resource key.</param>
	/// <returns>The target resource.</returns>
	public static string GetString(string key)
		=> Resources.ResourceManager.GetString(key, App.CurrentCulture) ?? throw new KeyNotFoundException("The target resource is not found.");

	/// <summary>
	/// Try to fetch the specified token via its name.
	/// </summary>
	/// <param name="tokenName">The name of the token, such as <c>Colon</c>.</param>
	/// <returns>The token string.</returns>
	public static string Token(string tokenName) => GetString($"_Token_{tokenName}");
}
