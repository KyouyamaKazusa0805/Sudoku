namespace Sudoku.Drawing;

public partial struct DashArray
{
	/// <summary>
	/// Defines JSON serialization converter on type <see cref="DashArray"/>.
	/// </summary>
	/// <seealso cref="DashArray"/>
	private sealed class Converter : JsonConverter<DashArray>
	{
		/// <inheritdoc/>
		public override DashArray Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (typeToConvert != typeof(DashArray) || reader.TokenType != JsonTokenType.StartArray)
			{
				throw new JsonException();
			}

			var targetCollection = new List<double>();
			while (reader.Read())
			{
				switch (reader.TokenType)
				{
					case JsonTokenType.Number when options.NumberHandling == JsonNumberHandling.Strict:
					{
						targetCollection.Add(reader.GetDouble());
						break;
					}
					case JsonTokenType.String when options.NumberHandling is var o && o.HasFlag(JsonNumberHandling.AllowReadingFromString):
					{
						if (reader.TryGetDouble(out var value))
						{
							targetCollection.Add(value);
							break;
						}

						bool optionPredicate() => o.HasFlag(JsonNumberHandling.AllowNamedFloatingPointLiterals);
						targetCollection.Add(
							reader.GetString() switch
							{
								nameof(double.NaN) when optionPredicate() => double.NaN,
								"Infinity" when optionPredicate() => double.PositiveInfinity,
								"-Infinity" when optionPredicate() => double.NegativeInfinity,
								{ } s => double.Parse(s),
								_ => throw new JsonException()
							}
						);

						break;
					}
					case JsonTokenType.EndArray:
					{
						goto ReturnValue;
					}
					default:
					{
						throw new JsonException();
					}
				}
			}

		ReturnValue:
			return [.. targetCollection];
		}

		/// <inheritdoc/>
		public override void Write(Utf8JsonWriter writer, DashArray value, JsonSerializerOptions options)
		{
			writer.WriteStartArray();
			foreach (var element in value)
			{
				switch (options)
				{
					case { NumberHandling: var o } when o.HasFlag(JsonNumberHandling.WriteAsString):
					{
						writer.WriteStringValue(element.ToString());
						break;
					}
					case { NumberHandling: var o } when o.HasFlag(JsonNumberHandling.AllowNamedFloatingPointLiterals):
					{
						writer.WriteStringValue(
							element switch
							{
								double.NaN => nameof(double.NaN),
								double.NegativeInfinity => "-Infinity",
								double.PositiveInfinity => "Infinity",
								_ => element.ToString()
							}
						);
						break;
					}
					default:
					{
						writer.WriteNumberValue(element);
						break;
					}
				}
			}

			writer.WriteEndArray();
		}
	}
}
