using System.Runtime.CompilerServices;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Sudoku.Data;

namespace Sudoku.Drawing.Converters
{
	/// <summary>
	/// Indicates a <see cref="ValueTuple{T1, T2}"/> JSON converter.
	/// </summary>
	/// <seealso cref="ValueTuple{T1, T2}"/>
	public sealed class DirectLineJsonConverter : JsonConverter<(Cells Start, Cells End)>
	{
		/// <summary>
		/// Indicates the property name.
		/// </summary>
		private const string Start = nameof(Start), End = nameof(End);


		/// <summary>
		/// Indicates the inner options.
		/// </summary>
		private static readonly JsonSerializerOptions InnerOptions;


		static DirectLineJsonConverter()
		{
			InnerOptions = new() { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
			InnerOptions.Converters.Add(new CellsJsonConverter());
		}


		/// <inheritdoc/>
		public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof((Cells Start, Cells End));


		/// <inheritdoc/>
		[SkipLocalsInit]
		public override unsafe (Cells Start, Cells End) Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			Cells start, end;
			byte pos;
			while (reader.Read())
			{
				switch (reader.TokenType)
				{
					case JsonTokenType.PropertyName:
					{
						pos = (byte)(reader.GetString() == Start ? 0 : 1);
						break;
					}
					case JsonTokenType.String:
					{
						string cellsJson = reader.GetString()!;
						if (*&pos == 0)
						{
							start = JsonSerializer.Deserialize<Cells>(cellsJson, InnerOptions);
						}
						else
						{
							end = JsonSerializer.Deserialize<Cells>(cellsJson, InnerOptions);
						}

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
			writer.WriteString(Start, JsonSerializer.Serialize(value.Start, InnerOptions));
			writer.WriteString(End, JsonSerializer.Serialize(value.End, InnerOptions));
			writer.WriteEndObject();
		}
	}
}
