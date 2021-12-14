namespace Sudoku.Text;

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
public readonly partial struct ResourceDocument : IEquatable<ResourceDocument>, IValueEquatable<ResourceDocument>
{
	/// <summary>
	/// Indicates the default JSON node options.
	/// </summary>
	private static readonly JsonNodeOptions DefaultNodeOptions = new() { PropertyNameCaseInsensitive = false };

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
	/// Indicates the first JSON node.
	/// </summary>
	private readonly JsonNode _node;

	/// <summary>
	/// Indicates the current JSON object corresponding to.
	/// </summary>
	private readonly JsonObject _object;


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
		validate(
			_object = (
				_node = JsonNode.Parse(json, DefaultNodeOptions, DefaultDocumentOptions)
					?? throw new ArgumentException("The specified string can't be converted to regular JSON.")
			).AsObject()
		);


		static void validate(JsonObject @object)
		{
			foreach (var (key, value) in @object)
			{
				if (value is null || !value.AsValue().TryGetValue<string>(out _))
				{
					throw new JsonException(
						"The specified JSON code isn't regular one and not supported by resource document.",
						value?.GetPath(),
						null,
						null
					);
				}
			}
		}
	}


	/// <summary>
	/// Indicates the number of values stored in this document.
	/// </summary>
	public int Count
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _node.AsObject().Count;
	}


	/// <summary>
	/// Try to fetch the property value via the specified property name.
	/// </summary>
	/// <param name="propertyName">The property name to find.</param>
	/// <returns>The value that correspoding to the property.</returns>
	/// <exception cref="KeyNotFoundException">Throws when no such property found.</exception>
	public string this[string propertyName]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _node[propertyName]?.GetValue<string>() ?? throw new KeyNotFoundException("No such property found.");
	}


	/// <inheritdoc/>
	/// <remarks>
	/// Supported types of the parameter <paramref name="obj"/> are:
	/// <list type="bullet">
	/// <item><see cref="string"/> - Compare the JSON raw string formatted and indented.</item>
	/// <item><see cref="JsonObject"/> - Compare the JSON raw string formatted and indented.</item>
	/// <item>
	/// <see cref="JsonNode"/> - Compare the JSON raw string that the object
	/// converted to <see cref="JsonObject"/>, formatted and indented.
	/// </item>
	/// <item><see cref="ResourceDocument"/> - Compare the content.</item>
	/// <item>Otherwise - <see langword="false"/> will be returned.</item>
	/// </list>
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] object? obj) => obj switch
	{
		null => false,
		string json => _object.ToJsonString() == json,
		JsonObject @object => _object.ToJsonString() == @object.ToJsonString(),
		JsonNode node => _node.ToJsonString() == node.ToJsonString(),
		ResourceDocument doc => Equals(doc),
		_ => obj.ToString() == ToString()
	};

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(ResourceDocument other) => ToString() == other.ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => ToString().GetHashCode();

	/// <inheritdoc cref="object.ToString"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() => ToString(DefaultSerializerOptions);

	/// <summary>
	/// Converts the current instance to <see cref="string"/> representation in JSON format, via the specified
	/// <see cref="JsonSerializerOptions"/> instance as your customized options.
	/// </summary>
	/// <param name="options">The customized options being used in this method.</param>
	/// <returns>The <see cref="string"/> JSON representation.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(JsonSerializerOptions? options) => _node.ToJsonString(options ?? DefaultSerializerOptions);

	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Enumerator GetEnumerator() => new(_object);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	bool IValueEquatable<ResourceDocument>.Equals(in ResourceDocument other) => Equals(other);


	/// <inheritdoc cref="IEqualityOperators{TSelf, TOther}.operator =="/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(ResourceDocument left, ResourceDocument right) => left.Equals(right);

	/// <inheritdoc cref="IEqualityOperators{TSelf, TOther}.operator !="/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(ResourceDocument left, ResourceDocument right) => !(left == right);
}
