namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Represents a searcher that can creates <see cref="Step"/> instances for the specified technique.
/// </summary>
/// <seealso cref="Step"/>
public abstract class StepSearcher
{
	/// <summary>
	/// The qualified type name of this instance.
	/// </summary>
	protected string TypeName => EqualityContract.Name;

	/// <summary>
	/// Indicates the user-defined base name of the step searcher.
	/// This property can return <see langword="null"/> if this step searcher isn't marked <see cref="StepSearcherNameAttribute"/>.
	/// </summary>
	/// <seealso cref="StepSearcherNameAttribute"/>
	protected string? BaseName
		=> EqualityContract.GetCustomAttribute<StepSearcherNameAttribute>() switch
		{
			{ IsNamedValue: true, Name: var name } => name,
			{ IsResourceValue: true, ResourceEntry: var key } => R[key],
			_ => null
		};

	/// <summary>
	/// Indicates the <see cref="Type"/> instance that represents the reflection data for the current instance.
	/// This property is used as type checking to distinct with multiple <see cref="StepSearcher"/>s.
	/// </summary>
	protected Type EqualityContract => GetType();


	/// <summary>
	/// Returns the real name of this instance.
	/// </summary>
	/// <returns>Real name of this instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sealed override string ToString() => BaseName ?? R[$"StepSearcherName_{TypeName}"] ?? TypeName;

	/// <summary>
	/// Try to search for <see cref="Step"/> results for the current technique rule.
	/// </summary>
	/// <param name="context">
	/// <para>
	/// The analysis context. This argument offers you some elementary data configured or assigned, for the current loop of step searching.
	/// </para>
	/// <para>
	/// All available <see cref="Step"/> results will be stored in property <see cref="AnalysisContext.Accumulator"/>
	/// of this argument, if property <see cref="AnalysisContext.OnlyFindOne"/> returns <see langword="false"/>;
	/// otherwise, the property won't be used, and this method will return the first found step.
	/// </para>
	/// </param>
	/// <returns>
	/// Returns the first found step. The nullability of the return value are as belows:
	/// <list type="bullet">
	/// <item>
	/// <see langword="null"/>:
	/// <list type="bullet">
	/// <item><c><paramref name="context"/>.OnlyFindOne == <see langword="false"/></c>.</item>
	/// <item><c><paramref name="context"/>.OnlyFindOne == <see langword="true"/></c>, but nothing found.</item>
	/// </list>
	/// </item>
	/// <item>
	/// Not <see langword="null"/>:
	/// <list type="bullet">
	/// <item>
	/// <c><paramref name="context"/>.OnlyFindOne == <see langword="true"/></c>,
	/// and found <b>at least one step</b>. In this case the return value is the first found step.
	/// </item>
	/// </list>
	/// </item>
	/// </list>
	/// </returns>
	/// <seealso cref="Step"/>
	/// <seealso cref="AnalysisContext"/>
	protected internal abstract Step? GetAll(scoped ref AnalysisContext context);
}
