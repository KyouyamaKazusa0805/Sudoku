using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Sudoku.Data.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="Type"/>.
	/// </summary>
	/// <seealso cref="Type"/>
	[DebuggerStepThrough]
	public static class TypeEx
	{
		/// <summary>
		/// To check whether the specified type has marked the specified attribute.
		/// </summary>
		/// <typeparam name="TAttribute">The type of the attribute.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The type.</param>
		/// <param name="inherit">
		/// <see langword="true"/> to search this member's inheritance chain
		/// to find the attributes; otherwise, <see langword="false"/>.
		/// This parameter is ignored for properties and events.
		/// </param>
		/// <param name="attributes">
		/// (<see langword="out"/> parameter) All attributes found.
		/// </param>
		/// <returns>A <see cref="bool"/> indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool HasMarkedAttribute<TAttribute>(
			this Type @this, bool inherit, [NotNullWhen(true)] out IEnumerable<TAttribute>? attributes)
			where TAttribute : Attribute
		{
			var temp = @this.GetCustomAttributes(inherit).OfType<TAttribute>();
			if (temp.Any())
			{
				attributes = temp;
				return true;
			}
			else
			{
				attributes = null;
				return false;
			}
		}

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
