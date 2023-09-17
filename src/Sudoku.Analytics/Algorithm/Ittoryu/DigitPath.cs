using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.SourceGeneration;

namespace Sudoku.Algorithm.Ittoryu;

/// <summary>
/// Indicates the target digit path.
/// </summary>
/// <param name="Digits">The digits path.</param>
[ComparisonOperators]
[CollectionBuilder(typeof(DigitPath), nameof(Create))]
public readonly partial record struct DigitPath(Digit[] Digits) :
	IComparable<DigitPath>,
	IComparisonOperators<DigitPath, DigitPath, bool>,
	IEnumerable<Digit>
{
	/// <summary>
	/// Indicates whethe the pattern is complete.
	/// </summary>
	public bool IsComplete => Digits.Length == 9;

	/// <summary>
	/// Indicates hte digits string.
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string[] DigitsString => from digit in Digits select (digit + 1).ToString();


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int CompareTo(DigitPath other) => GetHashCode().CompareTo(other.GetHashCode());

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(DigitPath other) => GetHashCode() == other.GetHashCode();

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		var (result, multiplicativeIdentity) = (0, 1);
		foreach (var digit in Digits.EnumerateReversely())
		{
			result += digit * multiplicativeIdentity;
			multiplicativeIdentity *= 10;
		}

		return result;
	}

	/// <inheritdoc cref="object.ToString"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() => ToString("->");

	/// <inheritdoc cref="ToString()"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(string? separator)
		=> separator switch { null or [] => string.Concat(DigitsString), _ => string.Join(separator, DigitsString) };

	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public OneDimensionalArrayEnumerator<Digit> GetEnumerator() => new(Digits);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator IEnumerable.GetEnumerator() => Digits.GetEnumerator();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator<Digit> IEnumerable<Digit>.GetEnumerator() => ((IEnumerable<Digit>)Digits).GetEnumerator();


	/// <summary>
	/// Creates a <see cref="DigitPath"/> instance via collection expression.
	/// </summary>
	/// <param name="digits">A list of digits to be initialized.</param>
	/// <returns>A <see cref="DigitPath"/> instance.</returns>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[DebuggerStepThrough]
	public static DigitPath Create(scoped ReadOnlySpan<Digit> digits) => new([.. digits]);


	/// <summary>
	/// Implicit cast from a <see cref="Digit"/> sequence into a <see cref="DigitPath"/>.
	/// </summary>
	/// <param name="digitSequence">A digit sequence. Please note that the value can be <see langword="null"/>.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator DigitPath(Digit[]? digitSequence) => new(digitSequence is null ? [] : digitSequence);
}
