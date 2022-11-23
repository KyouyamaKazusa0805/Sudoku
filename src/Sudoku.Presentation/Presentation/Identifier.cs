namespace Sudoku.Presentation;

/// <summary>
/// Defines an identifier that can differ colors.
/// </summary>
[JsonConverter(typeof(JsonConverter))]
public readonly partial struct Identifier : IEquatable<Identifier>, IEqualityOperators<Identifier, Identifier, bool>
{
#pragma warning disable CS0618
	/// <summary>
	/// Initializes an <see cref="Identifier"/> instance.
	/// </summary>
	[FileAccessOnly]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Identifier()
	{
	}

	/// <summary>
	/// Initializes an <see cref="Identifier"/> instance via the ID value.
	/// </summary>
	/// <param name="id">The ID value.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private Identifier(int id) : this() => (Mode, Id) = (IdentifierColorMode.Id, id);

	/// <summary>
	/// Initializes an <see cref="Identifier"/> instance via the color value.
	/// </summary>
	/// <param name="a">The alpha value.</param>
	/// <param name="r">The red value.</param>
	/// <param name="g">The green value.</param>
	/// <param name="b">The blue value.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private Identifier(byte a, byte r, byte g, byte b) : this()
		=> (Mode, ColorRawValue) = (IdentifierColorMode.Raw, a << 24 | r << 16 | g << 8 | b);

	/// <summary>
	/// Initializes an <see cref="Identifier"/> instance via the specified displaying color kind as the named kind.
	/// </summary>
	/// <param name="namedKind">The color kind.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private Identifier(DisplayColorKind namedKind) : this() => (Mode, NamedKind) = (IdentifierColorMode.Named, namedKind);
#pragma warning restore CS0618


	/// <summary>
	/// Indicates the alpha value.
	/// <b>The value can only be used when the <see cref="Mode"/> is <see cref="IdentifierColorMode.Raw"/>.</b>
	/// </summary>
	public byte A => (byte)(ColorRawValue >> 24 & byte.MaxValue);

	/// <summary>
	/// Indicates the red value.
	/// <b>The value can only be used when the <see cref="Mode"/> is <see cref="IdentifierColorMode.Raw"/>.</b>
	/// </summary>
	public byte R => (byte)(ColorRawValue >> 16 & byte.MaxValue);

	/// <summary>
	/// Indicates the green value.
	/// <b>The value can only be used when the <see cref="Mode"/> is <see cref="IdentifierColorMode.Raw"/>.</b>
	/// </summary>
	public byte G => (byte)(ColorRawValue >> 8 & byte.MaxValue);

	/// <summary>
	/// Indicates the blue value.
	/// <b>The value can only be used when the <see cref="Mode"/> is <see cref="IdentifierColorMode.Raw"/>.</b>
	/// </summary>
	public byte B => (byte)(ColorRawValue & byte.MaxValue);

	/// <summary>
	/// Indicates the ID value used.
	/// <b>The value can only be used when the <see cref="Mode"/> is <see cref="IdentifierColorMode.Id"/>.</b>
	/// </summary>
	public int Id { get; }

	/// <summary>
	/// Indicates the raw color value.
	/// </summary>
	public int ColorRawValue { get; }

	/// <summary>
	/// Indicates the kind of the identifier named.
	/// <b>The value can only be used when the <see cref="Mode"/> is <see cref="IdentifierColorMode.Named"/>.</b>
	/// </summary>
	public DisplayColorKind NamedKind { get; }

	/// <summary>
	/// Indicates the mode.
	/// </summary>
	public IdentifierColorMode Mode { get; }

	private string RawValueDisplayer
		=> Mode switch
		{
			IdentifierColorMode.Id => $"ID = {Id}",
			IdentifierColorMode.Raw => $"Color = #{A:X2}{R:X2}{G:X2}{B:X2}",
			IdentifierColorMode.Named => $"{nameof(NamedKind)} = {NamedKind}",
			_ => "<Unknown mode>"
		};

	private int TemporaryHashCodeBase
		=> Mode switch
		{
			IdentifierColorMode.Id => Id,
			IdentifierColorMode.Raw => ColorRawValue,
			IdentifierColorMode.Named => NamedKind.GetHashCode(),
			_ => 0xDEAD
		};


	[GeneratedOverriddingMember(GeneratedEqualsBehavior.TypeCheckingAndCallingOverloading)]
	public override partial bool Equals(object? obj);

	/// <inheritdoc/>
	/// <exception cref="NotSupportedException">Throws when the specified mode is not supported.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(Identifier other)
		=> Mode == other.Mode
		&& Mode switch
		{
			IdentifierColorMode.Raw => A == other.A && R == other.R && G == other.G && B == other.B,
			IdentifierColorMode.Id => Id == other.Id,
			IdentifierColorMode.Named => NamedKind == other.NamedKind,
			_ => throw new NotSupportedException("The specified mode is not supported.")
		};

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(Mode), nameof(TemporaryHashCodeBase))]
	public override partial int GetHashCode();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() => $$"""{{nameof(Identifier)}} { {{RawValueDisplayer}} }""";


	/// <summary>
	/// Creates an <see cref="Identifier"/> instance via the specified ID value.
	/// </summary>
	/// <param name="id">The ID value.</param>
	/// <returns>The result <see cref="Identifier"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Identifier FromId(int id) => new(id);

	/// <summary>
	/// Creates an <see cref="Identifier"/> instance via the color value.
	/// </summary>
	/// <param name="r">The red value.</param>
	/// <param name="g">The green value.</param>
	/// <param name="b">The blue value.</param>
	/// <returns>The result <see cref="Identifier"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Identifier FromColor(byte r, byte g, byte b) => new(byte.MaxValue, r, g, b);

	/// <summary>
	/// Creates an <see cref="Identifier"/> instance via the color value.
	/// </summary>
	/// <param name="a">The alpha value.</param>
	/// <param name="r">The red value.</param>
	/// <param name="g">The green value.</param>
	/// <param name="b">The blue value.</param>
	/// <returns>The result <see cref="Identifier"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Identifier FromColor(byte a, byte r, byte g, byte b) => new(a, r, g, b);

	/// <summary>
	/// Creates an <see cref="Identifier"/> instance via the named kind.
	/// </summary>
	/// <param name="namedKind">The named kind.</param>
	/// <returns>The result <see cref="Identifier"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Identifier FromNamedKind(DisplayColorKind namedKind) => new(namedKind);


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(Identifier left, Identifier right) => left.Equals(right);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(Identifier left, Identifier right) => !(left == right);


	/// <summary>
	/// Explicit cast from <see cref="Identifier"/> to <see cref="int"/> indicating the ID value.
	/// </summary>
	/// <param name="identifier">The <see cref="Identifier"/> instance.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator int(Identifier identifier)
		=> identifier.Mode == IdentifierColorMode.Id
			? identifier.Id
			: throw new InvalidCastException("The instance cannot be converted to an 'int' due to invalid status.");

	/// <summary>
	/// Explicit cast from <see cref="Identifier"/> to <see cref="DisplayColorKind"/> indicating the named kind value.
	/// </summary>
	/// <param name="identifier">The <see cref="Identifier"/> instance.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator DisplayColorKind(Identifier identifier)
		=> identifier.Mode == IdentifierColorMode.Named
			? identifier.NamedKind
			: throw new InvalidCastException("The instance cannot be converted to a 'DisplayColorKind' due to invalid status.");

	/// <summary>
	/// Explicit cast from <see cref="Identifier"/> to (<see cref="byte"/>, <see cref="byte"/>,
	/// <see cref="byte"/>).
	/// </summary>
	/// <param name="identifier">The <see cref="Identifier"/> instance.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator (byte R, byte G, byte B)(Identifier identifier)
		=> identifier.Mode == IdentifierColorMode.Raw && identifier.A == byte.MaxValue
			? (identifier.R, identifier.G, identifier.B)
			: throw new InvalidCastException("The instance cannot be converted to a triple due to invalid status.");

	/// <summary>
	/// Explicit cast from <see cref="Identifier"/> to (<see cref="byte"/>, <see cref="byte"/>,
	/// <see cref="byte"/>, <see cref="byte"/>).
	/// </summary>
	/// <param name="identifier">The <see cref="Identifier"/> instance.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator (byte A, byte R, byte G, byte B)(Identifier identifier)
		=> identifier.Mode == IdentifierColorMode.Raw
			? (identifier.A, identifier.R, identifier.G, identifier.B)
			: throw new InvalidCastException("The instance cannot be converted to a quadruple due to invalid status.");

	/// <summary>
	/// Implicit cast from (<see cref="byte"/>, <see cref="byte"/>, <see cref="byte"/>, <see cref="byte"/>)
	/// to <see cref="Identifier"/>.
	/// </summary>
	/// <param name="colorQuadruple">
	/// The quadruple of element types <see cref="byte"/>, <see cref="byte"/>, <see cref="byte"/>
	/// and <see cref="byte"/>.
	/// </param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Identifier((byte A, byte R, byte G, byte B) colorQuadruple)
		=> new(colorQuadruple.A, colorQuadruple.R, colorQuadruple.G, colorQuadruple.B);

	/// <summary>
	/// Implicit cast from (<see cref="byte"/>, <see cref="byte"/>, <see cref="byte"/>)
	/// to <see cref="Identifier"/>.
	/// </summary>
	/// <param name="colorTriple">
	/// The quadruple of element types <see cref="byte"/>, <see cref="byte"/> and <see cref="byte"/>.
	/// </param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Identifier((byte R, byte G, byte B) colorTriple)
		=> new(byte.MaxValue, colorTriple.R, colorTriple.G, colorTriple.B);

	/// <summary>
	/// Implicit cast from <see cref="int"/> indicating the ID value to <see cref="Identifier"/>.
	/// </summary>
	/// <param name="id">The ID value.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Identifier(int id) => new(id);

	/// <summary>
	/// Implicit cast from <see cref="DisplayColorKind"/> indicating the displaying color kind as the named kind,
	/// as the ID value to <see cref="Identifier"/>.
	/// </summary>
	/// <param name="namedKind">The displaying color kind.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Identifier(DisplayColorKind namedKind) => new(namedKind);
}


