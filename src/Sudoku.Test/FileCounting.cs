using Sudoku.Diagnostics;

namespace Sudoku.Test;

/// <summary>
/// Provides the method to create a file counter, and count all files up.
/// </summary>
internal static class FileCounting
{
	/// <summary>
	/// To count all files, and output the result using the <see cref="Console"/>.
	/// </summary>
	internal static void CountUp() => Console.WriteLine(
		new FileCounter(
			root: Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.Parent!.FullName,
			extension: "cs",
			withBinOrObjDirectory: false
		).CountUp()
	);

	/// <summary>
	/// To count all files, and output the result using the <see cref="Console"/> asynchronously.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
	/// <returns>The task of the operation.</returns>
	internal static async Task CountUpAsync(CancellationToken cancellationToken = default) => Console.WriteLine(
		await new FileCounter(
			root: Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.Parent!.FullName,
			extension: "cs",
			withBinOrObjDirectory: false
		).CountUpAsync(cancellationToken)
	);
}
