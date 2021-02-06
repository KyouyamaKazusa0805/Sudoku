using System;
using System.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization;
using Sudoku.Data;

namespace Sudoku.JsonConverters
{
	/// <summary>
	/// Indicates a <see cref="ValueTuple{T1, T2}"/> JSON converter.
	/// </summary>
	/// <seealso cref="ValueTuple{T1, T2}"/>
	[JsonConverter(typeof((Cells Start, Cells End)))]
	public sealed class DirectLineJsonConverter : JsonConverter<(Cells Start, Cells End)>
	{
		/// <summary>
		/// Indicates the property name.
		/// </summary>
		private const string Start = nameof(Start), End = nameof(End);


		/// <summary>
		/// Indicates the inner converter.
		/// </summary>
		private static readonly JsonConverter<Cells> Converter = new CellsJsonConverter();


		/// <inheritdoc/>
		public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof((Cells Start, Cells End));


		/// <inheritdoc/>
		public override unsafe (Cells Start, Cells End) Read(
			ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			Cells start, end;
			byte pos;
			while (reader.Read())
			{
				switch (reader.TokenType)
				{
					case JsonTokenType.PropertyName:
					{
						pos = reader.GetString() == Start ? 0 : 1;
						break;
					}
					case JsonTokenType.StartObject:
					{
						*(*&pos == 0 ? &start : &end) = reader.ReadObject(Converter, typeof(Cells), options);
						break;
					}
				}
			}

			return (*&start, *&end);
		}

		/// <inheritdoc/>
		public override void Write(Utf8JsonWriter writer, (Cells Start, Cells End) value, JsonSerializerOptions options)
		{
			writer.WriteStartObject();
			writer.WritePropertyName(Start);
			writer.WriteObject(value.Start, Converter, options);
			writer.WritePropertyName(End);
			writer.WriteObject(value.End, Converter, options);
			writer.WriteEndObject();
		}
	}
}
