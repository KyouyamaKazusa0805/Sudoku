namespace Sudoku.Diagnostics
{
	/// <summary>
	/// Indicates a unit for a file size.
	/// </summary>
	/*closed*/
	public enum SizeUnit : byte
	{
		/// <summary>
		/// Indicates the size is of bytes.
		/// </summary>
		Byte,

		/// <summary>
		/// Indicates the size is of kilobytes (KB).
		/// </summary>
		Kilobyte,

		/// <summary>
		/// Indicates the size is of megabytes (MB).
		/// </summary>
		Megabyte,

		/// <summary>
		/// Indicates the size is of gigabytes (GB).
		/// </summary>
		Gigabyte,

		/// <summary>
		/// Indicates the size is of terabytes (TB).
		/// </summary>
		Terabyte,

		/// <summary>
		/// Indicates the size is of KiB.
		/// </summary>
		IKilobyte,

		/// <summary>
		/// Indicates the size is of MiB.
		/// </summary>
		IMegabyte,
		/// <summary>
		/// Indicates the size is of GiB.
		/// </summary>
		IGigabyte,

		/// <summary>
		/// Indicates the size is of TiB.
		/// </summary>
		ITerabyte,
	}
}
