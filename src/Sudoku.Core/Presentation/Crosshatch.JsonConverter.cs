namespace Sudoku.Presentation;

partial record struct Crosshatch
{
	/// <summary>
	/// Indicates the JSON converter.
	/// </summary>
	[JsonConverter(typeof(Crosshatch))]
	public sealed class JsonConverter : JsonConverter<Crosshatch>
	{
		/// <inheritdoc/>
		public override bool HandleNull => false;


		/// <inheritdoc/>
		public override unsafe Crosshatch Read(
			ref Utf8JsonReader reader,
			Type typeToConvert,
			JsonSerializerOptions options
		)
		{
			Unsafe.SkipInit(out Cells start);
			Unsafe.SkipInit(out Cells end);
			string? pos = null;
			while (reader.Read())
			{
				switch (reader.TokenType)
				{
					case JsonTokenType.PropertyName:
					{
						pos = reader.GetString();
						break;
					}
					case JsonTokenType.String when reader.GetString() is { } cellsJson:
					{
						switch (pos)
						{
							case nameof(start):
							{
								start = JsonSerializer.Deserialize<Cells>(cellsJson, options);
								break;
							}
							case nameof(end):
							{
								end = JsonSerializer.Deserialize<Cells>(cellsJson, options);
								break;
							}
						}

						break;
					}
				}
			}

			return new(start, end);
		}

		/// <inheritdoc/>
		public override void Write(Utf8JsonWriter writer, Crosshatch value, JsonSerializerOptions options)
		{
			var converter = options.GetConverter(typeof(Cells)) as JsonConverter<Cells> ?? new Cells.JsonConverter();

			writer.WriteStartObject();
			writer.WriteObject(value.Start, converter, options);
			writer.WriteObject(value.End, converter, options);
			writer.WriteEndObject();
		}
	}
}
