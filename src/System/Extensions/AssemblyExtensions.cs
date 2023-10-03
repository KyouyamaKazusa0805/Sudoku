using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System.Reflection;

/// <summary>
/// Provides with extension methods on <see cref="Assembly"/>.
/// </summary>
/// <seealso cref="Assembly"/>
public static class AssemblyExtensions
{
	/// <summary>
	/// Gets all possible types derived from an <see langword="interface"/> type,
	/// or a base <see langword="class"/> type, in the specified assembly.
	/// </summary>
	/// <param name="this">The assembly to be checked.</param>
	/// <param name="baseType">The type as the base type.</param>
	/// <returns>All possible derived types.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[RequiresUnreferencedCode("Types might be removed")]
	public static TypeArrayEnumerator GetDerivedTypes(this Assembly @this, Type baseType)
		=> new(from type in @this.GetTypes() where type.IsAssignableTo(baseType) select type);

	/// <inheritdoc cref="GetDerivedTypes(Assembly, Type)"/>
	/// <typeparam name="TBase">The type as the base type.</typeparam>
	/// <param name="this"><inheritdoc/></param>
	/// <returns><inheritdoc/></returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[RequiresUnreferencedCode("Types might be removed")]
	public static TypeArrayEnumerator GetDerivedTypes<TBase>(this Assembly @this)
		=> new(from type in @this.GetTypes() where type.IsAssignableTo(typeof(TBase)) select type);
}
