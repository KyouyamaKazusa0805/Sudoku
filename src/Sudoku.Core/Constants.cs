using static System.Numerics.BitOperations;

namespace Sudoku;

/// <summary>
/// Encapsulates the class that stores all constants used in the whole project.
/// </summary>
public static partial class Constants
{
	/// <summary>
	/// Indicates the invalid return value of <see cref="PopCount(uint)"/>.
	/// </summary>
	/// <seealso cref="PopCount(uint)"/>
	public const int InvalidFirstSet = sizeof(uint) * 8;

	/// <summary>
	/// Indicates the invalid return value of <see cref="PopCount(ulong)"/>.
	/// </summary>
	/// <seealso cref="PopCount(ulong)"/>
	public const int InvalidFirstSetLong = sizeof(ulong) * 8;
}
