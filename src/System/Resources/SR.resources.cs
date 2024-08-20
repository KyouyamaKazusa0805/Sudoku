namespace System.Resources;

public partial class SR
{
	/// <summary>
	/// The internal resource managers.
	/// </summary>
	private static readonly List<(Assembly Assembly, ResourceManager ResourceManager)> ResourceManagers = [];


	/// <summary>
	/// Represents the resource fetching rule. This method can control the fetching rule,
	/// especially for the cases using culture-switching.
	/// </summary>
	public static Func<ResourceManager, string, CultureInfo?, string?> ResourceFetchingHandler { get; set; } =
		static (m, k, c) => c is null ? m.GetString(k) ?? m.GetString(k, DefaultCulture) : m.GetString(k, c);


	/// <summary>
	/// Try to get error information (used by exception message, <see cref="Exception.Message"/> property) values,
	/// or throw a <see cref="ResourceNotFoundException"/> if resource is not found.
	/// </summary>
	/// <inheritdoc cref="TryGet(string, out string?, CultureInfo?, Assembly?)"/>
	/// <exception cref="ResourceNotFoundException">Throws when the specified resource is not found.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ExceptionMessage(string resourceKey, CultureInfo? culture = null, Assembly? assembly = null)
		=> TryGet(resourceKey.StartsWith("ErrorInfo_") ? resourceKey : $"ErrorInfo_{resourceKey}", out var resource, culture, assembly)
			? resource
			: throw new ResourceNotFoundException(assembly, resourceKey, culture);

	/// <summary>
	/// Try to get resource via the key, or throw a <see cref="ResourceNotFoundException"/> if resource is not found.
	/// </summary>
	/// <inheritdoc cref="TryGet(string, out string?, CultureInfo?, Assembly?)"/>
	/// <exception cref="ResourceNotFoundException">Throws when the specified resource is not found.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Get(string resourceKey, CultureInfo? culture = null, Assembly? assembly = null)
		=> TryGet(resourceKey, out var resource, culture, assembly)
			? resource
			: throw new ResourceNotFoundException(assembly, resourceKey, culture);

	/// <summary>
	/// Try to get resource via the key, or return <see langword="null"/> if failed.
	/// </summary>
	/// <param name="resourceKey">The resource key.</param>
	/// <param name="resource">Indicates the target resource.</param>
	/// <param name="culture">The culture.</param>
	/// <param name="assembly">The assembly storing the resource dictionaries.</param>
	/// <returns>The result string result.</returns>
	/// <exception cref="MissingResourceManagerException">Throws when the resource manager object is missing.</exception>
	public static bool TryGet(string resourceKey, [NotNullWhen(true)] out string? resource, CultureInfo? culture = null, Assembly? assembly = null)
	{
		if (assembly is not null)
		{
			foreach (var (a, m) in ResourceManagers)
			{
				if (a == assembly)
				{
					return (resource = ResourceFetchingHandler(m, resourceKey, culture)) is not null;
				}
			}
			throw new MissingResourceManagerException(assembly);
		}

		foreach (var m in from pair in ResourceManagers select pair.ResourceManager)
		{
			if (ResourceFetchingHandler(m, resourceKey, culture) is { } result)
			{
				resource = result;
				return true;
			}
		}
		resource = null;
		return false;
	}
}
