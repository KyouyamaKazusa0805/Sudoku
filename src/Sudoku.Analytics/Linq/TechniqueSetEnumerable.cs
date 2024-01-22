namespace Sudoku.Linq;

/// <summary>
/// Represents a list of methods that is used for LINQ on <see cref="TechniqueSet"/>.
/// </summary>
/// <seealso cref="TechniqueSet"/>
public static class TechniqueSetEnumerable
{
	/// <inheritdoc cref="ArrayEnumerable.Select{T, TResult}(T[], Func{T, TResult})"/>
	public static ReadOnlySpan<TResult> Select<TResult>(this TechniqueSet @this, Func<Technique, TResult> selector)
	{
		var result = new TResult[@this.Count];
		var i = 0;
		foreach (var technique in @this)
		{
			result[i++] = selector(technique);
		}

		return result;
	}

	/// <inheritdoc cref="ArrayEnumerable.Where{T}(T[], Func{T, bool})"/>
	public static TechniqueSet Where(this TechniqueSet @this, Func<Technique, bool> selector)
	{
		var result = new List<Technique>(@this.Count);
		foreach (var technique in @this)
		{
			if (selector(technique))
			{
				result.Add(technique);
			}
		}

		return [.. result];
	}
}
