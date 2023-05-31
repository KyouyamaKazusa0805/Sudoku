namespace SudokuStudio.Collection;

/// <summary>
/// Defines a dash array of <see cref="double"/> values used by <see cref="DoubleCollection"/>-typed properties in controls,
/// for example, <see cref="Shape.StrokeDashArray"/>.
/// </summary>
/// <seealso cref="Shape.StrokeDashArray"/>
[JsonConverter(typeof(Converter))]
public readonly partial struct DashArray : IEnumerable<double>, IEquatable<DashArray>, IEqualityOperators<DashArray, DashArray, bool>
{
	/// <summary>
	/// Indicates the invalid value.
	/// </summary>
	public static readonly DashArray InvalidValue = new(0);


	/// <summary>
	/// The double values.
	/// </summary>
	internal readonly double[] _doubles = Array.Empty<double>();


	/// <summary>
	/// Initializes a <see cref="DashArray"/> instance.
	/// </summary>
	public DashArray()
	{
	}

	/// <summary>
	/// Initializes a <see cref="DashArray"/> instance via the specified list of values.
	/// </summary>
	/// <param name="doubles">A list of values.</param>
	public DashArray(params double[] doubles)
	{
		_doubles = new double[doubles.Length];

		Array.Copy(doubles, 0, _doubles, 0, doubles.Length);
	}


	/// <summary>
	/// Indicates the number of values.
	/// </summary>
	[JsonIgnore]
	public int Count => _doubles.Length;

	[JsonIgnore]
	private string ValuesString => $"[{string.Join(", ", _doubles)}]";


	[GeneratedOverridingMember(GeneratedEqualsBehavior.TypeCheckingAndCallingOverloading)]
	public override partial bool Equals(object? obj);

	/// <inheritdoc/>
	public bool Equals(DashArray other)
	{
		scoped var l = _doubles.AsSpan();
		scoped var r = other._doubles.AsSpan();
		return l.SequenceEqual(r);
	}

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

	[GeneratedOverridingMember(GeneratedToStringBehavior.SimpleMember, nameof(ValuesString))]
	public override partial string ToString();

	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	public Enumerator GetEnumerator() => new(_doubles);

	/// <summary>
	/// Converts the current collection into a <see cref="DoubleCollection"/> instance.
	/// </summary>
	/// <returns>A <see cref="DoubleCollection"/> result.</returns>
	public DoubleCollection ToDoubleCollection()
	{
		var result = new DoubleCollection();
		foreach (var element in _doubles)
		{
			result.Add(element);
		}

		return result;
	}

	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator() => _doubles.GetEnumerator();

	/// <inheritdoc/>
	IEnumerator<double> IEnumerable<double>.GetEnumerator() => ((IEnumerable<double>)_doubles).GetEnumerator();


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(DashArray left, DashArray right) => left.Equals(right);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(DashArray left, DashArray right) => !(left == right);
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
				case JsonTokenType.String when options.NumberHandling is var o && o.Flags(JsonNumberHandling.AllowReadingFromString):
				{
					if (reader.TryGetDouble(out var value))
					{
						targetCollection.Add(value);
						break;
					}

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


					bool optionPredicate() => o.Flags(JsonNumberHandling.AllowNamedFloatingPointLiterals);
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
		return new(targetCollection.ToArray());
	}

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, DashArray value, JsonSerializerOptions options)
	{
		writer.WriteStartArray();
		foreach (var element in value._doubles)
		{
			switch (options)
			{
				case { NumberHandling: var o } when o.Flags(JsonNumberHandling.WriteAsString):
				{
					writer.WriteStringValue(element.ToString());
					break;
				}
				case { NumberHandling: var o } when o.Flags(JsonNumberHandling.AllowNamedFloatingPointLiterals):
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