/// <summary>
/// Defines a JSON converter that is used for the serialization and deserialization on type <see cref="Identifier"/>.
/// </summary>
/// <remarks>
/// JSON Pattern:
/// <code>
/// {
///   "mode": "Raw",
///   "value": {
///     "a": 255,
///     "r": 0,
///     "g": 0,
///     "b": 0
///   }
/// }
/// </code>
/// </remarks>
/// <seealso cref="Identifier"/>
file sealed class JsonConverter : JsonConverter<Identifier>
{
	/// <summary>
	/// Indicates the property name <c>"Value"</c>.
	/// </summary>
	private const string ValuePropertyName = "Value";


	/// <inheritdoc/>
	public override Identifier Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
		{
			throw new JsonException();
		}

		if (!reader.Read()
			|| reader.TokenType != JsonTokenType.PropertyName
			|| reader.GetString() != ConvertName(nameof(Identifier.Mode), options))
		{
			throw new JsonException();
		}

		if (!reader.Read() || reader.TokenType != JsonTokenType.PropertyName
			|| reader.GetString() != ConvertName(ValuePropertyName, options))
		{
			throw new JsonException();
		}

		if (!reader.Read() || reader.TokenType != JsonTokenType.String)
		{
			throw new JsonException();
		}

		var mode = Enum.Parse<IdentifierColorMode>(reader.GetString() ?? throw new JsonException(), true);
		return mode switch
		{
			IdentifierColorMode.Id => getId(ref reader),
			IdentifierColorMode.Raw => getColor(ref reader),
			IdentifierColorMode.Named => getNamedKind(ref reader),
			_ => throw new JsonException()
		};


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static int getId(ref Utf8JsonReader reader)
		{
			if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
			{
				throw new JsonException();
			}

			var raw = (IdInternal)Deserialize(ref reader, typeof(IdInternal), IdInternalSerializerContext.Default)!;
			if (!reader.Read() || reader.TokenType != JsonTokenType.EndObject)
			{
				throw new JsonException();
			}

			return raw.Id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static (byte, byte, byte, byte) getColor(ref Utf8JsonReader reader)
		{
			if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
			{
				throw new JsonException();
			}

			var raw = (ColorInternal)Deserialize(ref reader, typeof(ColorInternal), ColorInternalSerializerContext.Default)!;
			if (!reader.Read() || reader.TokenType != JsonTokenType.EndObject)
			{
				throw new JsonException();
			}

			return (raw.A, raw.R, raw.G, raw.B);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static DisplayColorKind getNamedKind(ref Utf8JsonReader reader)
		{
			if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
			{
				throw new JsonException();
			}

			var raw = (NamedKindInternal)Deserialize(ref reader, typeof(NamedKindInternal), NamedKindInternalJsonSerializerContext.Default)!;
			if (!reader.Read() || reader.TokenType != JsonTokenType.EndObject)
			{
				throw new JsonException();
			}

			return Enum.Parse<DisplayColorKind>(raw.NamedKind, true);
		}
	}

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, Identifier value, JsonSerializerOptions options)
	{
		writer.WriteStartObject();
		writer.WriteString(ConvertName(nameof(value.Mode), options), value.Mode.ToString());
		writer.WritePropertyName(ConvertName(ValuePropertyName, options));
		writer.WriteStartObject();
		switch (value.Mode)
		{
			case IdentifierColorMode.Id:
			{
				writer.WriteNumber(nameof(value.Id), value.Id);
				break;
			}
			case IdentifierColorMode.Raw:
			{
				writer.WriteNumber(ConvertName(nameof(value.A), options), value.A);
				writer.WriteNumber(ConvertName(nameof(value.R), options), value.R);
				writer.WriteNumber(ConvertName(nameof(value.G), options), value.G);
				writer.WriteNumber(ConvertName(nameof(value.B), options), value.B);
				break;
			}
			case IdentifierColorMode.Named:
			{
				writer.WriteString(ConvertName(nameof(value.NamedKind), options), value.NamedKind.ToString());
				break;
			}
		}
		writer.WriteEndObject();
		writer.WriteEndObject();
	}


	/// <summary>
	/// Try to convert the specified string value as the specified naming policy
	/// specified in argument <paramref name="options"/>.
	/// </summary>
	/// <param name="base">The string to convert.</param>
	/// <param name="options">The options.</param>
	/// <returns>The value converted.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static string ConvertName(string @base, JsonSerializerOptions options)
		=> options.PropertyNamingPolicy?.ConvertName(@base) ?? @base;
}
