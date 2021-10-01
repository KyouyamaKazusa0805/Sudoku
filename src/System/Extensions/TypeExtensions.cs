namespace System;

/// <summary>
/// Provides extension method on <see cref="Type"/>.
/// </summary>
/// <seealso cref="Type"/>
public static class TypeExtensions
{
	/// <summary>
	/// Determine whether the type has implemented the specified typed interface.
	/// </summary>
	/// <typeparam name="TInterface">The type of the interface.</typeparam>
	/// <param name="this">The type to check.</param>
	/// <returns>The <see cref="bool"/> result.</returns>
	/// <exception cref="InvalidOperationException">
	/// Throws when the specified type argument isn't an <see langword="interface"/> type.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool HasImplemented<TInterface>(this Type @this) where TInterface : class =>
		typeof(TInterface).IsInterface
			? @this.GetInterfaces().OfType<TInterface>().Any()
			: throw new InvalidOperationException("The type argument must be an interface.");

	/// <summary>
	/// Determine whether the type contains a parameterless constructor.
	/// </summary>
	/// <param name="this">The type.</param>
	/// <param name="flags">
	/// The flags. The default value is
	/// <c><see cref="BindingFlags.Instance"/> | <see cref="BindingFlags.Public"/></c>.
	/// </param>
	/// <returns>The <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool ContainsParameterlessConstructor(
		this Type @this,
		BindingFlags flags = BindingFlags.Instance | BindingFlags.Public
	) => @this.GetConstructor(flags, Array.Empty<Type>()) is not null;
}
