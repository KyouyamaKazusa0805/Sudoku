namespace System.Reflection;

/// <summary>
/// Provides with extension methods on <see cref="MemberInfo"/> instances.
/// </summary>
/// <seealso cref="MemberInfo"/>
public static class MemberInfoExtensions
{
	/// <summary>
	/// Gets the type arguments of the specified attribute type applied to the specified property.
	/// </summary>
	/// <typeparam name="T">The type of the member information.</typeparam>
	/// <param name="this">The <see cref="MemberInfo"/> instance.</param>
	/// <param name="genericAttributeType">The generic attribute type.</param>
	/// <returns>The types of the generic type arguments.</returns>
	public static Type[] GetGenericAttributeTypeArguments<T>(this T @this, Type genericAttributeType) where T : MemberInfo
		=> @this.GetCustomGenericAttribute(genericAttributeType)?.GetType().GenericTypeArguments ?? Array.Empty<Type>();

	/// <summary>
	/// <inheritdoc cref="Attribute.GetCustomAttribute(MemberInfo, Type)" path="/summary"/>
	/// </summary>
	/// <typeparam name="T">The type of the member information.</typeparam>
	/// <param name="this">The <see cref="MemberInfo"/> instance.</param>
	/// <param name="genericAttributeType">The generic attribute type.</param>
	/// <returns>
	/// <inheritdoc cref="Attribute.GetCustomAttribute(MemberInfo, Type)" path="/returns"/>
	/// </returns>
	public static Attribute? GetCustomGenericAttribute<T>(this T @this, Type genericAttributeType) where T : MemberInfo
	{
		var customAttributes = @this.GetCustomAttributes();
		return genericAttributeType switch
		{
			{ IsGenericType: true, FullName: { } genericTypeName }
				=> (
					from a in customAttributes
					where a.GetType() is { IsGenericType: var g, FullName: { } f } && g && p(genericTypeName) == p(f)
					select a
				).ToArray() switch
				{
					[var attribute] => attribute,
					_ => null
				},
			_ => null
		};


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static int p(string s) => s.IndexOf('`');
	}
}
