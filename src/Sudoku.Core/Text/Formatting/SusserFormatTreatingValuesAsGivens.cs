using Sudoku.Concepts;

namespace Sudoku.Text.Formatting;

/// <summary>
/// Represents with a Susser formatter, removing all plus mark <c>'+'</c> as modifiable distinction tokens.
/// </summary>
public sealed record SusserFormatTreatingValuesAsGivens : SusserFormat, IGridFormatter
{
	/// <summary>
	/// Indicates the default instance. The property set are:
	/// <list type="bullet">
	/// <item><see cref="SusserFormat.Placeholder"/>: <c>'.'</c></item>
	/// <item><see cref="SusserFormat.WithModifiables"/>: <see langword="true"/></item>
	/// <item><see cref="SusserFormat.WithCandidates"/>: <see langword="false"/></item>
	/// <item><see cref="SusserFormat.ShortenSusser"/>: <see langword="false"/></item>
	/// </list>
	/// </summary>
	public static new readonly SusserFormatTreatingValuesAsGivens Default = new()
	{
		Placeholder = SusserFormat.Default.Placeholder,
		WithModifiables = true
	};


	/// <inheritdoc/>
	static IGridFormatter IGridFormatter.Instance => Default;


	/// <inheritdoc/>
	public override string ToString(in Grid grid) => base.ToString(grid).RemoveAll(ModifiablePrefix);
}
