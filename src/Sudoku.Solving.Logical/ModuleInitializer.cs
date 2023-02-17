namespace Sudoku.Solving.Logical;

/// <include file='../../global-doc-comments.xml' path='g/csharp9/feature[@name="module-initializer"]/target[@name="type"]' />
internal static class ModuleInitializer
{
	/// <include file='../../global-doc-comments.xml' path='g/csharp9/feature[@name="module-initializer"]/target[@name="method"]' />
	[ModuleInitializer]
	[RequiresUnreferencedCode(ModuleInitializerMessage.ModuleInitializerCannotBeCalledManually)]
	public static void Initialize()
	{
		// Registers the assembly.
		R.RegisterAssembly(typeof(ModuleInitializer).Assembly);

		// Initializes for step searchers.
		StepSearcherPool.Collection = StepSearcherPool.DefaultCollection();
	}
}
