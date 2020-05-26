using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Sudoku.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="Array"/>.
	/// </summary>
	/// <seealso cref="Array"/>
	[DebuggerStepThrough]
	public static class ArrayEx
	{
		/// <summary>
		/// Determine whether the specified array contains the specified element.
		/// </summary>
		/// <typeparam name="T">The type of the element.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The array.</param>
		/// <param name="element">The element.</param>
		/// <returns>The <see cref="bool"/> value indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Contains<T>(this T[] @this, T element) => @this.IndexOf(element) != -1;

		/// <summary>
		/// The extension instance method for <see cref="Array.IndexOf{T}(T[], T)"/>.
		/// </summary>
		/// <typeparam name="T">The type of the element.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The array.</param>
		/// <param name="element">The element.</param>
		/// <returns>The <see cref="bool"/> value indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int IndexOf<T>(this T[] @this, T element) => Array.IndexOf(@this, element);
	}
}
