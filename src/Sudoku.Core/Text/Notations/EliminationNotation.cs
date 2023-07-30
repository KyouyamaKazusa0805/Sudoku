namespace Sudoku.Text.Notations;

/// <summary>
/// Encapsulates a set of methods that handles a variety of instances, using Hodoku elimination notation
/// to output the <see cref="string"/> representation, or parse a <see cref="string"/> value to convert
/// it to the suitable-typed instance.
/// </summary>
public sealed class EliminationNotation : ICandidateNotation<EliminationNotation, EliminationNotationOptions>
{
	/// <inheritdoc/>
	public static CandidateNotation CandidateNotation => CandidateNotation.SusserElimination;


	/// <inheritdoc/>
	public static bool TryParseCandidates(string str, out CandidateMap result)
	{
		try
		{
			result = ParseCandidates(str);
			return true;
		}
		catch (FormatException)
		{
			SkipInit(out result);
			return false;
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToCandidatesString(scoped in CandidateMap candidates)
		=> ToCandidatesString(candidates, EliminationNotationOptions.Default);

	/// <inheritdoc/>
	public static string ToCandidatesString(scoped in CandidateMap candidates, scoped in EliminationNotationOptions options)
	{
		_ = options is { DigitFirst: var digitFirst, Separator: var separator };

		scoped var sb = new StringHandler();
		foreach (var candidate in candidates)
		{
			var cell = candidate / 9;
			var digit = candidate % 9;
			sb.Append(digitFirst ? $"{digit + 1}{cell / 9 + 1}{cell % 9 + 1}" : $"{cell / 9 + 1}{cell % 9 + 1}{digit + 1}");
			sb.Append(separator);
		}

		if (candidates)
		{
			sb.RemoveFromEnd(separator.Length);
		}

		return sb.ToStringAndClear();
	}

	/// <inheritdoc/>
	public static CandidateMap ParseCandidates(string str)
	{
		var segments = str.SplitBy([' ']);
		if (Array.IndexOf(segments, string.Empty) != -1)
		{
			throw new FormatException("The string contains empty segment.");
		}

		var result = CandidateMap.Empty;
		foreach (var segment in segments)
		{
			if (segment is not [var digitChar and >= '1' and <= '9', var rowChar and >= '1' and <= '9', var columnChar and >= '1' and <= '9'])
			{
				throw new FormatException("Each candidate segment contains invalid character.");
			}

			result.Add(((rowChar - '1') * 9 + columnChar - '1') * 9 + digitChar - '1');
		}

		return result;
	}
}
