namespace SudokuStudio;

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
	[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
	public static void __Initialize() => ResourceDictionary.RegisterResourceManager<SudokuStudioResources>();
}
