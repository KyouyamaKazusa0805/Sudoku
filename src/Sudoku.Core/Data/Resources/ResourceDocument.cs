namespace Sudoku.Data.Resources;

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
[AutoGetEnumerator(nameof(_root), MemberConversion = "new(@)", ReturnType = typeof(Enumerator))]
public readonly partial struct ResourceDocument
: IDisposable
, IEquatable<ResourceDocument>
, IEqualityOperators<ResourceDocument, ResourceDocument>
, ISimpleParseable<ResourceDocument>
, IValueEquatable<ResourceDocument>
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
	/// Initializes a <see cref="ResourceDocument"/> instance via the specified JSON string.
	/// </summary>
	/// <param name="json">The JSON string.</param>
	/// <exception cref="ArgumentException">
	/// Throws when the specified string isn't a JSON or can't be converted to a regular JSON string.
	/// </exception>
	/// <exception cref="JsonException">Throws when the specified string isn't valid JSON code.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ResourceDocument(string json)
	{
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

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] object? obj) =>
		obj is ResourceDocument comparer && Equals(comparer);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(ResourceDocument other) =>
		ToString(DefaultSerializerOptions) == other.ToString(DefaultSerializerOptions);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => ToString(DefaultSerializerOptions).GetHashCode();

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

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	bool IValueEquatable<ResourceDocument>.Equals(in ResourceDocument other) => Equals(other);


	/// <inheritdoc/>
	public static bool TryParse([NotNullWhen(true)] string? str, out ResourceDocument result)
	{
		try
		{
			result = Parse(str);
			return true;
		}
		catch (Exception ex) when (ex is JsonException or ArgumentException)
		{
			result = default;
			return false;
		}
	}

	/// <inheritdoc/>
	/// <exception cref="ArgumentException">
	/// Throws when the specified string isn't a JSON or can't be converted to a regular JSON string.
	/// </exception>
	/// <exception cref="JsonException">Throws when the specified string isn't valid JSON code.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ResourceDocument Parse(string? str)
	{
		Nullability.ThrowIfNull(str);

		return new(str);
	}


	/// <inheritdoc cref="IEqualityOperators{TSelf, TOther}.operator =="/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(ResourceDocument left, ResourceDocument right) => left.Equals(right);

	/// <inheritdoc cref="IEqualityOperators{TSelf, TOther}.operator !="/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(ResourceDocument left, ResourceDocument right) => !(left == right);
}
