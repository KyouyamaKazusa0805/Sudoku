using System;

namespace Sudoku.Diagnostics
{
	/// <summary>
	/// Encapsulates a size unit converter.
	/// </summary>
	public static class SizeUnitConverter
	{
		/// <summary>
		/// To convert the size into the specified unit.
		/// </summary>
		/// <param name="bytes">The bytes.</param>
		/// <param name="sizeUnit">The size unit.</param>
		/// <returns>The value of the specified size unit.</returns>
		public static decimal ConvertTo(long bytes, SizeUnit sizeUnit) =>
			bytes / sizeUnit switch
			{
				SizeUnit.Byte => 1M,
				SizeUnit.Kilobyte => 1024M,
				SizeUnit.IKilobyte => 1000M,
				SizeUnit.Megabyte => 1048576M,
				SizeUnit.IMegabyte => 1000000M,
				SizeUnit.Gigabyte => 1073741824M,
				SizeUnit.IGigabyte => 1000000000M,
				SizeUnit.Terabyte => 1099511627776M,
				SizeUnit.ITerabyte => 1000000000000M,
				_ => throw new ArgumentException("The specified argument is invalid.", nameof(sizeUnit))
			};

		/// <summary>
		/// To convert the size into the appropriate size unit.
		/// </summary>
		/// <param name="bytes">The bytes.</param>
		/// <param name="unit">(<see langword="out"/> parameter) The unit.</param>
		/// <returns>The value of the specified size unit.</returns>
		public static decimal Convert(long bytes, out SizeUnit unit)
		{
			switch (bytes)
			{
				case <= 1024L:
				{
					unit = SizeUnit.Byte;
					return bytes;
				}
				case <= 1048576L:
				{
					unit = SizeUnit.Kilobyte;
					return bytes / 1024M;
				}
				case <= 1073741824L:
				{
					unit = SizeUnit.Megabyte;
					return bytes / 1048576M;
				}
				case <= 1099511627776L:
				{
					unit = SizeUnit.Gigabyte;
					return bytes / 1073741824M;
				}
				default:
				{
					unit = SizeUnit.Terabyte;
					return bytes / 1099511627776M;
				}
			}
		}
	}
}
