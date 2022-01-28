using Handler = Sudoku.Resources.ResourceDocumentKeyNotFoundEventHandler;

namespace Sudoku.Resources;

/// <summary>
/// Defines a resource document manager.
/// </summary>
public sealed class ResourceDocumentManager : IEnumerable<ResourceDocument>
{
	/// <summary>
	/// Defines the inner collection to store documents.
	/// </summary>
	private readonly IDictionary<int, ResourceDocument> _list = new Dictionary<int, ResourceDocument>();


	/// <summary>
	/// Initializes a <see cref="ResourceDocumentManager"/> instance.
	/// </summary>
	internal ResourceDocumentManager()
	{
	}


	/// <summary>
	/// Indicates the number of all possible resource documents having stored in this manager.
	/// </summary>
	public int Count => _list.Count;

	/// <summary>
	/// Indicates the current LCID used.
	/// </summary>
	public int CurrentLcid { get; set; } = 1033;


	/// <summary>
	/// Defines a manager that is initialized by the module initializer.
	/// </summary>
	[InitializationOnly(InitializationCaller.ModuleInitializer)]
	public static ResourceDocumentManager Shared { get; internal set; } = null!;


	/// <summary>
	/// Try to get the property value via the property value, using the specified LCID.
	/// </summary>
	/// <param name="propertyName">The property name.</param>
	/// <returns>The property value found.</returns>
	/// <exception cref="KeyNotFoundException">
	/// Throws when the specified property name cannot be found.
	/// </exception>
	public string this[string propertyName]
	{
		get
		{
			// Searching for the target resource document.
			if (_list[CurrentLcid][propertyName, ResourceDocumentIndexerMode.NullableReturn] is { } r1)
			{
				return r1;
			}

			// Searching for the base resource document.
			if (_list[1033][propertyName, ResourceDocumentIndexerMode.NullableReturn] is { } r2)
			{
				return r2;
			}

			// Searching for external resources.
			foreach (Handler invocation in KeyNotFound?.GetInvocationList() ?? Array.Empty<Handler>())
			{
				if (invocation(this, propertyName) is { } r3)
				{
					return r3;
				}
			}

			// The key cannot be found in all dictionaries. Just throw exceptions to report the wrong case.
			throw new KeyNotFoundException($"The specified key cannot be found: {propertyName}.");
		}
	}

	/// <summary>
	/// Gets the specified resource document via the LCID value.
	/// </summary>
	/// <param name="lcid">The LCID.</param>
	/// <returns>
	/// The resource document. If the specified LCID cannot be found,
	/// the return value will be <see langword="null"/>.
	/// </returns>
	public ResourceDocument? this[int lcid]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _list.TryGetValue(lcid, out var result) ? result : null;
	}


	/// <summary>
	/// Indicates the event that is triggered when the key cannot be found.
	/// </summary>
	public event Handler? KeyNotFound;


	/// <summary>
	/// Add the specified resource document into the collection.
	/// </summary>
	/// <param name="element">The resource document instance.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Add(ResourceDocument element) => _list.Add(element.Lcid, element);

	/// <summary>
	/// To remove the specified resource document from the current collection via the LCID value.
	/// </summary>
	/// <param name="lcid">The resource document whose LCID is the current argument value specified.</param>
	/// <returns>Indicates whether the removing operation is successful.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Remove(int lcid) => _list.Remove(lcid);

	/// <summary>
	/// Determine whether the collection contains the resource document whose LCID is the specified one.
	/// </summary>
	/// <param name="lcid">The LCID to determine.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Contains(int lcid) => _list.ContainsKey(lcid);

	/// <summary>
	/// Determine whether at least one dictionary can find the specified key.
	/// </summary>
	/// <param name="key">The key to be found.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool ContainsKey(string key) =>
		_list.Values.Any(dic => dic[key, ResourceDocumentIndexerMode.NullableReturn] is not null);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public IEnumerator<ResourceDocument> GetEnumerator() => _list.Values.GetEnumerator();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
