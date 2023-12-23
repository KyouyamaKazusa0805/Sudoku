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
		=> @this.GetCustomGenericAttribute(genericAttributeType)?.GetType().GenericTypeArguments ?? Type.EmptyTypes;

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
		return genericAttributeType switch
		{
			{ IsGenericType: true, FullName: { } genericTypeName }
				=> (
					from a in (Attribute[])@this.GetCustomAttributes()
					where a.GetType() is { IsGenericType: var g, FullName: { } f } && g && p(genericTypeName) == p(f)
					select a
				) is [var attribute] ? attribute : null,
			_ => null
		};


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static int p(string s) => s.IndexOf('`');
	}

	/// <summary>
	/// <inheritdoc cref="Attribute.GetCustomAttributes(MemberInfo, Type)" path="/summary"/>
	/// </summary>
	/// <typeparam name="T">The type of the member information.</typeparam>
	/// <param name="this">The <see cref="MemberInfo"/> instance.</param>
	/// <param name="genericAttributeType">The generic attribute type.</param>
	/// <returns>
	/// <inheritdoc cref="Attribute.GetCustomAttributes(MemberInfo, Type)" path="/returns"/>
	/// </returns>
	public static Attribute[] GetCustomGenericAttributes<T>(this T @this, Type genericAttributeType) where T : MemberInfo
	{
		return genericAttributeType switch
		{
			{ IsGenericType: true, FullName: { } genericTypeName }
				=>
				from a in (Attribute[])@this.GetCustomAttributes()
				where a.GetType() is { IsGenericType: var g, FullName: { } f } && g && p(genericTypeName) == p(f)
				select a,
			_ => []
		};


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static int p(string s) => s.IndexOf('`');
	}

	/// <summary>
	/// When overridden in a derived class, returns the <see langword="init"/> accessor for this property.
	/// </summary>
	/// <param name="this">The property.</param>
	/// <param name="nonPublic">
	/// Indicates whether the accessor should be returned if it is non-public.
	/// <see langword="true"/> if a non-public accessor is to be returned; otherwise, <see langword="false"/>.
	/// </param>
	/// <returns>
	/// This property's <see langword="init"/> method, or <see langword="null"/>, as shown in the following table.
	/// <list type="table">
	/// <listheader>
	/// <term>Value</term>
	/// <description>Condition</description>
	/// </listheader>
	/// <item>
	/// <term>The <see langword="init"/> method for this property</term>
	/// <description>
	/// The <see langword="init"/> accessor is public, or <paramref name="nonPublic"/> is <see langword="true"/>
	/// and the <see langword="init"/> accessor is non-public.
	/// </description>
	/// </item>
	/// <item>
	/// <term><see langword="null"/></term>
	/// <description>
	/// <paramref name="nonPublic"/> is <see langword="true"/>, but the property is read-only,
	/// or <paramref name="nonPublic"/> is <see langword="false"/> and the <see langword="init"/> accessor is non-public,
	/// or there is no <see langword="init"/> accessor.
	/// </description>
	/// </item>
	/// </list>
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static MethodInfo? GetInitMethod(this PropertyInfo @this, bool nonPublic)
	{
		return @this.GetSetMethod(nonPublic) switch
		{
			{ ReturnParameter: var r } i when Array.Exists(r.GetRequiredCustomModifiers(), match) => i,
			_ => null
		};


		static bool match(Type modreq) => modreq == typeof(IsExternalInit);
	}
}
