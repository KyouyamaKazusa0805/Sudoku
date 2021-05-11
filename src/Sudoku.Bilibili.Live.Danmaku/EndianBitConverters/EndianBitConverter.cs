using System;
using System.Runtime.CompilerServices;

namespace Sudoku.Bilibili.Live.Danmaku.EndianBitConverters
{
	/// <summary>
	/// A BitConverter with a specific endianness that converts base data types to an array of bytes,
	/// and an array of bytes to base data types, regardless of
	/// machine architecture. Access the little-endian and
	/// big-endian converters with their respective properties.
	/// </summary>
	/// <remarks>
	/// The EndianBitConverter implementations provide the same interface as <see cref="BitConverter"/>,
	/// but exclude those methods which perform the same on both big-endian and little-endian machines
	/// (such as <see cref="BitConverter.ToString(byte[])"/>).
	/// However, <see cref="GetBytes(bool)"/> is included for consistency.
	/// </remarks>
	public abstract class EndianBitConverter
	{
		/// <summary>
		/// Get an instance of a <see cref="LittleEndianBitConverter"/>,
		/// a <see cref="BitConverter"/> which performs all conversions in little-endian format regardless of
		/// machine architecture.
		/// </summary>
		public static EndianBitConverter LittleEndian { get; } = new LittleEndianBitConverter();

		/// <summary>
		/// Get an instance of a <see cref="BigEndianBitConverter"/>, a BitConverter which performs all conversions in big-endian format regardless of
		/// machine architecture.
		/// </summary>
		public static EndianBitConverter BigEndian { get; } = new BigEndianBitConverter();

		/// <summary>
		/// Indicates the byte order ("endianness") in which data should be converted.
		/// </summary>
		public abstract bool IsLittleEndian { get; }

		/// <summary>
		/// Returns the specified Boolean value as a byte array.
		/// </summary>
		/// <param name="value">A Boolean value.</param>
		/// <returns>A byte array with length 1.</returns>
		/// <remarks>You can convert a byte array back to a <see cref="Boolean"/> value by calling the <see cref="ToBoolean(byte[], int)"/> method.</remarks>
		public byte[] GetBytes(bool value) => new[] { value ? (byte)1 : (byte)0 };

		/// <summary>
		/// Returns the specified Unicode character value as an array of bytes.
		/// </summary>
		/// <param name="value">A character to convert.</param>
		/// <returns>An array of bytes with length 2.</returns>
		/// <remarks>
		/// You can convert a byte array back to a <see cref="char"/> value
		/// by calling the <see cref="ToChar(byte[], int)"/> method.
		/// </remarks>
		public byte[] GetBytes(char value) => GetBytes((short)value);

		/// <summary>
		/// Returns the specified double-precision floating point value as an array of bytes.
		/// </summary>
		/// <param name="value">The number to convert.</param>
		/// <returns>An array of bytes with length 8.</returns>
		/// <remarks>
		/// You can convert a byte array back to a <see cref="double"/> value
		/// by calling the <see cref="ToDouble(byte[], int)"/> method.
		/// </remarks>
		public byte[] GetBytes(double value) => GetBytes(BitConverter.DoubleToInt64Bits(value));

		/// <summary>
		/// Returns the specified 16-bit signed integer value as an array of bytes.
		/// </summary>
		/// <param name="value">The number to convert.</param>
		/// <returns>An array of bytes with length 2.</returns>
		/// <remarks>
		/// You can convert a byte array back to a <see cref="short"/>
		/// value by calling the <see cref="ToInt16(byte[], int)"/> method.
		/// </remarks>
		public abstract byte[] GetBytes(short value);

		/// <summary>
		/// Returns the specified 32-bit signed integer value as an array of bytes.
		/// </summary>
		/// <param name="value">The number to convert.</param>
		/// <returns>An array of bytes with length 4.</returns>
		/// <remarks>You can convert a byte array back to a <see cref="int"/>
		/// value by calling the <see cref="ToInt32(byte[], int)"/> method.
		/// </remarks>
		public abstract byte[] GetBytes(int value);

		/// <summary>
		/// Returns the specified 64-bit signed integer value as an array of bytes.
		/// </summary>
		/// <param name="value">The number to convert.</param>
		/// <returns>An array of bytes with length 8.</returns>
		/// <remarks>
		/// You can convert a byte array back to a <see cref="long"/> value
		/// by calling the <see cref="ToInt64(byte[], int)"/> method.
		/// </remarks>
		public abstract byte[] GetBytes(long value);

		/// <summary>
		/// Returns the specified single-precision floating point value as an array of bytes.
		/// </summary>
		/// <param name="value">The number to convert.</param>
		/// <returns>An array of bytes with length 4.</returns>
		/// <remarks>
		/// You can convert a byte array back to a <see cref="float"/> value
		/// by calling the <see cref="ToSingle(byte[], int)"/> method.
		/// </remarks>
		public byte[] GetBytes(float value) => GetBytes((int)new SingleConverter(value).GetIntValue());

