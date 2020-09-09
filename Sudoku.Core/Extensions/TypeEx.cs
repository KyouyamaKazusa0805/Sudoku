using System;
using System.Diagnostics;
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
		/// Indicates whether two types are same.
		/// </summary>
		/// <typeparam name="T1">The type 1.</typeparam>
		/// <typeparam name="T2">The type 2.</typeparam>
		/// <returns>A <see cref="bool"/> value.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TypeEquals<T1, T2>() => typeof(T1) == typeof(T2);
	}
}
