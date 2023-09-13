using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Sudoku.Text.Formatting;

/// <summary>
/// Represents with a Susser format, but only extracts for pre-eliminations.
/// </summary>
public sealed partial record SusserFormatEliminationsOnly : SusserFormat, IGridFormatter
{
	/// <summary>
	/// Indicates the default instance. The property set are:
	/// <list type="bullet">
	/// <item><see cref="SusserFormat.Placeholder"/>: <c>'.'</c></item>
	/// <item><see cref="SusserFormat.WithModifiables"/>: <see langword="true"/></item>
	/// <item><see cref="SusserFormat.WithCandidates"/>: <see langword="true"/></item>
	/// <item><see cref="SusserFormat.ShortenSusser"/>: <see langword="false"/></item>
	/// </list>
	/// </summary>
	public static new readonly SusserFormatEliminationsOnly Default = new()
	{
		Placeholder = SusserFormat.Default.Placeholder,
		WithModifiables = true,
		WithCandidates = true
	};


	/// <inheritdoc/>
	static IGridFormatter IGridFormatter.Instance => Default;


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString(scoped in Grid grid)
		=> EliminationPattern().Match(base.ToString(grid)) switch
		{
			{ Success: true, Value: var value } => value,
			_ => string.Empty
		};


	[GeneratedRegex("""(?<=\:)(\d{3}\s+)*\d{3}""", RegexOptions.Compiled, 5000)]
	internal static partial Regex EliminationPattern();
}
