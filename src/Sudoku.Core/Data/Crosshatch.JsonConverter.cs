#if false

using System;
using System.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sudoku.Data
{
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
				ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			{
				Cells start, end;
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
						case JsonTokenType.String:
						{
							string cellsJson = reader.GetString()!;
							switch (pos)
							{
								case nameof(Start):
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

				return (*&start, *&end);
			}

			/// <inheritdoc/>
			public override void Write(Utf8JsonWriter writer, Crosshatch value, JsonSerializerOptions options)
			{
				var converter = options.GetConverter<Cells, Cells.JsonConverter>();

				writer.WriteStartObject();
				writer.WriteObject(value.Start, converter, options);
				writer.WriteObject(value.End, converter, options);
				writer.WriteEndObject();
			}
		}
	}
}

#endif