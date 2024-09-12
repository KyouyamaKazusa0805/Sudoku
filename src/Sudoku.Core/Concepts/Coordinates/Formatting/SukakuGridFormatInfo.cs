namespace Sudoku.Concepts.Coordinates.Formatting;

/// <summary>
/// Represents a <see cref="GridFormatInfo"/> type that supports Sukaku formatting.
/// </summary>
public sealed partial class SukakuGridFormatInfo : GridFormatInfo
{
	[GeneratedRegex("""\d*[\-\+]?\d+""", RegexOptions.Compiled, 5000)]
	public static partial Regex GridSukakuSegmentPattern { get; }


	/// <inheritdoc/>
	[return: NotNullIfNotNull(nameof(formatType))]
	public override object? GetFormat(Type? formatType) => formatType == typeof(GridFormatInfo) ? this : null;

	/// <inheritdoc/>
	public override SukakuGridFormatInfo Clone() => new() { Placeholder = Placeholder, Multiline = Multiline };

	/// <inheritdoc/>
	protected internal override string FormatCore(ref readonly Grid grid)
	{
		if (Multiline)
		{
			// Append all digits.
			var builders = new StringBuilder[81];
			for (var i = 0; i < 81; i++)
			{
				builders[i] = new();
				foreach (var digit in grid.GetCandidates(i))
				{
					builders[i].Append(digit + 1);
				}
			}

			// Now consider the alignment for each column of output text.
			var sb = new StringBuilder();
			var span = (stackalloc int[9]);
			for (var column = 0; column < 9; column++)
			{
				var maxLength = 0;
				for (var p = 0; p < 9; p++)
				{
					maxLength = Math.Max(maxLength, builders[p * 9 + column].Length);
				}

				span[column] = maxLength;
			}
			for (var row = 0; row < 9; row++)
			{
				for (var column = 0; column < 9; column++)
				{
					var cell = row * 9 + column;
					sb.Append(builders[cell].ToString().PadLeft(span[column])).Append(' ');
				}
				sb.RemoveFrom(^1).AppendLine(); // Remove last whitespace.
			}

			return sb.ToString();
		}
		else
		{
			var sb = new StringBuilder();
			for (var i = 0; i < 81; i++)
			{
				sb.Append("123456789");
			}

			for (var i = 0; i < 729; i++)
			{
				if (!grid.GetExistence(i / 9, i % 9))
				{
					sb[i] = Placeholder;
				}
			}

			return sb.ToString();
		}
	}

	/// <inheritdoc/>
	protected internal override Grid ParseCore(string str)
	{
		if (Multiline)
		{
			var matches = from m in GridSukakuSegmentPattern.Matches(str) select m.Value;
			if (matches is { Length: not 81 })
			{
				return Grid.Undefined;
			}

			var result = Grid.Empty;
			for (var offset = 0; offset < 81; offset++)
			{
				var s = Regex.Replace(matches[offset], @"\d", @delegate.ReturnEmptyString);
				if (s.Length > 9)
				{
					// More than 9 characters.
					return Grid.Undefined;
				}

				var mask = (Mask)0;
				foreach (var c in s)
				{
					mask |= (Mask)(1 << c - '1');
				}

				if (mask == 0)
				{
					return Grid.Undefined;
				}

				for (var digit = 0; digit < 9; digit++)
				{
					result.SetExistence(offset, digit, (mask >> digit & 1) != 0);
				}
			}

			return result;
		}
		else
		{
			if (str.Length < 729)
			{
				return Grid.Undefined;
			}

			var result = Grid.Empty;
			for (var i = 0; i < 729; i++)
			{
				var c = str[i];
				if (c is not (>= '0' and <= '9' or '.'))
				{
					return Grid.Undefined;
				}

				if (c is '0' or '.')
				{
					result.SetExistence(i / 9, i % 9, false);
				}
			}

			return result;
		}
	}
}
