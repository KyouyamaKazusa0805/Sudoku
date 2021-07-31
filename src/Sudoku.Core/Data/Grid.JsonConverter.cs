using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sudoku.Data
{
	partial struct Grid
	{
		/// <summary>
		/// Indicates a <see cref="Grid"/> JSON converter.
		/// </summary>
		/// <seealso cref="Grid"/>
		[JsonConverter(typeof(Grid))]
		public sealed class JsonConverter : JsonConverter<Grid>
		{
			/// <inheritdoc/>
			public override bool HandleNull => false;


			/// <inheritdoc/>
			public override Grid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			{
				while (reader.Read())
				{
					if (reader.TokenType != JsonTokenType.String)
					{
						continue;
					}

					return reader.GetString() is not string code || !TryParse(code, out var grid) ? Undefined : grid;
				}

				return Undefined;
			}

			/// <inheritdoc/>
			public override void Write(Utf8JsonWriter writer, Grid value, JsonSerializerOptions options) =>
				writer.WriteStringValue(value.ToString("#"));
		}
	}
}
