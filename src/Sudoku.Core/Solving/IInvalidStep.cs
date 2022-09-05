namespace Sudoku.Solving;

/// <summary>
/// Provides with an invalid step instance that can be used for some cases that the technique should be skipped
/// because it is temporarily not supported by the technique.
/// </summary>
public interface IInvalidStep : IStep
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
	public static readonly IInvalidStep Instance = new InvalidStep();
}

/// <summary>
/// The background type that has implemented the type <see cref="IInvalidStep"/>.
/// </summary>
/// <seealso cref="IInvalidStep"/>
file sealed class InvalidStep : IInvalidStep
{
	/// <inheritdoc/>
	string IStep.Name => throw new NotSupportedException();

	/// <inheritdoc/>
	string? IStep.Format => throw new NotSupportedException();

	/// <inheritdoc/>
	decimal IStep.Difficulty => throw new NotSupportedException();

	/// <inheritdoc/>
	Technique IStep.TechniqueCode => throw new NotSupportedException();

	/// <inheritdoc/>
	TechniqueTags IStep.TechniqueTags => throw new NotSupportedException();

	/// <inheritdoc/>
	TechniqueGroup IStep.TechniqueGroup => throw new NotSupportedException();

	/// <inheritdoc/>
	DifficultyLevel IStep.DifficultyLevel => throw new NotSupportedException();

	/// <inheritdoc/>
	Stableness IStep.Stableness => throw new NotSupportedException();

	/// <inheritdoc/>
	Rarity IStep.Rarity => throw new NotSupportedException();

	/// <inheritdoc/>
	ImmutableArray<Conclusion> IVisual.Conclusions => throw new NotSupportedException();

	/// <inheritdoc/>
	ImmutableArray<View> IVisual.Views => throw new NotSupportedException();


	/// <inheritdoc/>
	void IStep.ApplyTo(scoped ref Grid grid) => throw new NotSupportedException();

	/// <inheritdoc/>
	string IStep.Formatize(bool handleEscaping) => throw new NotSupportedException();

	/// <inheritdoc/>
	bool IStep.HasTag(TechniqueTags flags) => throw new NotSupportedException();

	/// <inheritdoc/>
	string IStep.ToFullString() => throw new NotSupportedException();

	/// <inheritdoc/>
	string IStep.ToSimpleString() => throw new NotSupportedException();

	/// <inheritdoc/>
	string IStep.ElimStr() => throw new NotSupportedException();
}
