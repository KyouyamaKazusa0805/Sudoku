namespace Sudoku.Data;

partial record struct UnknownValue
{
	/// <summary>
	/// Indicates the JSON converter.
	/// </summary>
	[JsonConverter(typeof(UnknownValue))]
	public sealed class JsonConverter : JsonConverter<UnknownValue>
	{
		/// <inheritdoc/>
		public override bool HandleNull => false;


		/// <inheritdoc/>
		public override unsafe UnknownValue Read(
			ref Utf8JsonReader reader,
			Type typeToConvert,
			JsonSerializerOptions options
		)
		{
			Unsafe.SkipInit(out int cell);
			Unsafe.SkipInit(out char identifier);
			Unsafe.SkipInit(out short mask);
			string? pos = null;
			while (reader.Read())
			{
				switch (reader.TokenType)
				{
					case JsonTokenType.PropertyName:
					{
						pos = reader.GetString();
						break;
					}
					case JsonTokenType.String:
					{
						switch (pos)
						{
							case nameof(Cell):
							{
								cell = reader.GetInt32();
								break;
							}
							case nameof(UnknownIdentifier):
							{
								identifier = (char)reader.GetInt32();
								break;
							}
							case nameof(DigitsMask):
							{
								mask = reader.GetInt16();
								break;
							}
						}

						break;
					}
				}
			}

			return new(cell, identifier, mask);
		}

		/// <inheritdoc/>
		public override void Write(Utf8JsonWriter writer, UnknownValue value, JsonSerializerOptions options)
		{
			writer.WriteStartObject();
			writer.WriteNumber(nameof(Cell), value.Cell);
			writer.WriteNumber(nameof(UnknownIdentifier), value.UnknownIdentifier);
			writer.WriteNumber(nameof(DigitsMask), value.DigitsMask);
			writer.WriteEndObject();
		}
	}
}
