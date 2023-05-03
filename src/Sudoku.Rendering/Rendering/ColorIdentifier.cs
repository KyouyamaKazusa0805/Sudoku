namespace Sudoku.Rendering;

/// <summary>
/// Represents an identifier that is used for describing target rendering item.
/// </summary>
[JsonConverter(typeof(JsonConverter))]
public abstract partial class ColorIdentifier : IEquatable<ColorIdentifier>, IEqualityOperators<ColorIdentifier, ColorIdentifier, bool>
{
	[GeneratedOverridingMember(GeneratedEqualsBehavior.AsCastAndCallingOverloading)]
	public sealed override partial bool Equals(object? obj);

	/// <inheritdoc/>
	public abstract bool Equals([NotNullWhen(true)] ColorIdentifier? other);

	/// <inheritdoc/>
	public abstract override int GetHashCode();

	/// <inheritdoc/>
	public abstract override string ToString();


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(ColorIdentifier? left, ColorIdentifier? right)
		=> (left, right) switch { (null, null) => true, (not null, not null) => left.Equals(right), _ => false };

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(ColorIdentifier? left, ColorIdentifier? right) => !(left == right);


	/// <summary>
	/// Implicit cast from <see cref="int"/> to <see cref="ColorIdentifier"/>.
	/// </summary>
	/// <param name="paletteId">The <see cref="int"/> instance indicating the palette ID.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator ColorIdentifier(int paletteId) => new PaletteIdColorIdentifier(paletteId);

	/// <summary>
	/// Implicit cast from <see cref="WellKnownColorIdentifierKind"/> to <see cref="ColorIdentifier"/>.
	/// </summary>
	/// <param name="kind">The <see cref="WellKnownColorIdentifierKind"/> instance.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator ColorIdentifier(WellKnownColorIdentifierKind kind) => new WellKnownColorIdentifier(kind);
}

/// <summary>
/// The file-local type that can serialize or deserialize a <see cref="ColorIdentifier"/> instance.
/// </summary>
/// <seealso cref="ColorIdentifier"/>
file sealed class JsonConverter : JsonConverter<ColorIdentifier>
{
	private const string PropertyNameType = "type";

	private const string PropertyNameValue = "value";


	/// <inheritdoc/>
	public override ColorIdentifier? Read(scoped ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		switch (reader.GetString())
		{
			case nameof(ColorColorIdentifier):
			{
				var inst = Deserialize<ColorColorIdentifier_DeserializationTempType>(ref reader, options);
				return new ColorColorIdentifier(inst.A, inst.R, inst.G, inst.B);
			}
			case nameof(PaletteIdColorIdentifier):
			{
				var inst = Deserialize<PaletteIdColorIdentifier_DeserializationTempType>(ref reader, options);
				return new PaletteIdColorIdentifier(inst.Value);
			}
			case nameof(WellKnownColorIdentifier):
			{
				var inst = Deserialize<WellKnownColorIdentifier_DeserializationTempType>(ref reader, options);
				return new WellKnownColorIdentifier(inst.Kind);
			}
			default:
			{
				throw new JsonException();
			}
		}
	}

	/// <inheritdoc/>
	public override void Write(Utf8JsonWriter writer, ColorIdentifier value, JsonSerializerOptions options)
	{
		switch (value)
		{
			case ColorColorIdentifier inst:
			{
				writer.WriteStartObject();
				writer.WriteString(PropertyNameType, nameof(ColorColorIdentifier));
				writer.WritePropertyName(PropertyNameValue);
				writer.WriteStartObject();
				Serialize(writer, new ColorColorIdentifier_DeserializationTempType { A = inst.A, R = inst.R, G = inst.G, B = inst.B }, options);
				writer.WriteEndObject();
				writer.WriteEndObject();

				break;
			}
			case PaletteIdColorIdentifier inst:
			{
				writer.WriteStartObject();
				writer.WriteString(PropertyNameType, nameof(PaletteIdColorIdentifier));
				writer.WritePropertyName(PropertyNameValue);
				writer.WriteStartObject();
				Serialize(writer, new PaletteIdColorIdentifier_DeserializationTempType { Value = inst.Value }, options);
				writer.WriteEndObject();
				writer.WriteEndObject();

				break;
			}
			case WellKnownColorIdentifier inst:
			{
				writer.WriteStartObject();
				writer.WriteString(PropertyNameType, nameof(WellKnownColorIdentifier));
				writer.WritePropertyName(PropertyNameValue);
				writer.WriteStartObject();
				Serialize(writer, new WellKnownColorIdentifier_DeserializationTempType { Kind = inst.Kind }, options);
				writer.WriteEndObject();
				writer.WriteEndObject();

				break;
			}
		}
	}
}

/// <summary>
/// The temporary and intermediate type used for deserialization on <see cref="ColorColorIdentifier"/> instances.
/// </summary>
file record struct ColorColorIdentifier_DeserializationTempType(byte A, byte R, byte G, byte B);

/// <summary>
/// The temporary and intermediate type used for deserialization on <see cref="PaletteIdColorIdentifier"/> instances.
/// </summary>
file record struct PaletteIdColorIdentifier_DeserializationTempType(int Value);

/// <summary>
/// The temporary and intermediate type used for deserialization on <see cref="WellKnownColorIdentifier"/> instances.
/// </summary>
file record struct WellKnownColorIdentifier_DeserializationTempType(WellKnownColorIdentifierKind Kind);
