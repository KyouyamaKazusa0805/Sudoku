using System.Diagnostics;

namespace Sudoku.Data.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="object"/>.
	/// </summary>
	/// <seealso cref="object"/>
	[DebuggerStepThrough]
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
		public static string NullableToString(this object? @this) =>
			@this.NullableToString(string.Empty);

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
		public static string NullableToString(this object? @this, string defaultValue) =>
			@this?.ToString() ?? defaultValue;
	}
}
