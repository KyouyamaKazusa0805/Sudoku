using System.Text.Json;

namespace Sudoku.UI.Data;

partial record struct UIColor
{
	/// <summary>
	/// Defines a JSON converter that allows the object of type <see cref="UIColor"/>
	/// can be serialized and deserialized.
	/// </summary>
	[JsonConverter(typeof(UIColor))]
	public sealed class JsonConverter : JsonConverter<UIColor>
	{
		/// <inheritdoc/>
		/// <exception cref="InvalidOperationException">Throws when the specified data is invalid.</exception>
		public override UIColor Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			while (reader.Read())
			{
				if (reader.TokenType != JsonTokenType.String)
				{
					continue;
				}

				if (reader.GetString() is not { } code)
				{
					continue;
				}

				return Parse(code);
			}

			throw new InvalidOperationException("The specified data is invalid.");
		}

		/// <inheritdoc/>
		public override void Write(Utf8JsonWriter writer, UIColor value, JsonSerializerOptions options) =>
			writer.WriteStringValue(value.ToString("#"));
	}
}
