namespace Sudoku.CommandLine;

/// <include file='../../global-doc-comments.xml' path='g/csharp9/feature[@name="module-initializer"]/target[@name="type"]' />
internal static class ModuleInitializer
{
	/// <include file='../../global-doc-comments.xml' path='g/csharp9/feature[@name="module-initializer"]/target[@name="method"]' />
	[ModuleInitializer]
	[RequiresUnreferencedCode(ModuleInitializerMessage.ModuleInitializerCannotBeCalledManually)]
	public static void Initialize() => R.AddExternalResourceFetecher(typeof(ModuleInitializer).Assembly, TextResources.ResourceManager.GetString);
}