		/// <summary>
		/// Returns the specified 16-bit unsigned integer value as an array of bytes.
		/// </summary>
		/// <param name="value">The number to convert. </param>
		/// <returns>An array of bytes with length 2.</returns>
		/// <remarks>
		/// You can convert a byte array back to a <see cref="ushort"/> value
		/// by calling the <see cref="ToUInt16(byte[], int)"/> method.
		/// </remarks>
		public byte[] GetBytes(ushort value) => GetBytes((short)value);

		/// <summary>
		/// Returns the specified 32-bit unsigned integer value as an array of bytes.
		/// </summary>
		/// <param name="value">The number to convert.</param>
		/// <returns>An array of bytes with length 4.</returns>
		/// <remarks>
		/// You can convert a byte array back to a <see cref="uint"/>
		/// value by calling the <see cref="ToUInt32(byte[], int)"/> method.
		/// </remarks>
		public byte[] GetBytes(uint value) => GetBytes((int)value);

		/// <summary>
		/// Returns the specified 64-bit unsigned integer value as an array of bytes.
		/// </summary>
		/// <param name="value">The number to convert.</param>
		/// <returns>An array of bytes with length 8.</returns>
		/// <remarks>
		/// You can convert a byte array back to a <see cref="ulong"/>
		/// value by calling the <see cref="ToUInt64(byte[], int)"/> method.
		/// </remarks>
		public byte[] GetBytes(ulong value) => GetBytes((long)value);

		/// <summary>
		/// Returns a Boolean value converted from the byte at a specified position in a byte array.
		/// </summary>
		/// <param name="value">A byte array.</param>
		/// <param name="startIndex">The index of the byte within <paramref name="value"/>.</param>
		/// <returns>
		/// true if the byte at <paramref name="startIndex"/> in <paramref name="value"/> is nonzero; otherwise, false.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="startIndex"/> is less than zero or greater than the length of <paramref name="value"/> minus 1.
		/// </exception>
		public bool ToBoolean(byte[] value, int startIndex)
		{
			CheckArguments(value, startIndex, 1);

			return value[startIndex] != 0;
		}

		/// <summary>
		/// Returns a Unicode character converted from two bytes at a specified position in a byte array.
		/// </summary>
		/// <param name="value">An array.</param>
		/// <param name="startIndex">The starting position within <paramref name="value"/>.</param>
		/// <returns>A character formed by two bytes beginning at <paramref name="startIndex"/>.</returns>
		/// <remarks>
		/// The ToChar method converts the bytes from index <paramref name="startIndex"/> to <paramref name="startIndex"/> + 1 to a <see cref="Char"/> value.
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="startIndex"/> is less than zero or greater than the length of <paramref name="value"/> minus 2.
		/// </exception>
		public char ToChar(byte[] value, int startIndex) => (char)ToInt16(value, startIndex);

		/// <summary>
		/// Returns a double-precision floating point number converted from eight bytes at a specified position in a byte array.
		/// </summary>
		/// <param name="value">An array of bytes.</param>
		/// <param name="startIndex">The starting position within <paramref name="value"/>.</param>
		/// <returns>A double precision floating point number formed by eight bytes beginning at <paramref name="startIndex"/>.</returns>
		/// <remarks>
		/// The ToDouble method converts the bytes from index <paramref name="startIndex"/> to <paramref name="startIndex"/> + 7 to a <see cref="Double"/>
		/// value.
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="startIndex"/> is less than zero or greater than the length of <paramref name="value"/> minus 8.
		/// </exception>
		public double ToDouble(byte[] value, int startIndex) =>
			BitConverter.Int64BitsToDouble(ToInt64(value, startIndex));

		/// <summary>
		/// Returns a 16-bit signed integer converted from two bytes at a specified position in a byte array.
		/// </summary>
		/// <param name="value">An array of bytes.</param>
		/// <param name="startIndex">The starting position within <paramref name="value"/>.</param>
		/// <returns>A 16-bit signed integer formed by two bytes beginning at <paramref name="startIndex"/>.</returns>
		/// <remarks>
		/// The ToInt16 method converts the bytes from index <paramref name="startIndex"/> to <paramref name="startIndex"/> + 1 to an <see cref="Int16"/>
		/// value.
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="startIndex"/> is less than zero or greater than the length of <paramref name="value"/> minus 2.
		/// </exception>
		public abstract short ToInt16(byte[] value, int startIndex);

