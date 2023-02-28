namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with an invalid step instance that can be used for some cases that the technique should be skipped
/// because it is temporarily not supported by the technique.
/// </summary>
public partial interface IInvalidStep : IStep
{
	/// <summary>
	/// Indicates the only instance of type <see cref="IInvalidStep"/> that can be used.
	/// </summary>
	/// <remarks>
	/// This instance is special. The instance will be used only if the step searcher found the puzzle
	/// being invalid to be successfully and correctly handled.
	/// All members existed in type <see cref="IStep"/> are implemented with throwing behavior.
	/// In other words, you cannot use any possible members in this instance; otherwise you will get
	/// a <see cref="NotSupportedException"/> instance.
	/// </remarks>
	/// <seealso cref="NotSupportedException"/>
	public static readonly IInvalidStep Instance;
}
