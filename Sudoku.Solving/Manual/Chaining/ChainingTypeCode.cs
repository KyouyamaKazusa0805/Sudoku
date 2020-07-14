namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Indicates a type code for a chain.
	/// </summary>
	public enum ChainingTypeCode : byte
	{
		/// <summary>
		/// Indicates the XY-Chain.
		/// </summary>
		XyChain = 1,

		/// <summary>
		/// Indicates the XY-Cycle.
		/// </summary>
		XyCycle,

		/// <summary>
		/// Indicates the continuous nice loop.
		/// </summary>
		ContinuousNiceLoop,

		/// <summary>
		/// Indicates the XY-X-Chain.
		/// </summary>
		XyXChain,

		/// <summary>
		/// Indicates the discontinuous nice loop.
		/// </summary>
		DiscontinuousNiceLoop,

		/// <summary>
		/// Indicates the alternating inference chain.
		/// </summary>
		Aic,

		/// <summary>
		/// Indicates the cell forcing chains.
		/// </summary>
		CellFc,

		/// <summary>
		/// Indictaes the region forcing chains.
		/// </summary>
		RegionFc,

		/// <summary>
		/// Indicates the binary forcing chains.
		/// </summary>
		BinaryFc,
	}
}
