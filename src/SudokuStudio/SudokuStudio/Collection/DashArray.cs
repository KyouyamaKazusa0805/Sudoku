namespace SudokuStudio.Collection;

/// <summary>
/// Defines a dash array of <see cref="double"/> values used by <see cref="DoubleCollection"/>-typed properties in controls,
/// for example, <see cref="Shape.StrokeDashArray"/>.
/// </summary>
/// <seealso cref="Shape.StrokeDashArray"/>
[GeneratedOverloadingOperator(GeneratedOperator.EqualityOperators)]
[JsonConverter(typeof(Converter))]
public readonly partial struct DashArray : IEnumerable<double>, IEquatable<DashArray>, IEqualityOperators<DashArray, DashArray, bool>
{
	/// <summary>
	/// The double values.
	/// </summary>
	[FileAccessOnly]
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
	[DebuggerHidden]
	private string ValuesString => $"[{string.Join(", ", _doubles)}]";


	[GeneratedOverriddingMember(GeneratedEqualsBehavior.TypeCheckingAndCallingOverloading)]
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

	[GeneratedOverriddingMember(GeneratedToStringBehavior.SimpleMember, nameof(ValuesString))]
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


	/// <summary>
	/// Defines an enumerator of this type.
	/// </summary>
	public ref partial struct Enumerator
	{
		/// <summary>
		/// Initializes a <see cref="Enumerator"/> instance.
		/// </summary>
		/// <param name="doubles">The double values.</param>
		[FileAccessOnly]
		internal Enumerator(double[] doubles) => _doubles = doubles;
	}
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
		if (typeToConvert != typeof(DashArray))
		{
			throw new JsonException();
		}

		if (reader.TokenType != JsonTokenType.StartArray)
		{
			throw new JsonException();
		}

		var targetCollection = new List<double>();
		while (reader.Read())
		{
			switch (reader.TokenType)
			{
				case JsonTokenType.Number:
				{
					targetCollection.Add(reader.GetDouble());
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
		return new(targetCollection.ToArray());
	}

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, DashArray value, JsonSerializerOptions options)
		=> writer.WriteArray(value._doubles, options);
}
