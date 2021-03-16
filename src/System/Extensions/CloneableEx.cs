using System.Runtime.CompilerServices;

namespace System.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="ICloneable"/>.
	/// </summary>
	/// <seealso cref="ICloneable"/>
	public static class CloneableEx
	{
		/// <summary>
		/// Clone this object and try to cast to the specified type.
		/// If the type is invalid to cast, the return value will be <see langword="null"/>.
		/// </summary>
		/// <typeparam name="TClass">
		/// The type to cast. The type should be a <see langword="class"/> because the type
		/// to implement <see cref="ICloneable"/> should be a reference type.
		/// </typeparam>
		/// <param name="this">The object.</param>
		/// <returns>
		/// The cast result. If cast is valid, the value will be so valid; otherwise,
		/// <see langword="null"/>.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TClass? TryCloneAs<TClass>(this ICloneable @this) where TClass : class =>
			@this.Clone() as TClass;

		/// <summary>
		/// Clone this object and cast to the specified type no matter how.
		/// </summary>
		/// <typeparam name="TClass">
		/// The type to cast. The type should be a <see langword="class"/> because the type
		/// to implement <see cref="ICloneable"/> should be a reference type.
		/// </typeparam>
		/// <param name="this">The object.</param>
		/// <returns>The cast result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TClass CloneAs<TClass>(this ICloneable @this) where TClass : class => (TClass)@this.Clone();

		/// <summary>
		/// Try to cast the current instance to the generic cloneable type <see cref="ICloneable{T}"/>.
		/// </summary>
		/// <typeparam name="TClass">The type of the instance.</typeparam>
		/// <param name="this">The object.</param>
		/// <returns>The cast result. If cast is invalid, the return value will be <see langword="null"/>.</returns>
		/// <seealso cref="ICloneable{T}"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ICloneable<TClass>? Cast<TClass>(this ICloneable @this) where TClass : class =>
			@this as ICloneable<TClass>;
	}
}
