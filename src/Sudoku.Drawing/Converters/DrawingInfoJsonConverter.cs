namespace Sudoku.Drawing.Converters
{
	/// <summary>
	/// Indicates a <see cref="DrawingInfo"/> JSON converter.
	/// </summary>
	/// <seealso cref="DrawingInfo"/>
	public sealed class DrawingInfoJsonConverter : JsonConverter<DrawingInfo>
	{
		/// <inheritdoc/>
		public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(DrawingInfo);


		/// <inheritdoc/>
		[SkipLocalsInit]
		public override unsafe DrawingInfo Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			long id;
			int value;
			byte pos;
			while (reader.Read())
			{
				switch (reader.TokenType)
				{
					case JsonTokenType.PropertyName:
					{
						pos = (byte)(reader.GetString() == nameof(DrawingInfo.Id) ? 0 : 1);
						break;
					}
					case JsonTokenType.Number:
					{
						if (*&pos == 0)
						{
							id = reader.GetInt64();
						}
						else
						{
							value = reader.GetInt32();
						}

						break;
					}
				}
			}

			return new(*&id, *&value);
		}

		/// <inheritdoc/>
		public override void Write(Utf8JsonWriter writer, DrawingInfo value, JsonSerializerOptions options)
		{
			writer.WriteStartObject();
			writer.WriteNumber(nameof(DrawingInfo.Id), value.Id);
			writer.WriteNumber(nameof(DrawingInfo.Value), value.Value);
			writer.WriteEndObject();
		}
	}
}
