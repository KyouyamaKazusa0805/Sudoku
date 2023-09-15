using System.Runtime.CompilerServices;
using Sudoku.Concepts;

namespace Sudoku.Text.Formatting;

/// <summary>
/// Provides with a formatter that allows a <see cref="Grid"/> instance being formatted as Hodoku library format.
/// </summary>
/// <seealso cref="Grid"/>
public sealed record HodokuLibraryFormat : SusserFormat, IGridFormatter
{
	/// <summary>
	/// Indicates the format prefix.
	/// </summary>
	private const string FormatPrefix = ":0000:x:";


	/// <summary>
	/// Indicates the default instance.
	/// </summary>
	public static new readonly HodokuLibraryFormat Default = new() { Placeholder = SusserFormat.Default.Placeholder };


	/// <summary>
	/// Indicates the format suffix.
	/// </summary>
	private string FormatSuffix => new(':', WithCandidates ? 2 : 3);


	/// <inheritdoc/>
	static IGridFormatter IGridFormatter.Instance => Default;


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString(scoped ref readonly Grid grid) => $"{FormatPrefix}{base.ToString(in grid)}{FormatSuffix}";
}
