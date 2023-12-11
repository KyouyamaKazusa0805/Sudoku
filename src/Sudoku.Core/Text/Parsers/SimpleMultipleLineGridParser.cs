using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Sudoku.Concepts;

namespace Sudoku.Text.Parsers;

/// <summary>
/// Represents a simple multiple-line grid parser.
/// </summary>
public sealed partial record SimpleMultipleLineGridParser : GridParser
{
	/// <inheritdoc/>
	public override Func<string, Grid> Parser
		=> [MethodImpl(MethodImplOptions.AggressiveInlining)] static (string str) =>
		{
			if (GridSimpleMultilinePattern().Match(str) is not { Success: true, Value: var match })
			{
				return Grid.Undefined;
			}

			// Remove all '\r's and '\n's.
			scoped var sb = new StringHandler(81 + (9 << 1));
			sb.Append(from @char in match where @char is not ('\r' or '\n') select @char);
			return new SusserGridParser().Parser(sb.ToStringAndClear());
		};


	[GeneratedRegex("""([\d\.\+]{9}(\r|\n|\r\n)){8}[\d\.\+]{9}""", RegexOptions.Compiled, 5000)]
	public static partial Regex GridSimpleMultilinePattern();
}
