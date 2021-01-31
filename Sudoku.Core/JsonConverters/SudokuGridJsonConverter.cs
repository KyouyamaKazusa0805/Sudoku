using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Sudoku.Data;

namespace Sudoku.JsonConverters
{
	[JsonConverter(typeof(SudokuGrid))]
	public sealed class SudokuGridJsonConverter : JsonConverter<SudokuGrid>
	{
		/// <inheritdoc/>
		public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(SudokuGrid);


		/// <inheritdoc/>
		public override SudokuGrid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			while (reader.Read())
			{
				switch (reader.TokenType)
				{
					case JsonTokenType.String:
					{
						if (reader.GetString() is not string code)
						{
							return SudokuGrid.Undefined;
						}
						if (!SudokuGrid.TryParse(code, out var grid))
						{
							return SudokuGrid.Undefined;
						}

						return grid;
					}
				}
			}

			return SudokuGrid.Undefined;
		}

		/// <inheritdoc/>
		public override void Write(Utf8JsonWriter writer, SudokuGrid value, JsonSerializerOptions options) =>
			writer.WriteStringValue(value.ToString("#"));
	}
}
