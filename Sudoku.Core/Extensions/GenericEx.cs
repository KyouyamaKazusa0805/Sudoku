using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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
	}
}
