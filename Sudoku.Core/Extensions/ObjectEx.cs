using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Sudoku.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="object"/>.
	/// </summary>
	/// <seealso cref="object"/>
	//[DebuggerStepThrough]
	public static class ObjectEx
	{
		/// <summary>
		/// Returns a <see cref="string"/> that represents the current object
		/// though the object is <see langword="null"/>.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The object.</param>
		/// <returns>
		/// A <see cref="string"/> represents the current object.
		/// If the current object is <see langword="null"/>, this value will be
		/// <see cref="string.Empty"/>. Therefore, this method will never throw.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string NullableToString(this object? @this) => @this.NullableToString(string.Empty);

		/// <summary>
		/// Returns a <see cref="string"/> that represents the current object
		/// though the object is <see langword="null"/>.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The object.</param>
		/// <param name="defaultValue">
		/// The default return value when the current object is <see langword="null"/>.
		/// </param>
		/// <returns>
		/// A <see cref="string"/> represents the current object.
		/// If the current object is <see langword="null"/>, this value will be
		/// <paramref name="defaultValue"/>. Therefore, this method will never throw.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string NullableToString(this object? @this, string defaultValue) =>
			@this?.ToString() ?? defaultValue;

		/// <summary>
		/// To check whether the specified type has marked the specified attribute.
		/// </summary>
		/// <typeparam name="TAttribute">The type of the attribute.</typeparam>
		/// <param name="this">
		/// (<see langword="this"/> parameter) The instance. This instance will
		/// never be useful except to get its type.
		/// </param>
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
			this object @this, bool inherit, [NotNullWhen(true)] out IEnumerable<TAttribute>? attributes)
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
