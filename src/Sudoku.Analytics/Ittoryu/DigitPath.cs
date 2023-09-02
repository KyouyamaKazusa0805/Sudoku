namespace Sudoku.Ittoryu;

/// <summary>
/// Indicates the target digit path.
/// </summary>
/// <param name="Digits">The digits path.</param>
public readonly record struct DigitPath(Digit[] Digits)
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
	public override string ToString() => string.Join("->", from digit in Digits select digit + 1);

	/// <inheritdoc cref="ToString()"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(string separator) => string.Join(separator, from digit in Digits select digit + 1);


	/// <summary>
	/// Implicit cast from a <see cref="Digit"/> sequence into a <see cref="DigitPath"/>.
	/// </summary>
	/// <param name="digitSequence">A digit sequence. Please note that the value can be <see langword="null"/>.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator DigitPath(Digit[]? digitSequence) => new(digitSequence is null ? [] : digitSequence);
}
