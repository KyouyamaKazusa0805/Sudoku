using System.Runtime.CompilerServices;
using System.Text;

namespace Sudoku.Text.Notation;

/// <summary>
/// Represents a notation that formats digits specified as <see cref="Mask"/>.
/// </summary>
/// <seealso cref="Mask"/>
public sealed partial class DigitNotation : INotation<DigitNotation, Mask, DigitNotation.Kind>
{
	/// <inheritdoc/>
	public static Mask Parse(string text, Kind notation)
	{
		var mask = (Mask)0;
		foreach (var digitChar in text)
		{
			switch (digitChar)
			{
				case >= '1' and <= '9':
				{
					mask |= (Mask)(1 << digitChar - '1');
					break;
				}
				case ',' when notation == Kind.Separated:
				{
					continue;
				}
				case ',':
				{
					throw new InvalidOperationException("The character is invalid in compact mode.");
				}
				default:
				{
					throw new InvalidOperationException("Invalid character encountered.");
				}
			}
		}

		return mask;
	}

	/// <inheritdoc cref="ToString(Mask, Kind)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToString(Mask value) => ToString(value, Kind.Separated);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToString(Mask value, Kind notation)
	{
		return notation switch
		{
			Kind.Separated => defaultToString(value, ", "),
			Kind.Compact => defaultToString(value, string.Empty),
			_ => throw new ArgumentOutOfRangeException(nameof(notation))
		};


		static string defaultToString(Mask digitsMask, string separator)
		{
			scoped var sb = new StringHandler(9);
			foreach (var digit in digitsMask)
			{
				sb.Append(digit + 1);
				sb.Append(separator);
			}

			sb.RemoveFromEnd(separator.Length);
			return sb.ToStringAndClear();
		}
	}

	/// <summary>
	/// Gets the string representation for the specified digit.
	/// </summary>
	/// <param name="digit">The digit.</param>
	/// <returns>The string representation of that digit.</returns>
	/// <remarks><i><b>
	/// Please note that this method is very close to <see cref="ToString(Mask)"/>. The only difference is the type of the parameter
	/// (one is a <see cref="Mask"/> and the other is a <see cref="Digit"/>). Don't invoke this method if you don't know
	/// whether the target value is a digit or not.
	/// </b></i></remarks>
	/// <seealso cref="ToString(Mask)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToString(Digit digit) => (digit + 1).ToString();
}
