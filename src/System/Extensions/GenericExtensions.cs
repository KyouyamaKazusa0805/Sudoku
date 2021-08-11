using System.Runtime.CompilerServices;

namespace System
{
	/// <summary>
	/// Provides extension methods on generic type.
	/// </summary>
	public static class GenericExtensions
	{
		/// <summary>
		/// Get the string representation of the instance, with the trailing space after the result string value.
		/// </summary>
		/// <typeparam name="TNotNull">The type of the instance.</typeparam>
		/// <param name="this">The instance.</param>
		/// <returns>The result value.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string ToStringWithTrailingSpace<TNotNull>(this TNotNull @this) where TNotNull : notnull =>
			$"{@this.ToString()} ";
	}
}
