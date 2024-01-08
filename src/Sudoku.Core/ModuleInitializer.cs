namespace Sudoku;

/// <include file="../../global-doc-comments.xml" path="//g/csharp9/feature[@name='module-initializer']/target[@name='type']"/>
internal static class ModuleInitializer
{
	/// <include file="../../global-doc-comments.xml" path="//g/csharp9/feature[@name='module-initializer']/target[@name='method']"/>
	[DebuggerStepThrough]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[ModuleInitializer]
	[MethodImpl(MethodImplOptions.NoInlining)]
	[Obsolete("This method cannot be called by ourselves.", true)]
	[RequiresUnreferencedCode("Module initializer cannot be invoked by user ourselves.")]
	public static void Initialize() => ResourceDictionary.RegisterResourceManager<CoreResources>();
}
