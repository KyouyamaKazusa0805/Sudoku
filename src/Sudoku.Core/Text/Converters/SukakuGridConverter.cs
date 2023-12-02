using System.Numerics;
using System.Runtime.CompilerServices;
using System.SourceGeneration;
using System.Text;
using Sudoku.Concepts;
using Sudoku.Text.Converters;

namespace Sudoku.Text.SudokuGrid;

/// <summary>
/// Represents a converter type that converts a <see cref="Grid"/> instance into an equivalent <see cref="string"/> value
/// using Sukaku formatting rule.
/// </summary>
/// <param name="Multiline">
/// <para>Indicates whether the output should be multi-line.</para>
/// <para>The default value is <see langword="false"/>.</para>
/// </param>
public sealed partial record SukakuGridConverter(bool Multiline = false) : GridConverter
{
	/// <summary>
	/// Indicates the dot character.
	/// </summary>
	private const char Dot = '.';

	/// <summary>
	/// Indicates the zero character.
	/// </summary>
	private const char Zero = '0';


	/// <summary>
	/// Indicates the default instance. The property set are:
	/// <list type="bullet">
	/// <item><see cref="Placeholder"/>: <c>'.'</c></item>
	/// <item><see cref="Multiline"/>: <see langword="false"/></item>
	/// </list>
	/// </summary>
	public static readonly SukakuGridConverter Default = new() { Placeholder = '.' };


	/// <summary>
	/// Indicates the placeholder of the grid text formatter.
	/// </summary>
	/// <value>The new placeholder text character to be set. The value must be <c>'.'</c> or <c>'0'</c>.</value>
	/// <returns>The placeholder text.</returns>
	[ImplicitField]
	public required char Placeholder
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _placeholder;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		init => _placeholder = value switch
		{
			Zero or Dot => value,
			_ => throw new InvalidOperationException($"The placeholder character invalid; expected: '{Zero}' or '{Dot}'.")
		};
	}


	/// <inheritdoc/>
	public override GridNotationConverter Converter
		=> (scoped ref readonly Grid grid) =>
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
				scoped var span = (stackalloc int[9]);
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
		};
}
