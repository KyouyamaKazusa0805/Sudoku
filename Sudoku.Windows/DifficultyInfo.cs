#if CSHARP_9_PREVIEW
namespace Sudoku.Windows
{
	/// <summary>
	/// Indicates the difficulty information used for gather the technique information of a puzzle.
	/// </summary>
	public sealed record DifficultyInfo(string? Technique, int Count, decimal Total, decimal Max);
}

namespace System.Runtime.CompilerServices
{
#warning This class need removing after release C# 9.
	public class IsExternalInit
	{
	}
}
#else
	/// <summary>
	/// Indicates the difficulty information used for gather the technique information of a puzzle.
	/// </summary>
	/// <remarks>
	/// This <see langword="class"/> will be replaced with the <see langword="record"/> after using .NET 5.
	/// </remarks>
	public sealed class DifficultyInfo
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="technique">The technique name.</param>
		/// <param name="count">The number of the technique used.</param>
		/// <param name="total">The total difficulty.</param>
		/// <param name="max">The maximum difficulty of all this technique instances.</param>
		public DifficultyInfo(string? technique, int count, decimal total, decimal max) =>
			(Technique, Count, Total, Max) = (technique, count, total, max);


		/// <summary>
		/// The technique name. If the value is <see langword="null"/>, the information will be a summary.
		/// </summary>
		public string? Technique { get; }

		/// <summary>
		/// The count.
		/// </summary>
		public int Count { get; }

		/// <summary>
		/// The total difficulty.
		/// </summary>
		public decimal Total { get; }

		/// <summary>
		/// The maximum difficulty.
		/// </summary>
		public decimal Max { get; }
	}
#endif
