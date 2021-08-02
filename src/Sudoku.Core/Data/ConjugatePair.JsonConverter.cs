using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Sudoku.Data.Parsers;
using static Sudoku.Constants;

namespace Sudoku.Data
{
	partial struct ConjugatePair
	{
		/// <summary>
		/// Defines a JSON converter that allows the current instance being serialized.
		/// </summary>
		[JsonConverter(typeof(ConjugatePair))]
		public sealed class JsonConverter : JsonConverter<ConjugatePair>
		{
			/// <inheritdoc/>
			public override bool HandleNull => false;


			/// <inheritdoc/>
			/// <exception cref="InvalidOperationException">
			/// Throws when the specified value is invalid to parse.
			/// </exception>
			public override ConjugatePair Read(
				ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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

					if (
						Regex.Match(code, RegularExpressions.ConjugatePair) is not
						{
							Success: true,
							Groups: { Count: 4 } groups
						} match
					)
					{
						continue;
					}

					if (!CellParser.TryParse(groups[1].Value, out byte from)
						|| !CellParser.TryParse(groups[2].Value, out byte to)
						|| !byte.TryParse(groups[3].Value, out byte digit))
					{
						continue;
					}

					return new(from, to, digit);
				}

				throw new InvalidOperationException("The specified data is invalid.");
			}

			/// <inheritdoc/>
			public override void Write(Utf8JsonWriter writer, ConjugatePair value, JsonSerializerOptions options) =>
				writer.WriteStringValue(value.ToString());
		}
	}
}
