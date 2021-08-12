namespace Sudoku.Drawing.Converters;

/// <summary>
/// Indicates a <see cref="Cells"/> JSON converter.
/// </summary>
/// <seealso cref="Cells"/>
public sealed class CellsJsonConverter : JsonConverter<Cells>
{
	/// <summary>
	/// The property name.
	/// </summary>
	private const string HighBits = nameof(HighBits), LowBits = nameof(LowBits);


	/// <inheritdoc/>
	public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(Cells);

	/// <inheritdoc/>
	public override unsafe Cells Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		long hi, lo;
		while (reader.Read())
		{
			byte pos;
			switch (reader.TokenType)
			{
				case JsonTokenType.PropertyName:
				{
					pos = (byte)(reader.GetString() == HighBits ? 0 : 1);
					break;
				}
				case JsonTokenType.Number:
				{
					*(*&pos == 0 ? &hi : &lo) = reader.GetInt64();

					break;
				}
			}
		}

		return new(*&hi, *&lo);
	}

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, Cells value, JsonSerializerOptions options)
	{
		var (hi, lo) = value;
		writer.WriteStartObject();
		writer.WriteNumber(HighBits, hi);
		writer.WriteNumber(LowBits, lo);
		writer.WriteEndObject();
	}
}
