using System.Runtime.CompilerServices;
using Windows.System;

namespace Sudoku.UI.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="VirtualKeyModifiers"/>.
	/// </summary>
	/// <seealso cref="VirtualKeyModifiers"/>
	public static class VirtualKeyModifiersEx
	{
		/// <summary>
		/// Determine whether the current instance is down.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The modifier.</param>
		/// <returns>A <see cref="bool"/> result indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsDown(this VirtualKeyModifiers @this) =>
		(
			@this switch
			{
				VirtualKeyModifiers.Control => VirtualKey.Control,
				VirtualKeyModifiers.Menu => VirtualKey.Menu,
				VirtualKeyModifiers.Shift => VirtualKey.Shift,
				VirtualKeyModifiers.Windows => VirtualKey.LeftWindows
			}
		).IsDown();
	}
}
