using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sudoku.Data
{
	partial struct SudokuGrid
	{
		/// <summary>
		/// Indicates a <see cref="SudokuGrid"/> JSON converter.
		/// </summary>
		/// <seealso cref="SudokuGrid"/>
		[JsonConverter(typeof(SudokuGrid))]
		public sealed class JsonConverter : JsonConverter<SudokuGrid>
		{
			/// <inheritdoc/>
			public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(SudokuGrid);


			/// <inheritdoc/>
			public override SudokuGrid Read(
				ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			{
				while (reader.Read())
				{
					switch (reader.TokenType)
					{
						case JsonTokenType.String:
						{
							if (reader.GetString() is not string code)
							{
								return Undefined;
							}
							if (!TryParse(code, out var grid))
							{
								return Undefined;
							}

							return grid;
						}
					}
				}

				return Undefined;
			}

			/// <inheritdoc/>
			public override void Write(Utf8JsonWriter writer, SudokuGrid value, JsonSerializerOptions options) =>
				writer.WriteStringValue(value.ToString("#"));
		}
	}
}
