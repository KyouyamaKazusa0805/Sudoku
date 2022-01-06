namespace System;

/// <summary>
/// Provides extension methods on <see cref="string"/>.
/// </summary>
/// <seealso cref="string"/>
internal static class StringExtensions
{
#if !NETSTANDARD2_1_OR_GREATER
	/// <summary>
	/// Check whether the last character is the specified character.
	/// </summary>
	/// <param name="this">The string.</param>
	/// <param name="character">The character to check.</param>
	/// <returns>A <see cref="bool"/> result</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool EndsWith(this string @this, char character) => @this[^1] == character;
#endif

	/// <summary>
	/// Count how many specified characters are in the current string.
	/// </summary>
	/// <param name="this">The current string.</param>
	/// <param name="character">The character to count.</param>
	/// <returns>The number of characters found.</returns>
	public static unsafe int CountOf(this string @this, char character)
	{
		int count = 0;
		fixed (char* pThis = @this)
		{
			for (char* p = pThis; *p != '\0'; p++)
			{
				if (*p == character)
				{
					count++;
				}
			}
		}

		return count;
	}

	/// <summary>
	/// Converts the current string into the camel case.
	/// </summary>
	/// <param name="this">The string.</param>
	/// <returns>The result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToCamelCase(this string @this)
	{
		@this = @this.TrimStart('_');
		return @this.Substring(0, 1).ToLowerInvariant() + @this.Substring(1);
	}

	/// <summary>
	/// Separate the attribute string representation into multiple elements.
	/// The attribute string will be like <c>Type({value1, value2, value3})</c>.
	/// </summary>
	/// <param name="this">The attribute string.</param>
	/// <param name="tokenStartIndex">The token start index.</param>
	/// <returns>The array of separated values.</returns>
	internal static string[]? GetMemberValues(this string @this, int tokenStartIndex)
	{
		string[] result = (
			from parameterValue in @this.Substring(
				tokenStartIndex,
				@this.Length - tokenStartIndex - 2
			).Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries)
			select parameterValue.Substring(1, parameterValue.Length - 2)
		).ToArray();

		if (result.Length == 0)
		{
			return null;
		}

		result[0] = result[0][2..];
		return result;
	}

	/// <summary>
	/// To split the info on the <c>*.csv</c> file line.
	/// </summary>
	/// <param name="this">A line of the file.</param>
	/// <returns>The <see cref="string"/>[] result.</returns>
	/// <exception cref="ArgumentException">Throws when the specified string is invalid to split.</exception>
	internal static unsafe string[] SplitInfo(this string @this)
	{
		if ((@this.CountOf('"') & 1) != 0)
		{
			throw new ArgumentException("The specified string is invalid to split.", nameof(@this));
		}

		fixed (char* pLine = @this)
		{
			for (int i = 0, outerLoopIterationLength = @this.Length - 1; i < outerLoopIterationLength;)
			{
				if (pLine[i++] != '"')
				{
					continue;
				}

				for (int j = i + 1, innerLoopIterationLength = @this.Length; j < innerLoopIterationLength; j++)
				{
					if (pLine[j] != '"')
					{
						continue;
					}

					for (int p = i + 1; p <= j - 1; p++)
					{
						if (pLine[p] == ',')
						{
							// Change to the temporary character.
							pLine[p] = '，';
						}
					}

					i = j + 1 + 1;
					break;
				}
			}
		}

		string[] result = @this.Split(',');
		for (int i = 0, length = result.Length; i < length; i++)
		{
			string temp = result[i].Replace(@"""", string.Empty).Replace('，', ',');

			result[i] = i == result.Length - 1 || i == result.Length - 2
				? string.IsNullOrEmpty(temp) ? string.Empty : temp.Substring(0, temp.Length - 1)
				: temp;
		}

		return result;
	}
}