		/// <summary>
		/// Returns a 32-bit signed integer converted from four bytes at a specified position in a byte array.
		/// </summary>
		/// <param name="value">An array of bytes. </param>
		/// <param name="startIndex">The starting position within <paramref name="value"/>.</param>
		/// <returns>A 32-bit signed integer formed by four bytes beginning at <paramref name="startIndex"/>.</returns>
		/// <remarks>
		/// The ToInt32 method converts the bytes from index <paramref name="startIndex"/> to <paramref name="startIndex"/> + 3 to an <see cref="Int32"/>
		/// value.
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="startIndex"/> is less than zero or greater than the length of <paramref name="value"/> minus 4.
		/// </exception>
		public abstract int ToInt32(byte[] value, int startIndex);

		/// <summary>
		/// Returns a 64-bit signed integer converted from eight bytes at a specified position in a byte array.
		/// </summary>
		/// <param name="value">An array of bytes.</param>
		/// <param name="startIndex">The starting position within <paramref name="value"/>.</param>
		/// <returns>A 64-bit signed integer formed by eight bytes beginning at <paramref name="startIndex"/>.</returns>
		/// <remarks>
		/// The ToInt64 method converts the bytes from index <paramref name="startIndex"/> to <paramref name="startIndex"/> + 7 to an <see cref="Int64"/>
		/// value.
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="startIndex"/> is less than zero or greater than the length of <paramref name="value"/> minus 8.
		/// </exception>
		public abstract long ToInt64(byte[] value, int startIndex);

		/// <summary>
		/// Returns a single-precision floating point number converted from four bytes at a specified position in a byte array.
		/// </summary>
		/// <param name="value">An array of bytes.</param>
		/// <param name="startIndex">The starting position within <paramref name="value"/>.</param>
		/// <returns>A single-precision floating point number formed by four bytes beginning at <paramref name="startIndex"/>.</returns>
		/// <remarks>
		/// The ToSingle method converts the bytes from index <paramref name="startIndex"/> to <paramref name="startIndex"/> + 3 to a <see cref="Single"/>
		/// value.
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="startIndex"/> is less than zero or greater than the length of <paramref name="value"/> minus 4.
		/// </exception>
		public float ToSingle(byte[] value, int startIndex) =>
			new SingleConverter(ToInt32(value, startIndex)).GetFloatValue();

		/// <summary>
		/// Returns a 16-bit unsigned integer converted from two bytes at a specified position in a byte array.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="startIndex">The starting position within <paramref name="value"/>.</param>
		/// <returns>A 16-bit unsigned integer formed by two bytes beginning at <paramref name="startIndex"/>.</returns>
		/// <remarks>
		/// The ToUInt16 method converts the bytes from index <paramref name="startIndex"/> to <paramref name="startIndex"/> + 1 to a <see cref="UInt16"/>
		/// value.
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="startIndex"/> is less than zero or greater than the length of <paramref name="value"/> minus 2.
		/// </exception>
		public ushort ToUInt16(byte[] value, int startIndex) => (ushort)ToInt16(value, startIndex);

		/// <summary>
		/// Returns a 32-bit unsigned integer converted from four bytes at a specified position in a byte array.
		/// </summary>
		/// <param name="value">An array of bytes. </param>
		/// <param name="startIndex">The starting position within <paramref name="value"/>.</param>
		/// <returns>A 32-bit unsigned integer formed by four bytes beginning at <paramref name="startIndex"/>.</returns>
		/// <remarks>
		/// The ToUInt32 method converts the bytes from index <paramref name="startIndex"/> to <paramref name="startIndex"/> + 3 to a <see cref="UInt32"/>
		/// value.
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="startIndex"/> is less than zero or greater than the length of <paramref name="value"/> minus 4.
		/// </exception>
		public uint ToUInt32(byte[] value, int startIndex) => (uint)ToInt32(value, startIndex);

		/// <summary>
		/// Returns a 64-bit unsigned integer converted from eight bytes at a specified position in a byte array.
		/// </summary>
		/// <param name="value">An array of bytes. </param>
		/// <param name="startIndex">The starting position within <paramref name="value"/>.</param>
		/// <returns>A 64-bit unsigned integer formed by the eight bytes beginning at <paramref name="startIndex"/>.</returns>
		/// <remarks>
		/// The ToUInt64 method converts the bytes from index <paramref name="startIndex"/> to <paramref name="startIndex"/> + 7 to a <see cref="UInt64"/>
		/// value.
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="startIndex"/> is less than zero or greater than the length of <paramref name="value"/> minus 8.
		/// </exception>
		public ulong ToUInt64(byte[] value, int startIndex) => (ulong)ToInt64(value, startIndex);


		/// <summary>
		/// To check arguments.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="startIndex">The start index.</param>
		/// <param name="byteLength">The byte length.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void CheckArguments(byte[] value, int startIndex, int byteLength)
		{
			// confirms startIndex is not negative or too far along the byte array
			if ((uint)startIndex > value.Length - byteLength)
			{
				throw new ArgumentOutOfRangeException(nameof(value));
			}
		}
	}
}
