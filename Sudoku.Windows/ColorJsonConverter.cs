#if JSON_SERIALIZER

using System;
using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;
using Sudoku.Drawing.Extensions;
using Sudoku.Extensions;

namespace Sudoku.Windows
{
	/// <summary>
	/// Indicates a color JSON converter.
	/// </summary>
	[JsonConverter(typeof(Color))]
	public sealed class ColorJsonConverter : JsonConverter<Color>
	{
		/// <inheritdoc/>
		public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(Color);


		/// <inheritdoc/>
		public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var colorValueSpan = (stackalloc byte[4]);
			for (int index = -1; reader.Read();)
			{
				switch (reader.TokenType)
				{
					case JsonTokenType.PropertyName or JsonTokenType.String when reader.GetString() is var s:
					{
						if (s is null or not ("A" or "R" or "G" or "B"))
						{
							break;
						}

						index++;
						break;
					}
					case JsonTokenType.Number:
					{
						colorValueSpan[index] = reader.GetByte();

						break;
					}
				}
			}

			return Color.FromArgb(colorValueSpan[0], colorValueSpan[1], colorValueSpan[2], colorValueSpan[3]);
		}

		/// <inheritdoc/>
		public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
		{
			var (a, r, g, b) = value;
			writer.WriteStartObject();
			writer.WriteNumber("A", a);
			writer.WriteNumber("R", r);
			writer.WriteNumber("G", g);
			writer.WriteNumber("B", b);
			writer.WriteEndObject();
		}
	}
}

#endif