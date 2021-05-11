using System;
using System.Runtime.InteropServices;

namespace Sudoku.Bilibili.Live.Danmaku.EndianBitConverters
{
	/// <summary>
	/// Converts between Single (float) and Int32 (int), as <see cref="BitConverter"/>
	/// doesn't have a method to do this in all .NET versions.
	/// A union is used instead of an unsafe pointer cast
	/// so we don't have to worry about the trusted environment implications.
	/// </summary>
	[StructLayout(LayoutKind.Explicit)]
	internal readonly struct SingleConverter
	{
		/// <summary>
		/// Maps to an <see cref="int"/> value.
		/// </summary>
		[FieldOffset(0)]
		private readonly int _intValue;

		/// <summary>
		/// Maps to an <see cref="float"/> value.
		/// </summary>
		[FieldOffset(0)]
		private readonly float _floatValue;


		/// <summary>
		/// Initializes an instance with the specified <see cref="int"/> value.
		/// </summary>
		/// <param name="value">The value.</param>
		public SingleConverter(int value) : this() => _intValue = value;

		/// <summary>
		/// Initializes an instance with the specified <see cref="float"/> value.
		/// </summary>
		/// <param name="value">The value.</param>
		public SingleConverter(float value) : this() => _floatValue = value;


		/// <summary>
		/// Gets the <see cref="int"/> value.
		/// </summary>
		/// <returns>The value.</returns>
		public int GetIntValue() => _intValue;

		/// <summary>
		/// Gets the <see cref="float"/> value.
		/// </summary>
		/// <returns>The value.</returns>
		public float GetFloatValue() => _floatValue;
	}
}
