namespace Sudoku.Serialization;

/// <summary>
/// Defines a JSON converter that parses the string to a link type or vice versa.
/// </summary>
[JsonConverter(typeof(LinkType))]
public sealed class LinkTypeJsonConverter : JsonConverter<LinkType>
{
	/// <inheritdoc/>
	public override LinkType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (typeToConvert != typeof(LinkType))
		{
			return LinkTypes.Default;
		}

		if (reader.Read())
		{
			if (reader.TokenType == JsonTokenType.Number)
			{
				return new(Math.Clamp(reader.GetByte(), (byte)LinkTypes.Default, (byte)LinkTypes.Strong));
			}
		}

		return LinkTypes.Default;
	}

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, LinkType value, JsonSerializerOptions options) =>
		writer.WriteNumberValue(value.TypeKind);
}

/// <summary>
/// Provides a serializer context on type <see cref="Data.Link"/>.
/// </summary>
/// <seealso cref="Data.Link"/>
[JsonSerializable(typeof(Link), GenerationMode = JsonSourceGenerationMode.Metadata)]
public sealed partial class LinkJsonSerializerContext : JsonSerializerContext
{
}
