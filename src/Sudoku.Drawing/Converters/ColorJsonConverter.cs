using Sudoku.Drawing.Extensions;

namespace Sudoku.Drawing.Converters
{
	/// <summary>
	/// Indicates a <see cref="Color"/> JSON converter.
	/// </summary>
	/// <seealso cref="Color"/>
	[JsonConverter(typeof(Color))]
	public sealed class ColorJsonConverter : JsonConverter<Color>
	{
		/// <inheritdoc/>
		public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(Color);


		/// <inheritdoc/>
		/// <exception cref="JsonException">
		/// Throws when the current token isn't <c>"A"</c>, <c>"R"</c>, <c>"G"</c> or <c>"B"</c>.
		/// </exception>
		[SkipLocalsInit]
		public override unsafe Color Read(
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
							nameof(Color.A) or "a" => span,
							nameof(Color.R) or "r" => span + 1,
							nameof(Color.G) or "g" => span + 2,
							nameof(Color.B) or "b" => span + 3,
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

			return Color.FromArgb(span[0], span[1], span[2], span[3]);
		}

		/// <inheritdoc/>
		public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
		{
			var (a, r, g, b) = value;
			writer.WriteStartObject();
			writer.WriteNumber(nameof(Color.A), a);
			writer.WriteNumber(nameof(Color.R), r);
			writer.WriteNumber(nameof(Color.G), g);
			writer.WriteNumber(nameof(Color.B), b);
			writer.WriteEndObject();
		}
	}
}
