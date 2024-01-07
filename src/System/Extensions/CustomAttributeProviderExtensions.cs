namespace System.Reflection;

/// <summary>
/// Provides with extension methods on <see cref="ICustomAttributeProvider"/>.
/// </summary>
/// <seealso cref="ICustomAttributeProvider"/>
public static class CustomAttributeProviderExtensions
{
	/// <summary>
	/// Gets the type arguments of the specified attribute type applied to the specified property.
	/// </summary>
	/// <typeparam name="T">The type of custom attribute provider.</typeparam>
	/// <param name="this">The instance.</param>
	/// <param name="genericAttributeType">The generic attribute type.</param>
	/// <returns>The types of the generic type arguments.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Type[] GetGenericAttributeTypeArguments<T>(this T @this, Type genericAttributeType) where T : class, ICustomAttributeProvider
		=> @this.GetCustomGenericAttribute(genericAttributeType)?.GetType().GenericTypeArguments ?? Type.EmptyTypes;

	/// <summary>
	/// <inheritdoc cref="Attribute.GetCustomAttribute(MemberInfo, Type)" path="/summary"/>
	/// </summary>
	/// <typeparam name="T">The type of custom attribute provider.</typeparam>
	/// <param name="this">The instance.</param>
	/// <param name="genericAttributeType">The generic attribute type.</param>
	/// <returns>
	/// <inheritdoc cref="Attribute.GetCustomAttribute(MemberInfo, Type)" path="/returns"/>
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Attribute? GetCustomGenericAttribute<T>(this T @this, Type genericAttributeType) where T : class, ICustomAttributeProvider
		=> genericAttributeType switch
		{
			{ IsGenericType: true, FullName: { } genericTypeName }
				=> (
					from a in @this.GetAttributesCore()
					where a.GetType() is { IsGenericType: var g, FullName: { } f } && g && genericTypeName.IndexOfBacktick() == f.IndexOfBacktick()
					select a
				) is [var attribute] ? attribute : null,
			_ => null
		};

	/// <summary>
	/// <inheritdoc cref="Attribute.GetCustomAttributes(MemberInfo, Type)" path="/summary"/>
	/// </summary>
	/// <typeparam name="T">The type of custom attribute provider.</typeparam>
	/// <param name="this">The instance.</param>
	/// <param name="genericAttributeType">The generic attribute type.</param>
	/// <returns>
	/// <inheritdoc cref="Attribute.GetCustomAttributes(MemberInfo, Type)" path="/returns"/>
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Attribute[] GetCustomGenericAttributes<T>(this T @this, Type genericAttributeType) where T : class, ICustomAttributeProvider
		=> genericAttributeType switch
		{
			{ IsGenericType: true, FullName: { } genericTypeName }
				=>
				from a in @this.GetAttributesCore()
				where a.GetType() is { IsGenericType: var g, FullName: { } f } && g && genericTypeName.IndexOfBacktick() == f.IndexOfBacktick()
				select a,
			_ => []
		};

	/// <summary>
	/// Get the index of the text of the back tick.
	/// </summary>
	/// <param name="this">The string text.</param>
	/// <returns>The index of the backtick in the string.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int IndexOfBacktick(this string @this) => @this.IndexOf('`');

	/// <summary>
	/// Get custom attributes.
	/// </summary>
	/// <typeparam name="T">The type of the provider.</typeparam>
	/// <param name="this">The custom attribute provider.</param>
	/// <returns>The attributes.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static Attribute[] GetAttributesCore<T>(this T @this) where T : class, ICustomAttributeProvider
		=> @this switch
		{
			MemberInfo m => (Attribute[])m.GetCustomAttributes(),
			Assembly a => (Attribute[])a.GetCustomAttributes(),
			_ => (@this.GetCustomAttributes(true) as Attribute[])!
		};
}
