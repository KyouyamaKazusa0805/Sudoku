namespace Sudoku.Analytics.StepSearchers;

/// <summary>
/// Represents a searcher that can creates <see cref="Step"/> instances for the specified technique.
/// </summary>
/// <seealso cref="Step"/>
public abstract class StepSearcher
{
	/// <summary>
	/// Returns the qualified type name of this instance.
	/// </summary>
	/// <returns>Qualified type name of this instance.</returns>
	public string ToTypeNameString() => GetType().Name;

	/// <summary>
	/// Returns the real name of this instance.
	/// </summary>
	/// <returns>Real name of this instance.</returns>
	public sealed override string ToString()
	{
		const string commonPrefix = "StepSearcherName_";
		return GetType() switch
		{
			{ Name: var typeName } type => type.GetCustomAttribute<StepSearcherAttribute>() switch
			{
				{ NameResourceEntry: { } key } => R[key] ?? f(typeName),
				_ => f(typeName)
			}
		};


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static string f(string typeName) => R[$"{commonPrefix}{typeName}"] ?? typeName;
	}

	/// <summary>
	/// Try to search for <see cref="Step"/> results for the current technique rule.
	/// </summary>
	/// <param name="context">
	/// <para>
	/// The analysis context. This argument offers you some elementary data configured or assigned, for the current loop of step searching.
	/// </para>
	/// <para>
	/// All available <see cref="Step"/> results will be stored in property <see cref="LogicalAnalysisContext.Accumulator"/>
	/// of this argument, if property <see cref="LogicalAnalysisContext.OnlyFindOne"/> returns <see langword="false"/>;
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
	/// <seealso cref="LogicalAnalysisContext"/>
	public virtual Step? GetAllUnsafe(scoped ref LogicalAnalysisContext context) => GetAll(ref context);

	/// <inheritdoc cref="GetAllUnsafe(ref LogicalAnalysisContext)"/>
	protected internal abstract Step? GetAll(scoped ref LogicalAnalysisContext context);
}
