namespace Sudoku.UI.DataFactory;

/// <summary>
/// Provides with the read-only values.
/// </summary>
internal static class CommonReadOnlyFactory
{
	/// <summary>
	/// Creates a default option instance with:
	/// <list type="bullet">
	/// <item><see cref="JsonSerializerOptions.WriteIndented"/> is <see langword="true"/></item>
	/// <item><see cref="JsonSerializerOptions.IgnoreReadOnlyProperties"/> is <see langword="true"/></item>
	/// <item>
	/// <see cref="JsonSerializerOptions.PropertyNamingPolicy"/> is <see cref="JsonNamingPolicy.CamelCase"/>
	/// </item>
	/// <item>
	/// <see cref="JsonSerializerOptions.Encoder"/> is <see cref="JavaScriptEncoder.UnsafeRelaxedJsonEscaping"/>
	/// </item>
	/// </list>
	/// </summary>
	public static JsonSerializerOptions DefaultSerializerOption
		=> new()
		{
			WriteIndented = true,
			IgnoreReadOnlyProperties = true,
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
		};
}
