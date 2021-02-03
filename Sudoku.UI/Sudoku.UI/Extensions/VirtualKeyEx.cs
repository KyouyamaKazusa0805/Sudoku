using System.Extensions;
using System.Runtime.CompilerServices;
using Microsoft.UI.Input;
using Windows.System;
using Windows.UI.Core;

namespace Sudoku.UI.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="VirtualKey"/>.
	/// </summary>
	/// <seealso cref="VirtualKey"/>
	public static class VirtualKeyEx
	{
		/// <summary>
		/// Determine whether the specified <see cref="VirtualKey"/> instance is a digit key.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The key to check.</param>
		/// <param name="anchor">
		/// <para>
		/// (<see langword="out"/> parameter) The anchor key result. The parameter is an <see langword="out"/>
		/// parameter, which means the value is decided in the method. The result value can be:
		/// <list type="table">
		/// <item>
		/// <term><see cref="VirtualKey.Number0"/></term>
		/// <description>
		/// If the current instance is between <see cref="VirtualKey.Number0"/>
		/// and <see cref="VirtualKey.Number9"/>.
		/// </description>
		/// </item>
		/// <item>
		/// <term><see cref="VirtualKey.NumberPad0"/></term>
		/// <description>
		/// If the current instance is between <see cref="VirtualKey.NumberPad0"/>
		/// and <see cref="VirtualKey.NumberPad9"/>.
		/// </description>
		/// </item>
		/// <item>
		/// <term><see langword="default"/></term>
		/// <description>Other cases.</description>
		/// </item>
		/// </list>
		/// </para>
		/// <para>
		/// This value can be used to confirm what range that the key is in, or get the offset of this range.
		/// For example:
		/// <code>
		/// if (key.IsDigit(out var anchor))
		/// {
		///	    int digitYouJustPressed = key - anchor;
		///	    // Code here...
		/// }
		/// </code>
		/// </para>
		/// <para>If you don't want to use this value, just use the discard syntax:
		/// <c><see langword="out _"/></c>.
		/// </para>
		/// </param>
		/// <returns>
		/// A <see cref="bool"/> result indicating that. If and only if the instance is between
		/// <see cref="VirtualKey.Number0"/> and <see cref="VirtualKey.Number9"/> or
		/// <see cref="VirtualKey.NumberPad0"/> and <see cref="VirtualKey.NumberPad9"/>, the return
		/// value will be <see langword="true"/>; otherwise, <see langword="false"/>.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsDigit(this VirtualKey @this, out VirtualKey anchor)
		{
			bool result;
			(result, anchor) = @this switch
			{
				>= VirtualKey.Number0 and <= VirtualKey.Number9 => (true, VirtualKey.Number0),
				VirtualKey.NumberPad0 and <= VirtualKey.NumberPad9 => (true, VirtualKey.NumberPad0),
				_ => (false, default)
			};

			return result;
		}

		/// <summary>
		/// Determine whether the current instance is down.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The modifier.</param>
		/// <returns>A <see cref="bool"/> result indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsDown(this VirtualKey @this)
		{
			var status = KeyboardInput.GetKeyStateForCurrentThread(@this);
			return status.Flags(CoreVirtualKeyStates.Down);
		}
	}
}
