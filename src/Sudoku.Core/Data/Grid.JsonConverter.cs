namespace Sudoku.Data;

partial struct Grid
{
	/// <summary>
	/// Defines a JSON converter that allows the current instance being serialized.
	/// </summary>
	/// <seealso cref="Grid"/>
	[JsonConverter(typeof(Grid))]
	public sealed class JsonConverter : JsonConverter<Grid>
	{
		/// <inheritdoc/>
		public override bool HandleNull => false;


		/// <inheritdoc/>
		/// <exception cref="InvalidOperationException">Throws when the specified data is invalid.</exception>
		public override Grid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			while (reader.Read())
			{
				if (reader.TokenType == JsonTokenType.String
					&& reader.GetString() is { } code && TryParse(code, out var grid))
				{
					return grid;
				}
			}

			throw new InvalidOperationException("The specified data is invalid.");
		}

		/// <inheritdoc/>
		public override void Write(Utf8JsonWriter writer, Grid value, JsonSerializerOptions options) =>
			writer.WriteStringValue(value.ToString("#"));
	}
}
