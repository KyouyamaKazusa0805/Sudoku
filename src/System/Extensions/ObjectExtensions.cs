using System.Runtime.CompilerServices;
using static System.Linq.Expressions.Expression;

namespace System;

/// <summary>
/// Provides with extension methods on <see cref="object"/>.
/// </summary>
/// <seealso cref="object"/>
public static class ObjectExtensions
{
	/// <summary>
	/// Try to cast an object to the specified type.
	/// </summary>
	/// <param name="this">The object to be casted.</param>
	/// <param name="targetType">The type that the current instance will be converted into.</param>
	/// <returns>The converted result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static object DynamicCastTo(this object @this, Type targetType)
	{
		var dataParam = Parameter(typeof(object), nameof(@this));
		return Lambda(Block(Convert(Convert(dataParam, @this.GetType()), targetType)), dataParam).Compile().DynamicInvoke([@this])!;
	}
}
