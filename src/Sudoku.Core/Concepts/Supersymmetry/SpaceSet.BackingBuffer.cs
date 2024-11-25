namespace Sudoku.Concepts.Supersymmetry;

public partial struct SpaceSet
{
	/// <summary>
	/// Represents a backing buffer type.
	/// </summary>
	[InlineArray(4)]
	private struct BackingBuffer
	{
		/// <summary>
		/// Indicates the backing field.
		/// </summary>
		private CellMap _map;
	}
}
