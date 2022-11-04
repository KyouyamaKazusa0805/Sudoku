namespace Sudoku.Text.Notations;

/// <summary>
/// Encapsulates a set of methods that handles a variety of instances, using Hodoku elimination notation
/// to output the <see cref="string"/> representation, or parse a <see cref="string"/> value to convert
/// it to the suitable-typed instance.
/// </summary>
public sealed class EliminationNotation : ICandidateNotation<EliminationNotation, EliminationNotationOptions>
{
	[Obsolete(DeprecatedConstructorsMessage.ConstructorIsMeaningless, false, DiagnosticId = "SCA0108", UrlFormat = "https://sunnieshine.github.io/Sudoku/code-analysis/sca0108")]
	private EliminationNotation() => throw new NotSupportedException();


	/// <inheritdoc/>
	public static CandidateNotation CandidateNotation => CandidateNotation.SusserElimination;


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
			SkipInit(out result);
			return false;
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToCandidatesString(scoped in Candidates candidates)
		=> ToCandidatesString(candidates, EliminationNotationOptions.Default);

	/// <inheritdoc/>
	public static string ToCandidatesString(scoped in Candidates candidates, scoped in EliminationNotationOptions options)
	{
		_ = options is { DigitFirst: var digitFirst, Separator: var separator };

		scoped var sb = new StringHandler();
		foreach (var candidate in candidates)
		{
			int cell = candidate / 9, digit = candidate % 9;
			sb.Append(digitFirst ? $"{digit + 1}{cell / 9 + 1}{cell % 9 + 1}" : $"{cell / 9 + 1}{cell % 9 + 1}{digit + 1}");
			sb.Append(separator);
		}

		if (candidates is not [])
		{
			sb.RemoveFromEnd(separator.Length);
		}

		return sb.ToStringAndClear();
	}

	/// <inheritdoc/>
	public static Candidates ParseCandidates(string str)
	{
		var segments = str.Split(' ');
		if (Array.IndexOf(segments, string.Empty) != -1)
		{
			throw new FormatException("The string contains empty segment.");
		}

		var result = Candidates.Empty;
		foreach (var segment in segments)
		{
			if (segment is not
				[
					var digitChar and >= '1' and <= '9',
					var rowChar and >= '1' and <= '9',
					var columnChar and >= '1' and <= '9'
				])
			{
				throw new FormatException("Each candidate segment contains invalid character.");
			}

			result.AddAnyway(((rowChar - '1') * 9 + columnChar - '1') * 9 + digitChar - '1');
		}

		return result;
	}
}
