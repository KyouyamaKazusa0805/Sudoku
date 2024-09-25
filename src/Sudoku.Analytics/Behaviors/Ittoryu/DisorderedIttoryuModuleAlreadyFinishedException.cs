namespace Sudoku.Behaviors.Ittoryu;

/// <summary>
/// The internal exception type that reports "Already finished" information, breaking the recursion.
/// </summary>
internal sealed class DisorderedIttoryuModuleAlreadyFinishedException : Exception
{
	/// <inheritdoc/>
	public override string Message => SR.Get("Message_DisorderedIttoryuModuleAlreadyFinished");
}
