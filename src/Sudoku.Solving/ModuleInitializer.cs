namespace Sudoku.Solving;

/// <summary>
/// Indicates the module initializer of this project.
/// </summary>
internal static class ModuleInitializer
{
	/// <summary>
	/// The initialize method.
	/// </summary>
	[ModuleInitializer, MethodImpl(MethodImplOptions.NoInlining)]
#if false
	[MemberNotNull(nameof(StepSearcherPool.InnerCollection))]
#endif
	public static void Initialize() => StepSearcherPool.Collection = (
		from type in typeof(ModuleInitializer).Assembly.GetTypes()
		where type.HasImplemented<IStepSearcher>() && !type.IsAbstract && type.ContainsParameterlessConstructor()
		select Activator.CreateInstance(type) as IStepSearcher into instance
		where instance is not null
		orderby instance.Options.Priority
		select instance
	).ToArray();
}
