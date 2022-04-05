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
	public static string GetName(this SymmetryType @this)
	{
		if (Enum.IsDefined(@this))
		{
			return typeof(SymmetryType)
				.GetField(@this.ToString())!
				.GetCustomAttribute<EnumFieldNameAttribute>()!
				.Name;
		}

		const string separator = ", ";
		var sb = new StringHandler(210);
		var flags = Enum.GetValues<SymmetryType>()[1..];
		foreach (var flag in flags)
		{
			if (@this.Flags(flag))
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
