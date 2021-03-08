using System.Buffers;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace System.Text
{
	partial struct ValueStringBuilder
	{
		/// <summary>
		/// To dispose the collection, all fields and properties will be cleared. In other words,
		/// this method is nearly equivalent to the code <c>this = default;</c>.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Dispose() => Dispose(true);

		/// <summary>
		/// To dispose the collection. Although this method is <see langword="public"/>,
		/// you may not call this method, because this method will be called automatically when
		/// the method <see cref="ToString"/> is called.
		/// </summary>
		/// <param name="clearAll">Indicates whether we should return the buffer.</param>
		/// <seealso cref="ToString"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void Dispose(bool clearAll)
		{
			char[]? toReturn = _chunk;

			if (clearAll)
			{
				// For safety, to avoid using pooled array if this instance is erroneously appended to again.
				this = default;
			}
			else
			{
				// Store the previous data, but clear the length value to 0.
				Length = 0;
				_chars.Clear();
			}

			// Returns the buffer memory.
			if (toReturn is not null)
			{
				ArrayPool<char>.Shared.Return(toReturn);
			}
		}
	}
}
