namespace Sudoku.Solving.Manual.Exocets
{
	/// <summary>
	/// Indicates the type code of an exocet.
	/// </summary>
	public enum ExocetTypeCode : byte
	{
		/// <summary>
		/// Indicates the basic type (Junior Exocet).
		/// </summary>
		[Name("Junior Exocet")]
		Junior = 0,

		/// <summary>
		/// Indicates the senior exocet.
		/// </summary>
		[Name("Senior Exocet")]
		Senior = 1,
	}
}
