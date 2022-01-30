using System.Reflection;
using Sudoku.Solving.Manual;
using Sudoku.Solving.Manual.Buffer;
using Sudoku.Solving.Manual.Searchers;

namespace Sudoku.Solving;

/// <include
///     file='../../global-doc-comments.xml'
///     path='g/csharp9/feature[@name="module-initializer"]/target[@name="type"]' />
internal static class ModuleInitializer
{
	/// <include
	///     file='../../global-doc-comments.xml'
	///     path='g/csharp9/feature[@name="module-initializer"]/target[@name="method"]' />
	[ModuleInitializer]
	public static void Initialize() =>
		StepSearcherPool.Collection = (
			from type in typeof(ModuleInitializer).Assembly.GetTypes()
			where type.IsDefined(typeof(StepSearcherAttribute)) && typeof(IStepSearcher).IsAssignableFrom(type)
			select (IStepSearcher)Activator.CreateInstance(type)! into instance
			orderby instance.Options.Priority
			select instance
		).ToArray();
}
