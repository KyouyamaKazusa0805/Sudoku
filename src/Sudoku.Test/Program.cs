using Sudoku.CodeGen;

[AutoEquality(nameof(P), nameof(Q))]
internal sealed class S
{
	public int P { get; }

	public S? Q { get; }
}
