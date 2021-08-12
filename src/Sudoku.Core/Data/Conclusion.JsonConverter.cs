using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Sudoku.Data.Parsers;
using static Sudoku.Constants;

namespace Sudoku.Data
{
	partial struct Conclusion
	{
		/// <summary>
		/// Defines a JSON converter that allows the current instance being serialized.
		/// </summary>
		[JsonConverter(typeof(Conclusion))]
		public sealed class JsonConverter : JsonConverter<Conclusion>
		{
			/// <inheritdoc/>
			public override bool HandleNull => false;


			/// <inheritdoc/>
			/// <exception cref="InvalidOperationException">Throws when the specified data is invalid.</exception>
			public override Conclusion Read(
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
						Regex.Match(code, RegularExpressions.Conclusion) is not
						{
							Success: true,
							Groups: { Count: 4 } groups
						} match
					)
					{
						continue;
					}

					if (!CellParser.TryParse(groups[1].Value, out byte cell)
						|| !byte.TryParse(groups[3].Value, out byte digit))
					{
						continue;
					}

					var conclusionType = groups[2].Value switch
					{
						"=" => ConclusionType.Assignment,
						"!=" or "<>" => ConclusionType.Elimination
					};

					return new(conclusionType, cell, digit);
				}

				throw new InvalidOperationException("The specified data is invalid.");
			}

			/// <inheritdoc/>
			public override void Write(Utf8JsonWriter writer, Conclusion value, JsonSerializerOptions options) =>
				writer.WriteStringValue(value.ToString());
		}
	}
}
