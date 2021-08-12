namespace Sudoku.Solving.Manual
{
	/// <summary>
	/// Encapsulates a step finder that used in solving in <see cref="ManualSolver"/>.
	/// </summary>
	/// <seealso cref="ManualSolver"/>
	public abstract class StepSearcher : IStepSearcher
	{
		/// <summary>
		/// Indicates the necessary property name.
		/// </summary>
		[Obsolete("The field is deprecated.", false)]
		private const string NecessaryPropertyName = "Properties";


		/// <inheritdoc/>
		public abstract SearchingOptions Options { get; set; }

		/// <summary>
		/// Indicates all step searchers and their type info used in the current solution.
		/// </summary>
		/// <remarks>
		/// Please note that the return value is a list of elements that contain its type and its
		/// searcher properties.
		/// </remarks>
		[Obsolete("The property is deprecated.", false)]
		public static IEnumerable<(Type CurrentType, string SearcherName, TechniqueProperties Properties)> AllStepSearchers =>
			from type in Assembly.GetExecutingAssembly().GetTypes()
			where !type.IsAbstract && type.IsSubclassOf<StepSearcher>() && !type.IsDefined<ObsoleteAttribute>()
			orderby TechniqueProperties.FromType(type)!.Priority
			let v = type.GetProperty(NecessaryPropertyName, BindingFlags.Public | BindingFlags.Static)
			let casted = v?.GetValue(null) as TechniqueProperties
			where casted is not null && !casted.DisabledReason.Flags(DisabledReason.HasBugs)
			select (type, (string)TextResources.Current[$"Progress{casted.DisplayLabel}"], casted);


		/// <inheritdoc/>
		public abstract void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid);

		/// <inheritdoc/>
		public override string ToString() =>
			$"{{ Type = {GetType().FullName}, Priority = {Options.Priority}, DisplayingLevel = {Options.DisplayingLevel} }}";

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public StepInfo? GetOne(in SudokuGrid grid) => ((IStepSearcher)this).GetOne(grid);
	}
}
