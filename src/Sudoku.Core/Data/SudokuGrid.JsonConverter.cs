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
							return reader.GetString() is not string code || !TryParse(code, out var grid)
								? Undefined
								: grid;
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
