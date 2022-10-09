namespace Sudoku.Diagnostics.CodeAnalysis;

/// <summary>
/// Provides extension methods on named arguments list type <see cref="NamedArguments"/>.
/// </summary>
/// <seealso cref="NamedArguments"/>
internal static class NamedArgumentsExtensions
{
	/// <summary>
	/// Try to fetch target <see cref="TypedConstant"/> instance whose corresponding key is the specified name.
	/// </summary>
	/// <param name="this">A list of key-value pair to be checked.</param>
	/// <param name="namedArgumentName">The named argument name to be checked.</param>
	/// <returns>The found <see cref="TypedConstant"/> value; otherwise, <see langword="null"/>.</returns>
	public static TypedConstant? GetValueByName(this NamedArguments @this, string namedArgumentName)
	{
		if (@this is [])
		{
			goto ReturnNull;
		}

		foreach (var (key, value) in @this)
		{
			if (key == namedArgumentName)
			{
				return value;
			}
		}

	ReturnNull:
		return null;
	}
}
