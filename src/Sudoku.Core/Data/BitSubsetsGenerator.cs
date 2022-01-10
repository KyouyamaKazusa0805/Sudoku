namespace Sudoku.Data;

/// <summary>
/// Encapsulates a bit combination generator.
/// </summary>
/// <remarks>
/// You can use this struct like this:
/// <code><![CDATA[
/// foreach (short mask in new BitSubsetsGenerator(9, 3))
/// {
///     // Do something to use the mask.
/// }
/// ]]></code>
/// </remarks>
public readonly ref partial struct BitSubsetsGenerator
{
	/// <summary>
	/// The inner enumerator.
	/// </summary>
	private readonly Enumerator _enumerator;


	/// <summary>
	/// Initializes an instance with the specified number of bits
	/// and <see langword="true"/> bits.
	/// </summary>
	/// <param name="bitCount">The number of bits.</param>
	/// <param name="oneCount">The number of <see langword="true"/> bits.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public BitSubsetsGenerator(int bitCount, int oneCount) => _enumerator = new(bitCount, oneCount);


	/// <summary>
	/// Gets the enumerator of the current instance in order to use <see langword="foreach"/> loop.
	/// </summary>
	/// <returns>The enumerator instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Enumerator GetEnumerator() => _enumerator;
}
