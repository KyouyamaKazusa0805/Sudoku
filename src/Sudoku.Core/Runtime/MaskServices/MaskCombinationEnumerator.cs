using System.Runtime.CompilerServices;
using System.SourceGeneration;

namespace Sudoku.Runtime.MaskServices;

/// <summary>
/// Indicates the enumerator of the current instance.
/// </summary>
/// <param name="bitCount">The number of bits.</param>
/// <param name="oneCount">The number of <see langword="true"/> bits.</param>
[Equals]
[GetHashCode]
[ToString]
public ref partial struct MaskCombinationEnumerator(int bitCount, int oneCount)
{
	/// <summary>
	/// The mask.
	/// </summary>
	private readonly long _mask = (1 << bitCount - oneCount) - 1;

	/// <summary>
	/// Indicates whether that the value is the last one.
	/// </summary>
	private bool _isLast = bitCount == 0;


	/// <inheritdoc/>
	public long Current { get; private set; } = (1 << oneCount) - 1;


	/// <inheritdoc/>
	public bool MoveNext()
	{
		var result = hasNext(ref this);
		if (result && !_isLast)
		{
			var smallest = Current & -Current;
			var ripple = Current + smallest;
			var ones = Current ^ ripple;
			ones = (ones >> 2) / smallest;
			Current = ripple | ones;
		}

		return result;


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static bool hasNext(scoped ref MaskCombinationEnumerator @this)
		{
			var result = !@this._isLast;
			@this._isLast = (@this.Current & -@this.Current & @this._mask) == 0;
			return result;
		}
	}
}
