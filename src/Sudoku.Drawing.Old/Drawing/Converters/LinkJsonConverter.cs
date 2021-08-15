namespace Sudoku.Drawing.Converters.Old;

/// <summary>
/// Indicates a <see cref="Link"/> JSON converter.
/// </summary>
/// <seealso cref="Link"/>
public sealed class LinkJsonConverter : JsonConverter<Link>
{
	/// <inheritdoc/>
	public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(Link);


	/// <inheritdoc/>
	[SkipLocalsInit]
	public override unsafe Link Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		int start, end;
		LinkType linkType;
		byte pos;
		while (reader.Read())
		{
			switch (reader.TokenType)
			{
				case JsonTokenType.PropertyName:
				{
					pos = reader.GetString() switch
					{
						nameof(Link.StartCandidate) => 0,
						nameof(Link.EndCandidate) => 1,
						nameof(Link.LinkType) => 2
					};
					break;
				}
				case JsonTokenType.Number:
				{
					switch (*&pos)
					{
						case 0:
						{
							start = reader.GetInt32();
							break;
						}
						case 1:
						{
							end = reader.GetInt32();
							break;
						}
						case 2:
						{
							linkType = (LinkType)reader.GetByte();
							break;
						}
					}

					break;
				}
			}
		}

		return new(*&start, *&end, *&linkType);
	}

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, Link value, JsonSerializerOptions options)
	{
		writer.WriteStartObject();
		writer.WriteNumber(nameof(Link.StartCandidate), value.StartCandidate);
		writer.WriteNumber(nameof(Link.EndCandidate), value.EndCandidate);
		writer.WriteNumber(nameof(Link.LinkType), (byte)value.LinkType);
		writer.WriteEndObject();
	}
}
