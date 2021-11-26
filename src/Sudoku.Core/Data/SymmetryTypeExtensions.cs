namespace Sudoku.Data;

/// <summary>
/// Provides extension methods for <see cref="SymmetryType"/>.
/// </summary>
/// <seealso cref="SymmetryType"/>
public static class SymmetryTypeExtensions
{
	/// <summary>
	/// Get the name of the current symmetry type.
	/// </summary>
	/// <param name="this">The type.</param>
	/// <returns>The name.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string GetName(this SymmetryType @this)
	{
		return @this switch
		{
			SymmetryType.None => "No symmetry",
			SymmetryType.Central => "Central symmetry type",
			SymmetryType.Diagonal => "Diagonal symmetry type",
			SymmetryType.AntiDiagonal => "Anti-diagonal symmetry type",
			SymmetryType.XAxis => "X-axis symmetry type",
			SymmetryType.YAxis => "Y-axis symmetry type",
			SymmetryType.AxisBoth => "Both X-axis and Y-axis",
			SymmetryType.DiagonalBoth => "Both diagonal and anti-diagonal",
			SymmetryType.All => "All symmetry type",
			_ => f(@this)
		};


		static string f(SymmetryType type)
		{
			const string separator = ", ";
			var sb = new StringHandler(initialCapacity: 210);
			var flags = Enum.GetValues<SymmetryType>()[1..];
			foreach (var flag in flags)
			{
				if (type.Flags(flag))
				{
					sb.Append(flag.ToString());
					sb.AppendLiteral(separator);
				}
			}

			if (sb.Length != 0)
			{
				sb.RemoveFromEnd(separator.Length);
				return sb.ToStringAndClear();
			}

			return string.Empty;
		}
	}
}
