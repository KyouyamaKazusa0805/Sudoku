namespace Sudoku.Bilibili.Live.Danmaku.EndianBitConverters
{
	/// <summary>
	/// A little-endian BitConverter that converts base data types to an array of bytes,
	/// and an array of bytes to base data types. All conversions are in
	/// little-endian format regardless of machine architecture.
	/// </summary>
	internal class LittleEndianBitConverter : EndianBitConverter
	{
		/// <inheritdoc/>
		public override bool IsLittleEndian => true;

		/// <inheritdoc/>
		public override byte[] GetBytes(short value) => new[] { (byte)value, (byte)(value >> 8) };

		/// <inheritdoc/>
		public override byte[] GetBytes(int value) => new[]
		{
			(byte)value,
			(byte)(value >> 8),
			(byte)(value >> 16),
			(byte)(value >> 24)
		};

		/// <inheritdoc/>
		public override byte[] GetBytes(long value) => new[]
		{
			(byte)value, (byte)(value >> 8), (byte)(value >> 16), (byte)(value >> 24),
			(byte)(value >> 32), (byte)(value >> 40), (byte)(value >> 48), (byte)(value >> 56)
		};

		/// <inheritdoc/>
		public override short ToInt16(byte[] value, int startIndex)
		{
			CheckArguments(value, startIndex, sizeof(short));

			return (short)((value[startIndex]) | (value[startIndex + 1] << 8));
		}

		/// <inheritdoc/>
		public override int ToInt32(byte[] value, int startIndex)
		{
			CheckArguments(value, startIndex, sizeof(int));

			return (value[startIndex])
				| (value[startIndex + 1] << 8)
				| (value[startIndex + 2] << 16)
				| (value[startIndex + 3] << 24);
		}

		/// <inheritdoc/>
		public override long ToInt64(byte[] value, int startIndex)
		{
			CheckArguments(value, startIndex, sizeof(long));

			return (uint)(
				(value[startIndex])
				| (value[startIndex + 1] << 8)
				| (value[startIndex + 2] << 16)
				| (value[startIndex + 3] << 24)
			) | (
				(long)(
					(value[startIndex + 4])
					| (value[startIndex + 5] << 8)
					| (value[startIndex + 6] << 16)
					| (value[startIndex + 7] << 24)
				) << 32
			);
		}
	}
}
