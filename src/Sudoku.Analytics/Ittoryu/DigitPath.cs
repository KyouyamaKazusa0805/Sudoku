namespace Sudoku.Ittoryu;

/// <summary>
/// Indicates the target digit path.
/// </summary>
/// <param name="Digits">The digits path.</param>
/// <param name="Nodes">Indicates the nodes that describes the detail for the solving path.</param>
public readonly record struct DigitPath(Digit[] Digits, PathNode[] Nodes)
{
	/// <summary>
	/// Indicates whethe the pattern is complete.
	/// </summary>
	public bool IsComplete => Digits.Length == 9;


	/// <inheritdoc/>
	public bool Equals(DigitPath other) => GetHashCode() == other.GetHashCode();

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		var (result, multiplicativeIdentity) = (0, 1);
		foreach (var digit in Digits)
		{
			result += digit * multiplicativeIdentity;
			multiplicativeIdentity *= 10;
		}

		return result;
	}

	/// <inheritdoc cref="object.ToString"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() => string.Join("->", Digits);

	/// <inheritdoc cref="ToString()"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(string separator) => string.Join(separator, Digits);
}
