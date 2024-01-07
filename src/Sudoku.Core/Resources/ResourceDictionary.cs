namespace Sudoku.Resources;

/// <summary>
/// Represents a easy way to fetch resource values using the specified key and culture information.
/// </summary>
public static class ResourceDictionary
{
	/// <summary>
	/// Indicates the resource manager reflection binding flags.
	/// </summary>
	private const BindingFlags DefaultBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;


	/// <summary>
	/// Indicates the default culture.
	/// </summary>
	public static readonly CultureInfo DefaultCulture = new(1033);

	/// <summary>
	/// The internal resource managers.
	/// </summary>
	private static readonly Dictionary<Assembly, ResourceManager> ResourceManagers = [];


	/// <summary>
	/// Register a new resource manager for the current assembly calling this method.
	/// </summary>
	/// <typeparam name="T">
	/// <para>The type of the resource manager provider.</para>
	/// <para>
	/// This type should point to a generated type, bound with your resource dictionary manifest file (*.resx), named like the file name of it.
	/// For example, if you create a resource dictionary manifest file called <c>Resource.resx</c>, a generated type will be named <c>Resource</c>.
	/// You should pass in this type (<c>Resource</c> here) as type argument to this method.
	/// </para>
	/// </typeparam>
	/// <exception cref="MissingResourceManagerException">Throws when the current calling assembly doesn't contain any resource manager.</exception>
	public static void RegisterResourceManager<T>() where T : class
	{
		var assembly = Assembly.GetCallingAssembly();
		var manager = typeof(T).GetProperty("ResourceManager", DefaultBindingFlags) switch
		{
			{ } pi => (ResourceManager)pi.GetValue(null)!,
			_ => null
		};
		ResourceManagers.Add(assembly, manager ?? throw new MissingResourceManagerException(assembly));
	}

	/// <summary>
	/// Try to get resource via the key, or throw a <see cref="TargetResourceNotFoundException"/> if resource is not found.
	/// </summary>
	/// <inheritdoc cref="GetOrNull(string, CultureInfo?, Assembly?)"/>
	/// <exception cref="TargetResourceNotFoundException">Throws when the specified resource is not found.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Get(string resourceKey, CultureInfo? culture = null, Assembly? assembly = null)
		=> GetOrNull(resourceKey, culture, assembly) ?? throw new TargetResourceNotFoundException(assembly, resourceKey, culture);

	/// <summary>
	/// Try to get resource via the key, or return <see langword="null"/> if failed.
	/// </summary>
	/// <param name="resourceKey">The resource key.</param>
	/// <param name="culture">The culture.</param>
	/// <param name="assembly">The assembly storing the resource dictionaries.</param>
	/// <returns>The result string result.</returns>
	/// <exception cref="MissingResourceManagerException">Throws when the resource manager object is missing.</exception>
	public static string? GetOrNull(string resourceKey, CultureInfo? culture = null, Assembly? assembly = null)
	{
		if (assembly is not null)
		{
			foreach (var (a, m) in ResourceManagers)
			{
				if (a == assembly)
				{
					return (culture is null ? m.GetString(resourceKey) : m.GetString(resourceKey, culture)) ?? m.GetString(resourceKey, DefaultCulture);
				}
			}

			throw new MissingResourceManagerException(assembly);
		}

		foreach (var m in ResourceManagers.Values)
		{
			if (((culture is null ? m.GetString(resourceKey) : m.GetString(resourceKey, culture)) ?? m.GetString(resourceKey, DefaultCulture)) is { } result)
			{
				return result;
			}
		}

		return null;
	}
}
