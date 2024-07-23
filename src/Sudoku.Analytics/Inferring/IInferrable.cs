namespace Sudoku.Inferring;

/// <summary>
/// Represents an inferrable object.
/// </summary>
/// <typeparam name="T">The type of the result.</typeparam>
public interface IInferrable<T> where T : allows ref struct
{
	/// <summary>
	/// Infers the object and returns an instance of type <typeparamref name="T"/>.
	/// </summary>
	/// <param name="grid">The grid to be checked.</param>
	/// <param name="result">The result returned.</param>
	public static abstract bool TryInfer(ref readonly Grid grid, [NotNullWhen(true)] out T? result);
}
