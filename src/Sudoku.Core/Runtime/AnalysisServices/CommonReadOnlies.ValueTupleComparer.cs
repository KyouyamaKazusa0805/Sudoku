namespace Sudoku.Runtime.AnalysisServices;

partial class CommonReadOnlies
{
	/// <summary>
	/// The inner comparer of <see cref="ValueTuple{T1, T2}"/> used for
	/// the field <see cref="IntersectionMaps"/>.
	/// </summary>
	/// <seealso cref="IntersectionMaps"/>
	private sealed class ValueTupleComparer : IEqualityComparer<(byte Value1, byte Value2)>
	{
		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals((byte Value1, byte Value2) x, (byte Value1, byte Value2) y) => x == y;

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetHashCode((byte Value1, byte Value2) obj) => obj.Value1 << 5 | obj.Value2;
	}
}
