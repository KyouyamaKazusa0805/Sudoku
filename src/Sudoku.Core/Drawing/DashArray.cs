namespace Sudoku.Drawing;

/// <summary>
/// Defines a dash array of <see cref="double"/> values. The values can be used in UI as dash array of a double collection.
/// </summary>
[JsonConverter(typeof(Converter))]
[TypeImpl(TypeImplFlag.Object_Equals | TypeImplFlag.Object_ToString | TypeImplFlag.EqualityOperators)]
public readonly partial struct DashArray() : IEnumerable<double>, IEquatable<DashArray>, IEqualityOperators<DashArray, DashArray, bool>
{
	/// <summary>
	/// Indicates the invalid value.
	/// </summary>
	public static readonly DashArray InvalidValue = [0];


	/// <summary>
	/// The double values.
	/// </summary>
	private readonly List<double> _doubles = [];


	/// <summary>
	/// Indicates the number of values.
	/// </summary>
	[JsonIgnore]
	public int Count => _doubles.Count;

	[JsonIgnore]
	[StringMember]
	private string ValuesString => $"[{string.Join(", ", _doubles)}]";


	/// <summary>
	/// Adds a new value into the collection.
	/// </summary>
	/// <param name="value">The value.</param>
	public void Add(double value) => _doubles.Add(value);

	/// <inheritdoc/>
	public bool Equals(DashArray other) => _doubles.SequenceEqual(other._doubles);

	/// <summary>
	/// Converts the current collection into an array of <see cref="double"/> values.
	/// </summary>
	/// <returns>An array of <see cref="double"/> values.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public double[] ToArray() => [.. _doubles];

	/// <inheritdoc cref="object.GetHashCode"/>
	public override int GetHashCode()
	{
		var result = new HashCode();
		foreach (var element in _doubles)
		{
			result.Add(element);
		}

		return result.ToHashCode();
	}

	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	public Enumerator GetEnumerator() => new(_doubles);

	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator() => _doubles.GetEnumerator();

	/// <inheritdoc/>
	IEnumerator<double> IEnumerable<double>.GetEnumerator() => ((IEnumerable<double>)_doubles).GetEnumerator();
}

/// <summary>
/// Defines JSON serialization converter on type <see cref="DashArray"/>.
/// </summary>
/// <seealso cref="DashArray"/>
file sealed class Converter : JsonConverter<DashArray>
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
