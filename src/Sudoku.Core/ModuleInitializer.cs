namespace Sudoku;

/// <include file="../../global-doc-comments.xml" path="//g/csharp9/feature[@name='module-initializer']/target[@name='type']"/>
[EditorBrowsable(EditorBrowsableState.Never)]
internal static class ModuleInitializer
{
	/// <include file="../../global-doc-comments.xml" path="//g/csharp9/feature[@name='module-initializer']/target[@name='method']"/>
	[ModuleInitializer]
	public static void Initialize() => SR.RegisterResourceManager<CoreResources>();
}
