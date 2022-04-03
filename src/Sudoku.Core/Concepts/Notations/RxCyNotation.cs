namespace Sudoku.Concepts.Notations;

/// <summary>
/// Defines the notation using RxCy notation.
/// </summary>
public sealed class RxCyNotation : IRegionNotation
{
	/// <summary>
	/// </summary>
	/// <exception cref="NotSupportedException">Always throws.</exception>
	[Obsolete("Do not call the constructor.", true)]
	private RxCyNotation() => throw new NotSupportedException();


	/// <inheritdoc/>
	public static string ToRegionString(params int[] regions)
	{
		if (regions.Length == 0
			|| ((int[])regions.Clone()).Distinct().ToArray() is var regionArray && regionArray.Length == 0)
		{
			return "{ }";
		}

		using StringHandler sbRow = new(9), sbColumn = new(9), sbBlock = new(9);
		foreach (int region in regionArray)
		{
			int value = region % 9 + 1;
			switch (region)
			{
				case >= 0 and < 9:
				{
					sbBlock.Append(value);
					break;
				}
				case >= 9 and < 18:
				{
					sbRow.Append(value);
					break;
				}
				case >= 18 and < 27:
				{
					sbColumn.Append(value);
					break;
				}
				default:
				{
					throw new InvalidOperationException("The specified region value is invalid.");
				}
			}
		}

		var targetSb = new StringHandler(30);
		f(sbRow, ref targetSb, 'r');
		f(sbColumn, ref targetSb, 'c');
		f(sbBlock, ref targetSb, 'b');

		return targetSb.ToStringAndClear();

		static void f(in StringHandler sbIterator, ref StringHandler targetSb, char c)
		{
			bool firstTime = true;
			foreach (char character in sbIterator)
			{
				if (firstTime)
				{
					targetSb.Append(c);
					firstTime = false;
				}

				targetSb.Append(character);
			}
		}
	}

	/// <inheritdoc/>
	public static string ToRegionStringSimple(params int[] regions)
	{
		if (regions.Length == 0
			|| ((int[])regions.Clone()).Distinct().ToArray() is var regionArray && regionArray.Length == 0)
		{
			return "{ }";
		}

		string @base = new string('r', 9) + new string('c', 9) + new string('b', 9);
		var targetSpan = (stackalloc char[27]);
		targetSpan.Fill('\0');

		Array.Sort(regionArray);
		int i = 0;
		foreach (int region in regionArray)
		{
			targetSpan[i++] = @base[region];
		}

		return targetSpan.ToString();
	}
}
