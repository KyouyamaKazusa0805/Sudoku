namespace Sudoku.Concepts;

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
	public static string GetNameMulti(this SymmetryType @this)
	{
		const string separator = ", ";
		var sb = new StringHandler(210);
		foreach (var flag in Enum.GetValues<SymmetryType>()[1..])
		{
			if (@this.Flags(flag))
			{
				sb.Append(flag.ToString());
				sb.AppendLiteral(separator);
			}
		}

		if (sb is not [])
		{
			sb.RemoveFromEnd(separator.Length);
			return sb.ToStringAndClear();
		}

		return string.Empty;
	}
}
