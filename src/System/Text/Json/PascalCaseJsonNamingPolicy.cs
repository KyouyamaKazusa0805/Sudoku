namespace System.Text.Json;

/// <summary>
/// Defines the pascal casing JSON naming policy.
/// </summary>
/// <remarks>
/// This type cannot be initialized; instead, you can use the property <see cref="PascalCase"/> to get the instance.
/// </remarks>
/// <seealso cref="PascalCase"/>
public sealed class PascalCaseJsonNamingPolicy : JsonNamingPolicy
{
	/// <summary>
	/// Gets the naming policy for pascal-casing.
	/// </summary>
	/// <returns>The naming policy for pascal-casing.</returns>
	public static JsonNamingPolicy PascalCase
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new PascalCaseJsonNamingPolicy();
	}


	/// <inheritdoc/>
	public override string ConvertName(string name)
		=> name switch
		{
			[] => string.Empty,
			[var firstChar and >= 'a' and <= 'z', .. var slice] => $"{(char)(firstChar - ' ')}{slice}",
			[>= 'A' and <= 'Z', ..] => name,
			[var firstChar, .. var slice] => $"{firstChar}{ConvertName(slice)}"
		};
}
