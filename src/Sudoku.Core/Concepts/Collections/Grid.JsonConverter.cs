#undef GRID_SERIALIZE_STRINGS
#define GRID_SERIALIZE_RAW_DATA

namespace Sudoku.Concepts.Collections;

partial struct Grid
{
	/// <summary>
	/// Defines a JSON converter.
	/// </summary>
	public sealed class JsonConverter : JsonConverter<Grid>
	{
		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Grid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
#if GRID_SERIALIZE_STRINGS
			if (!reader.Read() || reader.TokenType != JsonTokenType.String)
			{
				throw new JsonException();
			}

			string? value = reader.GetString();
			return value is null ? Undefined : Parse(value);
#elif GRID_SERIALIZE_RAW_DATA
			if (!reader.Read() || reader.TokenType != JsonTokenType.StartArray)
			{
				throw new JsonException();
			}

			short[] targetMaskList = new short[81];
			int i = 0;
			while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
			{
				short mask = reader.GetInt16();
				targetMaskList[i++] = mask;
			}

			if (!reader.Read() || reader.TokenType != JsonTokenType.EndArray)
			{
				throw new JsonException();
			}

			return new(targetMaskList);
#else
#error You must set the symbol either 'GRID_SERIALIZE_STRINGS' or 'GRID_SERIALIZE_RAW_DATA'.
#endif
		}

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void Write(Utf8JsonWriter writer, Grid value, JsonSerializerOptions options)
		{
#if GRID_SERIALIZE_STRINGS
			writer.WriteStringValue($"{value:#}");
#elif GRID_SERIALIZE_RAW_DATA
			writer.WriteStartArray();
			foreach (short mask in value.EnumerateMasks())
			{
				writer.WriteNumberValue(mask);
			}
			writer.WriteEndArray();
#else
#error You must set the symbol either 'GRID_SERIALIZE_STRINGS' or 'GRID_SERIALIZE_RAW_DATA'.
#endif
		}
	}
}
