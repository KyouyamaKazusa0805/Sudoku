namespace Sudoku.Text.Formatting;

/// <summary>
/// Represents with a formatter type that formats and parses a <see cref="CandidateMap"/> instance,
/// converting it into an equivalent <see cref="string"/> value.
/// </summary>
/// <seealso cref="CandidateMap"/>
public interface ICandidateMapFormatter : IFormatProvider
{
	/// <summary>
	/// Try to format a <see cref="CandidateMap"/> instance into the specified target-formatted <see cref="string"/> representation.
	/// </summary>
	/// <param name="candidateMap">A <see cref="CandidateMap"/> instance to be formatted.</param>
	/// <returns>A <see cref="string"/> representation as result.</returns>
	string ToString(scoped in CandidateMap candidateMap);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[return: NotNullIfNotNull(nameof(formatType))]
	object? IFormatProvider.GetFormat(Type? formatType) => formatType == GetType() ? this : null;
}
