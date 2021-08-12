namespace Sudoku.Diagnostics;

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
	public static decimal ConvertTo(long bytes, SizeUnit sizeUnit) => bytes / sizeUnit switch
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
	/// <param name="unit">The unit.</param>
	/// <returns>The value of the specified size unit.</returns>
	public static decimal Convert(long bytes, out SizeUnit unit)
	{
		decimal size; /*mixed-tuple*/
		(unit, size) = bytes switch
		{
			<= 1024L => (SizeUnit.Byte, 1M),
			<= 1048576L => (SizeUnit.Kilobyte, 1024M),
			<= 1073741824L => (SizeUnit.Megabyte, 1048576M),
			<= 1099511627776L => (SizeUnit.Gigabyte, 1073741824M),
			_ => (SizeUnit.Terabyte, 1099511627776M)
		};

		return size != 1M ? bytes / size : bytes;
	}
}
