namespace Sudoku.Concepts.Notations;

/// <summary>
/// Encapsulates a set of methods that handles a variety of instances, using Hodoku elimination notation
/// to output the <see cref="string"/> representation, or parse a <see cref="string"/> value to convert
/// it to the suitable-typed instance.
/// </summary>
public sealed class EliminationNotation :
	INotationHandler,
	ICandidateNotation<EliminationNotation, EliminationNotationOptions>
{
	[Obsolete("Please don't call this constructor.", true)]
	private EliminationNotation() => throw new NotSupportedException();


	/// <inheritdoc/>
	public Notation Notation => Notation.HodokuElimination;


	/// <inheritdoc/>
	public static bool TryParseCandidates(string str, out Candidates result)
	{
		try
		{
			result = ParseCandidates(str);
			return true;
		}
		catch (FormatException)
		{
			Unsafe.SkipInit(out result);
			return false;
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToCandidatesString(in Candidates candidates) =>
		ToCandidatesString(candidates, EliminationNotationOptions.Default);

	/// <inheritdoc/>
	public static string ToCandidatesString(in Candidates candidates, in EliminationNotationOptions options)
	{
		bool digitFirst = options.DigitFirst;
		string separator = options.Separator;

		var sb = new StringHandler();
		foreach (int candidate in candidates)
		{
			int cell = candidate / 9, digit = candidate % 9;
			sb.Append(
				digitFirst
					? $"{digit + 1}{cell / 9 + 1}{cell % 9 + 1}"
					: $"{cell / 9 + 1}{cell % 9 + 1}{digit + 1}"
			);

			sb.Append(separator);
		}

		sb.RemoveFromEnd(separator.Length);
		return sb.ToStringAndClear();
	}

	/// <inheritdoc/>
	public static Candidates ParseCandidates(string str)
	{
		string[] segments = str.Split(' ');
		if (Array.IndexOf(segments, string.Empty) != -1)
		{
			throw new FormatException("The string contains empty segment.");
		}

		var result = Candidates.Empty;
		foreach (string segment in segments)
		{
			if (
				segment is not [
					var digitChar and >= '0' and < '9',
					var rowChar and >= '0' and < '9',
					var columnChar and >= '0' and < '9'
				]
			)
			{
				throw new FormatException("Each candidate segment contains invalid character.");
			}

			int digit = digitChar - '0', cell = (rowChar - '0') * 9 + columnChar - '0';
			result.AddAnyway(cell * 9 + digit);
		}

		return result;
	}
}
