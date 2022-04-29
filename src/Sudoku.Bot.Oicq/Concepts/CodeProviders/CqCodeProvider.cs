namespace Sudoku.Bot.Oicq.Concepts.CodeProviders;

/// <summary>
/// Defines a CQ code provider.
/// </summary>
public sealed class CqCodeProvider : CodeProvider
{
	/// <inheritdoc/>
	public override string At(string target) => $"[CQ:at,qq={target}]";

	/// <inheritdoc/>
	public override string Image(string path) => $"[CQ:image,file=file:///{path}]";
}
