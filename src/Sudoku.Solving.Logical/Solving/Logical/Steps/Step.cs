namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a logical solving step that is a technique usage, and contains the conclusions.
/// </summary>
/// <param name="Conclusions"><inheritdoc cref="IVisual.Conclusions" path="/summary"/></param>
/// <param name="Views"><inheritdoc cref="IVisual.Views" path="/summary"/></param>
internal abstract record Step(Conclusion[] Conclusions, View[]? Views) : IStep
{
	/// <inheritdoc/>
	public virtual string Name => R[TechniqueCode.ToString()]!;

	/// <inheritdoc/>
	public virtual string? Format => R[$"TechniqueFormat_{EqualityContract.Name}"];

	/// <inheritdoc/>
	public abstract decimal BaseDifficulty { get; }

	/// <inheritdoc/>
	public abstract Technique TechniqueCode { get; }

	/// <inheritdoc/>
	public abstract TechniqueTags TechniqueTags { get; }

	/// <inheritdoc/>
	public virtual TechniqueGroup TechniqueGroup
		=> Enum.TryParse<TechniqueGroup>(TechniqueCode.ToString(), out var inst) ? inst : TechniqueGroup.None;

	/// <inheritdoc/>
	public abstract DifficultyLevel DifficultyLevel { get; }

	/// <inheritdoc/>
	public abstract Rarity Rarity { get; }

	/// <inheritdoc/>
	public virtual ExtraDifficultyCase[]? ExtraDifficultyCases => null;

	/// <inheritdoc/>
	public abstract IReadOnlyDictionary<string, string[]?>? FormatInterpolatedParts { get; }

	/// <inheritdoc cref="IStep.ConclusionText"/>
	protected string ConclusionText => ConclusionFormatter.Format(Conclusions.ToArray(), FormattingMode.Normal);

	/// <inheritdoc/>
	string IStep.ConclusionText => ConclusionText;


	/// <inheritdoc/>
	public void ApplyTo(scoped ref Grid grid)
	{
		foreach (var conclusion in Conclusions)
		{
			conclusion.ApplyTo(ref grid);
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool HasTag(TechniqueTags flags) => flags.IsFlag() ? TechniqueTags.Flags(flags) : TechniqueTags.MultiFlags(flags);

	/// <summary>
	/// <inheritdoc cref="IStep.ToString" path="/summary"/>
	/// </summary>
	/// <returns>
	/// <inheritdoc cref="IStep.ToString" path="/returns"/>
	/// </returns>
	/// <remarks>
	/// <inheritdoc cref="IStep.ToString" path="//remarks/para[1]"/>
	/// </remarks>
	public sealed override string ToString()
	{
		var currentCultureName = CultureInfo.CurrentCulture.Name;
		var formatArgs = FormatInterpolatedParts?.FirstOrDefault(comparison).Value;
		var colonToken = R.EmitPunctuation(Punctuation.Colon);
		return (Format, formatArgs) switch
		{
			(null, _) => ToSimpleString(),
			(_, null) => $"{Name}{colonToken}{Format} => {ConclusionText}",
			_ => $"{Name}{colonToken}{string.Format(Format, formatArgs)} => {ConclusionText}"
		};


		bool comparison(KeyValuePair<string, string[]?> kvp)
			=> currentCultureName.StartsWith(kvp.Key, StringComparison.CurrentCultureIgnoreCase);
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToSimpleString() => $"{Name} => {ConclusionText}";
}
