using System;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Sudoku.Models;

namespace Sudoku.JsonConverters
{
	/// <summary>
	/// Indicates a <see cref="DisplayingColor"/> JSON converter.
	/// </summary>
	/// <seealso cref="DisplayingColor"/>
	[JsonConverter(typeof(DisplayingColor))]
	public sealed class DisplayingColorJsonConverter : JsonConverter<DisplayingColor>
	{
		/// <inheritdoc/>
		public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(DisplayingColor);


		/// <inheritdoc/>
		/// <exception cref="JsonException">
		/// Throws when the current token isn't <c>"A"</c>, <c>"R"</c>, <c>"G"</c> or <c>"B"</c>.
		/// </exception>
		[SkipLocalsInit]
		public override unsafe DisplayingColor Read(
			ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			const int length = 4;
			byte* span = stackalloc byte[length];
			for (int i = 0; reader.Read() && i < length << 1; i++)
			{
				byte* ptr;
				if ((i & 1) == 0)
				{
					ptr = null;
				}

				switch (reader.TokenType)
				{
					case JsonTokenType.PropertyName:
					case JsonTokenType.String:
					{
						string? token = reader.GetString();
						ptr = token switch
						{
							nameof(DisplayingColor.A) or "a" => span,
							nameof(DisplayingColor.R) or "r" => span + 1,
							nameof(DisplayingColor.G) or "g" => span + 2,
							nameof(DisplayingColor.B) or "b" => span + 3,
							var str => throw new JsonException($"Can't check the current token '{str}' as a correct token.")
						};

						break;
					}
					case JsonTokenType.Number when *&ptr != null:
					{
						*ptr = reader.GetByte();

						break;
					}
				}
			}

			return DisplayingColor.FromArgb(span[0], span[1], span[2], span[3]);
		}

		/// <inheritdoc/>
		public override void Write(Utf8JsonWriter writer, DisplayingColor value, JsonSerializerOptions options)
		{
			var (a, r, g, b) = value;
			writer.WriteStartObject();
			writer.WriteNumber(nameof(DisplayingColor.A), a);
			writer.WriteNumber(nameof(DisplayingColor.R), r);
			writer.WriteNumber(nameof(DisplayingColor.G), g);
			writer.WriteNumber(nameof(DisplayingColor.B), b);
			writer.WriteEndObject();
		}
	}
}
