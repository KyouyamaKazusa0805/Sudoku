using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sudoku.Data
{
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
			public override Cells Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			{
				long hi, lo;
				while (reader.Read())
				{
					byte pos;
					switch (reader.TokenType)
					{
						case JsonTokenType.PropertyName:
						{
							pos = reader.GetString() switch { "HighBits" => 0, "LowBits" => 1 };
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
				writer.WriteNumber("HighBits", hi);
				writer.WriteNumber("LowBits", lo);
				writer.WriteEndObject();
			}
		}
	}
}
