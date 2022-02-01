namespace Sudoku.Resources;

/// <summary>
/// Defines an external resource manager.
/// </summary>
public sealed class ResourceManager :
	IEquatable<ResourceManager>,
	IEqualityOperators<ResourceManager, ResourceManager>
{
	/// <summary>
	/// Indicates the external resource manager.
	/// </summary>
	public static readonly ResourceManager Shared = new();


	/// <summary>
	/// Initializes a <see cref="ResourceManager"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private ResourceManager()
	{
	}


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
			// Searching for external resources.
			foreach (var router in Routers.GetInvocations())
			{
				if (router(propertyName) is { } result)
				{
					return result;
				}
			}

			// The key cannot be found in all dictionaries. Just throw exceptions to report the wrong case.
			throw new KeyNotFoundException($"The specified key cannot be found: {propertyName}.");
		}
	}


	/// <summary>
	/// Defines the inner collection to store documents.
	/// </summary>
	public event ResourceRouter? Routers;


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] object? obj) =>
		obj is ResourceManager comparer && Equals(comparer);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals([NotNullWhen(true)] ResourceManager? other) =>
		other is not null && Routers == other.Routers;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => Routers?.GetHashCode() ?? 0;


	/// <summary>
	/// Determine whether two <see cref="ResourceManager"/>s are equal.
	/// </summary>
	/// <param name="left">The left-side instance to compare.</param>
	/// <param name="right">The right-side instance to compare.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool Equals(ResourceManager? left, ResourceManager? right) =>
		(Left: left, Right: right) switch
		{
			(Left: null, Right: null) => true,
			(Left: not null, Right: not null) => left.Equals(right),
			_ => false
		};


	/// <summary>
	/// Determine whether two <see cref="ResourceManager"/>s are equal.
	/// </summary>
	/// <param name="left">The left-side instance to compare.</param>
	/// <param name="right">The right-side instance to compare.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(ResourceManager? left, ResourceManager? right) =>
		Equals(left, right);

	/// <summary>
	/// Determine whether two <see cref="ResourceManager"/>s are not equal.
	/// </summary>
	/// <param name="left">The left-side instance to compare.</param>
	/// <param name="right">The right-side instance to compare.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(ResourceManager? left, ResourceManager? right) =>
		!Equals(left, right);
}
