namespace System.Text.Json;

/// <summary>
/// Provides a list of <see cref="JsonSerializerOptions"/> instances that is used on default cases.
/// </summary>
public static class CommonSerializerOptions
{
	/// <summary>
	/// Creates a default option instance with:
	/// <list type="bullet">
	/// <item><see cref="JsonSerializerOptions.WriteIndented"/> is <see langword="true"/></item>
	/// <item><see cref="JsonSerializerOptions.IgnoreReadOnlyProperties"/> is <see langword="true"/></item>
	/// <item>
	/// <see cref="JsonSerializerOptions.Encoder"/> is <see cref="JavaScriptEncoder.UnsafeRelaxedJsonEscaping"/>
	/// </item>
	/// <item><see cref="JsonSerializerOptions.ReadCommentHandling"/> is <see cref="JsonCommentHandling.Skip"/></item>
	/// <item><see cref="JsonSerializerOptions.UnmappedMemberHandling"/> is <see cref="JsonUnmappedMemberHandling.Disallow"/></item>
	/// </list>
	/// </summary>
	public static readonly JsonSerializerOptions PascalCasing =
		new()
		{
			WriteIndented = true,
			IgnoreReadOnlyProperties = true,
			Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
			ReadCommentHandling = JsonCommentHandling.Skip,
			UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow
		};
}
