namespace Sudoku.Data;

partial struct Cells
{
	/// <summary>
	/// Defines a JSON converter that allows the current instance being serialized.
	/// </summary>
	[JsonConverter(typeof(Cells))]
	public sealed unsafe class JsonConverter : JsonConverter<Cells>
	{
		/// <inheritdoc/>
		public override bool HandleNull => false;


		/// <inheritdoc/>
		/// <exception cref="InvalidOperationException">Throws when the specified data is invalid.</exception>
		public override Cells Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			Unsafe.SkipInit(out long hi);
			Unsafe.SkipInit(out long lo);
			while (reader.Read())
			{
				Unsafe.SkipInit(out byte pos);
				switch (reader.TokenType)
				{
					case JsonTokenType.PropertyName:
					{
						pos = reader.GetString() switch
						{
							"HighBits" => 0,
							"LowBits" => 1,
							_ => throw new InvalidOperationException("The specified data is invalid.")
						};
						break;
					}
					case JsonTokenType.Number:
					{
						*(pos == 0 ? &hi : &lo) = reader.GetInt64();
						break;
					}
				}
			}

			return new(hi, lo);
		}

		/// <inheritdoc/>
		public override void Write(Utf8JsonWriter writer, Cells value, JsonSerializerOptions options)
		{
			_ = value is { _low: var lo, _high: var hi };

			writer.WriteStartObject();
			writer.WriteNumber("HighBits", hi);
			writer.WriteNumber("LowBits", lo);
			writer.WriteEndObject();
		}
	}
}
