using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System
{
	/// <summary>
	/// Provides extension methods on <see cref="IFormatProvider"/>.
	/// </summary>
	/// <seealso cref="IFormatProvider"/>
	public static class FormatProviderExtensions
	{
		/// <summary>
		/// To check whether the format provider has defined the format rule.
		/// If the rule is defined, this method will return the string representation
		/// according to the format rule.
		/// </summary>
		/// <param name="this">The format provider.</param>
		/// <param name="obj">The object.</param>
		/// <param name="format">The format string.</param>
		/// <param name="result">
		/// The result. If the format has been defined,
		/// this value won't be <see langword="null"/>.
		/// </param>
		/// <returns>The <see cref="bool"/> value indicating that.</returns>
		/// <remarks>
		/// You should use this as:
		/// <code>
		/// if (formatProvider.HasFormatted(this, format, out string? result))
		/// {
		///	    return result;
		/// }
		/// </code>
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool HasFormatted<TNotNull>(
			[NotNullWhen(true)] this IFormatProvider? @this, in TNotNull obj, string? format,
			[NotNullWhen(true)] out string? result)
			where TNotNull : notnull
		{
			if (@this?.GetFormat(obj.GetType()) is ICustomFormatter customFormatter)
			{
				result = customFormatter.Format(format, obj, @this);
				return true;
			}
			else
			{
				result = null;
				return false;
			}
		}
	}
}
