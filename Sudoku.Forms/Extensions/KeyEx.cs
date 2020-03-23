using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Sudoku.Forms.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="Key"/>.
	/// </summary>
	/// <seealso cref="Key"/>
	[DebuggerStepThrough]
	public static class KeyEx
	{
		/// <summary>
		/// Check whether the specified key is a digit key.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The key.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsDigit(this Key @this) =>
			@this >= Key.D0 && @this <= Key.D9 || @this >= Key.NumPad0 && @this <= Key.NumPad9;

		/// <summary>
		/// Check whether the specified key is a digit key in number pad.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The key.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNumPadDigit(this Key @this) =>
			@this >= Key.NumPad0 && @this <= Key.NumPad9;

		/// <summary>
		/// Check whether the specified key is a digit key above those alphabets
		/// in keyboard.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The key.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsDigitUpsideAlphabets(this Key @this) =>
			@this >= Key.D0 && @this <= Key.D9;

		/// <summary>
		/// Check whether the specified key is a letter.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The key.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAlphabet(this Key @this) =>
			@this >= Key.A && @this <= Key.Z;
	}
}
