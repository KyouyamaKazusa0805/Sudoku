namespace Sudoku.Solving;

/// <summary>
/// Indicates the module initializer of this project.
/// </summary>
internal static class ModuleInitializer
{
	/// <summary>
	/// The initialize method.
	/// </summary>
	[ModuleInitializer]
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static void Initialize() =>
		StepSearcherPool.InnerCollection = (
			from type in typeof(ModuleInitializer).Assembly.GetTypes()
			where type.IsSubclassOf<StepSearcher>() && !type.IsAbstract && type.ContainsParameterlessConstructor()
			select Activator.CreateInstance(type) as StepSearcher into instance
			where instance is not null
			orderby instance.Options.Priority
			select instance
		).ToArray();
}
