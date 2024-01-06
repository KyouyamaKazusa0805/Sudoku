namespace Sudoku.Resources;

/// <summary>
/// Defines a resource reader.
/// </summary>
public interface IResourceReader
{
	/// <summary>
	/// Indicates the default culture.
	/// </summary>
	private static readonly CultureInfo DefaultCulture = new(1033);


	/// <summary>
	/// The resource manager.
	/// </summary>
	protected abstract ResourceManager ResourceManager { get; }


	/// <summary>
	/// Try to get the resource file.
	/// </summary>
	/// <param name="resourceKey">The resource key.</param>
	/// <param name="culture">The culture information.</param>
	/// <returns>The final resource text.</returns>
	/// <exception cref="TargetResourceNotFoundException">Throws when the target resource is not found.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sealed string GetResource(string resourceKey, CultureInfo? culture = null)
	{
		var manager = ResourceManager;
		if (manager.GetString(resourceKey, culture) is { } cultureSpecifiedResource)
		{
			return cultureSpecifiedResource;
		}

		if (manager.GetString(resourceKey, DefaultCulture) is { } cultureGenericResource)
		{
			return cultureGenericResource;
		}

		throw new TargetResourceNotFoundException(resourceKey.GetType().Assembly, resourceKey, culture);
	}
}
