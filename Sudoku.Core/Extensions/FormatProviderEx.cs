using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Sudoku.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="IFormatProvider"/>.
	/// </summary>
	/// <seealso cref="IFormatProvider"/>
	[DebuggerStepThrough]
	public static class FormatProviderEx
	{
		/// <summary>
		/// To check whether the format provider has defined the format rule.
		/// If the rule is defined, this method will return the string representation
		/// according to the format rule.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The format provider.</param>
		/// <param name="obj">The object.</param>
		/// <param name="format">The format string.</param>
		/// <param name="result">
		/// (<see langword="out"/> parameter) The result. If the format has been defined,
		/// this value will not be <see langword="null"/>.
		/// </param>
		/// <returns>The <see cref="bool"/> value indicating that.</returns>
		/// <example>
		/// Using the method, you can simplify the check if the code is like:
		/// <code>
		/// if (formatProvider.HasFormatted(this, format, out string? result))<br/>
		/// {<br/>
		/// 	return result;<br/>
		/// }
		/// </code>
		/// </example>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool HasFormatted(
			this IFormatProvider? @this, object obj, string? format, [NotNullWhen(true)] out string? result)
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
