using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Sudoku.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="Type"/>.
	/// </summary>
	/// <seealso cref="Type"/>
	[DebuggerStepThrough]
	public static class TypeEx
	{
		/// <summary>
		/// Determines whether the current <see cref="Type"/> derives from the specified <see cref="Type"/>.
		/// </summary>
		/// <typeparam name="T">The type you want to determine.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The type.</param>
		/// <returns>
		/// <see langword="true"/> if the current <see cref="Type"/> derives from <typeparamref name="T"/>;
		/// otherwise, <see langword="false"/>. This method also returns <see langword="false"/> if
		/// <typeparamref name="T"/> and the current <see cref="Type"/> are equal.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsSubclassOf<T>(this Type @this) where T : class? => @this.IsSubclassOf(typeof(T));

		/// <summary>
		/// Indicates whether two types are same.
		/// </summary>
		/// <typeparam name="T1">The type 1.</typeparam>
		/// <typeparam name="T2">The type 2.</typeparam>
		/// <returns>A <see cref="bool"/> value.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TypeEquals<T1, T2>() => typeof(T1) == typeof(T2);
	}
}
