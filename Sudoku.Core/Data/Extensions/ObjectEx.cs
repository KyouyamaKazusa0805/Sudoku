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
		/// though the object is <c>null</c>.
		/// </summary>
		/// <param name="this">The object.</param>
		/// <returns>
		/// A <see cref="string"/> represents the current object.
		/// If the current object is <c>null</c>, this value will be
		/// <see cref="string.Empty"/>. Therefore, this method will never throw.
		/// </returns>
		public static string NullableToString(this object? @this) =>
			@this.NullableToString(string.Empty);

		/// <summary>
		/// Returns a <see cref="string"/> that represents the current object
		/// though the object is <c>null</c>.
		/// </summary>
		/// <param name="this">The object.</param>
		/// <param name="defaultValue">
		/// The default return value when the current object is <c>null</c>.
		/// </param>
		/// <returns>
		/// A <see cref="string"/> represents the current object.
		/// If the current object is <c>null</c>, this value will be
		/// <paramref name="defaultValue"/>. Therefore, this method will never throw.
		/// </returns>
		public static string NullableToString(this object? @this, string defaultValue) =>
			@this?.ToString() ?? defaultValue;
	}
}
