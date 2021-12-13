namespace Sudoku.Text;

/// <summary>
/// Defines a resource document. The resource document should be a JSON document of depth 1,
/// and only holds key-value pairs of type <see cref="string"/>. For example:
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
public readonly partial struct ResourceDocument
{
	/// <summary>
	/// Indicates the first element of the document.
	/// </summary>
	private readonly JsonElement _firstElement;


	/// <summary>
	/// Initializes a <see cref="ResourceDocument"/> instance via the specified JSON string.
	/// </summary>
	/// <param name="json">The JSON code.</param>
	/// <param name="instantValidate">
	/// Indicates whether the constructor will validate the JSON code as the regular resource document.
	/// The default value is <see langword="false"/>.
	/// </param>
	/// <exception cref="JsonException">Throws when the specified string isn't valid JSON code.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ResourceDocument(string json, bool instantValidate = false)
	{
		_firstElement = JsonSerializer.Deserialize<JsonElement>(json, new JsonSerializerOptions
		{
			AllowTrailingCommas = true,
			PropertyNameCaseInsensitive = false,
			MaxDepth = 1,
			ReadCommentHandling = JsonCommentHandling.Skip,
			NumberHandling = JsonNumberHandling.WriteAsString,
			Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
#if ONLY_SUPPORT_CAMEL_CASE_VIA_SERIALIZATION
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase
#endif
		});

		if (instantValidate && !f(_firstElement, out string? wrongPropertyName))
		{
			throw new JsonException(
				"The specified JSON code isn't regular one and not supported by resource document.",
				new InvalidCastException($"The property {wrongPropertyName} holds a non-string value.")
			);
		}


		static bool f(JsonElement firstElement, [NotNullWhen(false)] out string? wrongPropertyName)
		{
			foreach (var property in firstElement.EnumerateObject())
			{
				if (property is { Value.ValueKind: not JsonValueKind.String, Name: var name })
				{
					wrongPropertyName = name;
					return false;
				}
			}

			wrongPropertyName = null;
			return true;
		}
	}


	/// <summary>
	/// Try to fetch the property value via the specified property name.
	/// </summary>
	/// <param name="propertyName">The property name to find.</param>
	/// <returns>The value that correspoding to the property.</returns>
	/// <exception cref="KeyNotFoundException">Throws when no such property found.</exception>
	/// <exception cref="JsonException">
	/// Throws when the return value is <see langword="null"/>, which is disallowed.
	/// </exception>
	/// <exception cref="InvalidOperationException">
	/// Throws when the return value isn't a <see cref="string"/>, which is disallowed.
	/// </exception>
	public string this[string propertyName]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get =>
			_firstElement.GetProperty(propertyName) is { ValueKind: JsonValueKind.String } result
				? result.GetString() ?? throw new JsonException("Property path can't contain null value.")
				: throw new InvalidOperationException("The result value isn't a string, which is disallowed.");
	}


	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Enumerator GetEnumerator() => new(_firstElement);
}
