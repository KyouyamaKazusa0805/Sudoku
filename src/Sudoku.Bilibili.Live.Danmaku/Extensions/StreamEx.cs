using System;
using System.IO;
using System.Threading.Tasks;

namespace Sudoku.Bilibili.Live.Danmaku.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="Stream"/>.
	/// </summary>
	/// <seealso cref="Stream"/>
	public static class StreamEx
	{
		/// <summary>
		/// Read the buffer asynchronously.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <param name="buffer">The buffer.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="count">The count.</param>
		/// <returns>The task of the operation.</returns>
		/// <exception cref="ArgumentException">
		/// Throws when <paramref name="count"/> isn't a positive integer.
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// Throws when the offset, count and the buffer length has an invalid relation.
		/// </exception>
		/// <exception cref="ObjectDisposedException">Throws when the stream has already disposed.</exception>
		public static async Task ReadBAsync(this Stream stream, byte[] buffer, int offset, int count)
		{
			if (count <= 0)
			{
				throw new ArgumentException(
					"The specified argument is invalid: The value can't be 0.",
					nameof(count)
				);
			}

			if (offset + count > buffer.Length)
			{
				throw new InvalidOperationException(
					"Can't operate because the specified position is out of the valid range."
				);
			}

			int read = 0;
			while (read < count)
			{
				int available = await stream.ReadAsync(buffer.AsMemory(offset, count - read));
				if (available == 0)
				{
					throw new ObjectDisposedException(nameof(stream));
				}

				read += available;
				offset += available;
			}
		}
	}
}
