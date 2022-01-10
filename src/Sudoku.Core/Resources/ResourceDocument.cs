namespace Sudoku.Resources;

/// <summary>
/// Defines a resource document. The resource document always holds a JSON string value,
/// and only contains key-value pairs of type <see cref="string"/>. For example:
/// <code>
/// {
///   "prop1": "value1",
///   "prop2": "value2",
///   "prop3": "value3"
/// }
/// </code>
/// In this case, we can use the indexer <see cref="this[string]"/> to fetch the value via the key,
/// for example, <c>document["prop1"]</c> you'll get the value <c>"value1"</c>.
/// </summary>
public sealed partial class ResourceDocument
: IDisposable
, IEquatable<ResourceDocument>
, ISimpleParseable<ResourceDocument>
{
	/// <summary>
	/// Indicates the default JSON document options.
	/// </summary>
	private static readonly JsonDocumentOptions DefaultDocumentOptions = new()
	{
		AllowTrailingCommas = true,
		MaxDepth = 1,
		CommentHandling = JsonCommentHandling.Skip
	};

	/// <summary>
	/// Indicates the default serializer options used for serialization and deserialization operations.
	/// </summary>
	private static readonly JsonSerializerOptions DefaultSerializerOptions = new()
	{
		AllowTrailingCommas = true,
		MaxDepth = 1,
		NumberHandling = JsonNumberHandling.WriteAsString,
		WriteIndented = true,
#if FORCE_CAMEL_CASE_ON_DESERIALIZATION
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase
#endif
	};


	/// <summary>
	/// Indicates the root JSON element.
	/// </summary>
	private readonly JsonElement _root;

	/// <summary>
	/// Indicates the corresponding JSON document.
	/// </summary>
	private readonly JsonDocument _parentDoc;

	/// <summary>
	/// Indicates the culture used.
	/// </summary>
	private readonly CultureInfo _culture;


	/// <summary>
	/// Initializes a <see cref="ResourceDocument"/> instance via the specified JSON string.
	/// </summary>
	/// <param name="culture">Indicates the culture specified.</param>
	/// <param name="json">The JSON string.</param>
	/// <exception cref="ArgumentException">
	/// Throws when the specified string isn't a JSON or can't be converted to a regular JSON string.
	/// </exception>
	/// <exception cref="JsonException">Throws when the specified string isn't valid JSON code.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ResourceDocument(CultureInfo culture, string json)
	{
		_culture = culture;
		_parentDoc = JsonDocument.Parse(json, DefaultDocumentOptions);
		_root = _parentDoc.RootElement;

		int count = 0;
		foreach (var property in _root.EnumerateObject())
		{
			if (property is { Name: var name, Value.ValueKind: not JsonValueKind.String })
			{
				throw new JsonException("Parsed failed: values can't be non-strings.");
			}

			count++;
		}

		Count = count;
	}


	/// <summary>
	/// Indicates the number of elements in the whole document.
	/// </summary>
	public int Count { get; }

	/// <summary>
	/// Indicates the LCID the current document holds.
	/// </summary>
	public int Lcid => _culture.LCID;


	/// <summary>
	/// Try to fetch the property value via the specified property name.
	/// </summary>
	/// <param name="propertyName">The property name to find.</param>
	/// <returns>The value that correspoding to the property.</returns>
	/// <exception cref="KeyNotFoundException">Throws when no such property found.</exception>
	public string this[string propertyName]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _root.GetProperty(propertyName).GetString()!;
	}


	/// <inheritdoc cref="IDisposable.Dispose"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Dispose() => _parentDoc.Dispose();

	/// <summary>
	/// Determine whether the specified <see cref="ResourceDocument"/> instance holds the same value
	/// as the current instance.
	/// </summary>
	/// <param name="other">The instance to compare.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals([NotNullWhen(true)] ResourceDocument? other) =>
		other is not null && _culture.LCID == other._culture.LCID && ToString() == other.ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => HashCode.Combine(_culture.LCID, ToString(DefaultSerializerOptions));

	/// <inheritdoc cref="object.ToString"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() => ToString(DefaultSerializerOptions);

	/// <summary>
	/// Converts the current instance to <see cref="string"/> representation in JSON format,
	/// via the specified <see cref="JsonSerializerOptions"/> instance as your customized options.
	/// </summary>
	/// <param name="options">The customized options being used in this method.</param>
	/// <returns>The <see cref="string"/> JSON representation.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(JsonSerializerOptions? options) => JsonSerializer.Serialize(_root, options);

	/// <summary>
	/// Gets the enumerator of the current instance in order to use <see langword="foreach"/> loop.
	/// </summary>
	/// <returns>The enumerator instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Enumerator GetEnumerator() => new(_root);


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryParse([NotNullWhen(true)] string? str, [NotNullWhen(true)] out ResourceDocument? result) =>
		TryParse(str, CultureInfo.CurrentCulture, out result);

	/// <summary>
	/// Try to parse the specified string text and get the same-meaning instance
	/// of type <see cref="ResourceDocument"/>, with the specified <see cref="CultureInfo"/>.
	/// </summary>
	/// <param name="str">The string to parse. The value shouldn't <see langword="null"/>.</param>
	/// <param name="culture">The culture specified.</param>
	/// <param name="result">
	/// The result parsed. If failed to parse, the value will keep the <see langword="default"/> value,
	/// i.e. <see langword="default"/>(<see cref="ResourceDocument"/>).
	/// </param>
	/// <returns>A <see cref="bool"/> result indicating whether the operation is successful to execute.</returns>
	public static bool TryParse(
		[NotNullWhen(true)] string? str,
		CultureInfo? culture,
		[NotNullWhen(true)] out ResourceDocument? result
	)
	{
		try
		{
			result = Parse(str, culture);
			return true;
		}
		catch (Exception ex) when (ex is JsonException or ArgumentException)
		{
			result = null;
			return false;
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ResourceDocument Parse(string? str) => Parse(str, CultureInfo.CurrentCulture);

	/// <summary>
	/// Parse the specified string text, and get the same-meaning instance of type <see cref="ResourceDocument"/>.
	/// </summary>
	/// <param name="str">The string to parsed. The value shouldn't be <see langword="null"/>.</param>
	/// <param name="culture">The culture specified.</param>
	/// <returns>The result parsed.</returns>
	/// <exception cref="ArgumentException">
	/// Throws when the specified string isn't a JSON or can't be converted to a regular JSON string.
	/// </exception>
	/// <exception cref="JsonException">Throws when the specified string isn't valid JSON code.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ResourceDocument Parse(string? str, CultureInfo? culture)
	{
		Nullability.ThrowIfNull(str);

		return new(culture ?? CultureInfo.CurrentCulture, str);
	}
}
