using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Sudoku.Extensions
{
	/// <summary>
	/// Provides extension methods on generic instance.
	/// </summary>
	[DebuggerStepThrough]
	public static class GenericEx
	{
		/// <summary>
		/// Returns a <see cref="string"/> that represents the current object
		/// though the object is <see langword="null"/>. This method will never throw.
		/// </summary>
		/// <typeparam name="T">The type of this instance.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The object.</param>
		/// <returns>
		/// A <see cref="string"/> represents the current object.
		/// If the current object is <see langword="null"/>, this value will be
		/// <see cref="string.Empty"/>.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string NullableToString<T>([MaybeNull] this T @this) => @this.NullableToString(string.Empty);

		/// <summary>
		/// Returns a <see cref="string"/> that represents the current object
		/// though the object is <see langword="null"/>. This method will never throw.
		/// </summary>
		/// <typeparam name="T">The type of this instance.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The object.</param>
		/// <param name="defaultValue">
		/// The default return value when the current object is <see langword="null"/>.
		/// </param>
		/// <returns>
		/// A <see cref="string"/> represents the current object.
		/// If the current object is <see langword="null"/>, this value will be
		/// <paramref name="defaultValue"/>.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string NullableToString<T>([MaybeNull] this T @this, string defaultValue) =>
			@this?.ToString() ?? defaultValue;

		/// <summary>
		/// To check whether the specified instance has marked the specified attribute or not.
		/// </summary>
		/// <typeparam name="T">
		/// The type of the instance. This instance should not be <see langword="null"/>.
		/// </typeparam>
		/// <typeparam name="TAttribute">The type of the attribute.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The instance.</param>
		/// <param name="attributes">(<see langword="out"/> parameter) The attribute instances.</param>
		/// <param name="inherit">The parameter used for <see cref="MemberInfo.GetCustomAttributes(bool)"/>.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		/// <seealso cref="MemberInfo.GetCustomAttributes(bool)"/>
		public static bool HasMarked<T, TAttribute>(
			[NotNull] this T @this, [NotNullWhen(true)] out IEnumerable<TAttribute>? attributes, bool inherit = false)
			where T : notnull
			where TAttribute : Attribute
		{
			var temp = @this.GetType().GetCustomAttributes(inherit).OfType<TAttribute>();
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
	}
}
