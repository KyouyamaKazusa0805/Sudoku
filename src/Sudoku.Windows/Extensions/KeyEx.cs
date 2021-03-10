using System.Runtime.CompilerServices;
using System.Windows.Input;
using static System.Windows.Input.Key;

namespace Sudoku.Windows.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="Key"/>.
	/// </summary>
	/// <seealso cref="Key"/>
	public static class KeyEx
	{
		/// <summary>
		/// Check whether the specified key is a digit key.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The key.</param>
		/// <param name="fromZero">
		/// Indicates whether the checking is from <see cref="D0"/>
		/// or <see cref="NumPad0"/>.
		/// </param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsDigit(this Key @this, bool fromZero = true) =>
			@this >= (fromZero ? D0 : D1) && @this <= D9
			|| @this >= (fromZero ? NumPad0 : NumPad1) && @this <= NumPad9;

		/// <summary>
		/// Check whether the specified key is a digit key in number pad.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The key.</param>
		/// <param name="fromZero">
		/// Indicates whether the checking is from <see cref="D0"/>
		/// or <see cref="NumPad0"/>.
		/// </param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNumPadDigit(this Key @this, bool fromZero = true) =>
			@this >= (fromZero ? NumPad0 : NumPad1) && @this <= NumPad9;

		/// <summary>
		/// Check whether the specified key is a digit key above those alphabets
		/// in keyboard.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The key.</param>
		/// <param name="fromZero">
		/// Indicates whether the checking is from <see cref="D0"/>
		/// or <see cref="NumPad0"/>.
		/// </param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsDigitUpsideAlphabets(this Key @this, bool fromZero = true) =>
			@this >= (fromZero ? D0 : D1) && @this <= D9;

		/// <summary>
		/// Check whether the specified key is a letter.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The key.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAlphabet(this Key @this) => @this is >= A and <= Z;

		/// <summary>
		/// Check whether the specified key is a arrow key (up, down, left or right).
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The key.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsArrow(this Key @this) => @this is Up or Down or Left or Right;
	}
}
