namespace Sudoku.Concepts;

/// <summary>
/// Represents a type that has ability to create <see cref="Grid"/> instances called by compiler.
/// For the users' aspect, we can just use collection expressions.
/// </summary>
/// <seealso cref="Grid"/>
public static class GridCreator
{
	/// <summary>
	/// Returns <see cref="Grid.Undefined"/>, which is equivalent to <see langword="default"/>(<see cref="Grid"/>).
	/// </summary>
	/// <returns>
	/// Instance whose internal values are same as <see cref="Grid.Undefined"/>, i.e. a copy from field <see cref="Grid.Undefined"/>.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Grid Create() => Grid.Undefined;

	/// <summary>
	/// Returns a <see cref="Grid"/> instance via the raw mask values.
	/// </summary>
	/// <param name="rawMaskValues">
	/// <para>The raw mask values.</para>
	/// <para>
	/// This value can contain 1 or 81 elements.
	/// If the array contain 1 element, all elements in the target sudoku grid will be initialized by it, the uniform value;
	/// if the array contain 81 elements, elements will be initialized by the array one by one using the array elements respectively.
	/// </para>
	/// </param>
	/// <returns>A <see cref="Grid"/> result.</returns>
	public static Grid Create(scoped ReadOnlySpan<Mask> rawMaskValues)
	{
		switch (rawMaskValues.Length)
		{
			case 1:
			{
				var result = Grid.Undefined;
				var uniformValue = rawMaskValues[0];
				for (var cell = 0; cell < 81; cell++)
				{
					result[cell] = uniformValue;
				}
				return result;
			}
			case 81:
			{
				var result = Grid.Undefined;
				for (var cell = 0; cell < 81; cell++)
				{
					result[cell] = rawMaskValues[cell];
				}
				return result;
			}
			default:
			{
				throw new InvalidOperationException($"The argument '{nameof(rawMaskValues)}' must contain 81 elements.");
			}
		}
	}
}
