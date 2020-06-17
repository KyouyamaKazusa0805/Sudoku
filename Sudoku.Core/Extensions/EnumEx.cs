using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Sudoku.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="Enum"/>.
	/// </summary>
	/// <seealso cref="Enum"/>
	[DebuggerStepThrough]
	public static class EnumEx
	{
		/// <summary>
		/// Get all enumeration fields.
		/// </summary>
		/// <typeparam name="TEnum">The type of enumeration type.</typeparam>
		/// <returns>The fields.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TEnum[] GetValues<TEnum>() where TEnum : struct, Enum => (TEnum[])Enum.GetValues(typeof(TEnum));
	}
}
