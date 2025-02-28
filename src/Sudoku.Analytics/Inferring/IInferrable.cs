namespace Sudoku.Inferring;

/// <summary>
/// Represents an inferrable object.
/// </summary>
/// <typeparam name="TResult">
/// The type of the result. The result type should be considered as a non-<see langword="null"/> state.
/// </typeparam>
public interface IInferrable<TResult> where TResult : notnull, allows ref struct
{
	/// <summary>
	/// Infers the object and returns an instance of type <typeparamref name="TResult"/>, indicating the result.
	/// </summary>
	/// <param name="grid">The grid to be checked.</param>
	/// <param name="result">The result returned.</param>
	/// <returns>
	/// <para>A <see cref="bool"/> value indicating whether the operation is successfully handled.</para>
	/// <para>
	/// To differ with <paramref name="result"/>, this value only records the result
	/// that the current operation contains invalid data input or exception encountered.
	/// In other words, the argument <paramref name="result"/> may not have a valid data
	/// even if the return value is <see langword="true"/>.
	/// </para>
	/// </returns>
	public static abstract bool TryInfer(in Grid grid, [NotNullWhen(true)] out TResult? result);
}
