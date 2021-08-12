namespace Sudoku.Diagnostics.LanguageFeatures;

/// <summary>
/// Presents a syntax replacer that changes the current code style to another.
/// </summary>
public interface ISyntaxReplacer
{
	/// <summary>
	/// To check whether the code is valid to process.
	/// </summary>
	/// <returns>The code to check.</returns>
	/// <remarks>
	/// This method <b>shouldn't</b> be called at any other methods but the method <see cref="Replace(string)"/>.
	/// </remarks>
	/// <seealso cref="Replace(string)"/>
	bool IsValid(string code);

	/// <summary>
	/// Replace the current syntax to another.
	/// </summary>
	/// <param name="code">The code.</param>
	/// <returns>
	/// The replaced result. If the code is invalid, the method should return <see langword="null"/>
	/// as the invalid result.
	/// </returns>
	/// <remarks>
	/// We recommend you should call the method <see cref="IsValid(string)"/> at first to check whether
	/// the code is valid, and then process it.
	/// </remarks>
	/// <seealso cref="IsValid(string)"/>
	string? Replace(string code);


	/// <summary>
	/// Replace the current syntax to another asynchronously.
	/// </summary>
	/// <param name="code">The code.</param>
	/// <returns>
	/// The task of the current operation, returns the data of the result value. The inner data will be
	/// <see langword="null"/> if the code doesn't pass the validation.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public async ValueTask<string?> ReplaceAsync(string code) => await new ValueTask<string?>(Replace(code));

	/// <summary>
	/// Replace the current syntax to another asynchronously.
	/// </summary>
	/// <param name="code">The code.</param>
	/// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
	/// <returns>
	/// The task of the current operation, returns the data of the result value. The inner data will be
	/// <see langword="null"/> if the code doesn't pass the validation.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public async Task<string?> ReplaceAsync(string code, CancellationToken cancellationToken) =>
		await Task.Run(() => Replace(code), cancellationToken);
}
