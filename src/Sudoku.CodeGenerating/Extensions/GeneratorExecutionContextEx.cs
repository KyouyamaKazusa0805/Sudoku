using System;
using Microsoft.CodeAnalysis;

namespace Sudoku.CodeGenerating.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="GeneratorExecutionContext"/>.
	/// </summary>
	/// <seealso cref="GeneratorExecutionContext"/>
	public static class GeneratorExecutionContextEx
	{
		/// <summary>
		/// To check whether the context isn't executed on the specified projects.
		/// </summary>
		/// <param name="this">The context.</param>
		/// <param name="projectNames">The project names to check.</param>
		/// <returns>A <see cref="bool"/> result.</returns>
		public static bool IsNotInProject(this in GeneratorExecutionContext @this, params string[] projectNames)
		{
			string? assemblyName = @this.Compilation.AssemblyName;
			return !Array.Exists(projectNames, projectName => assemblyName == projectName);
		}
	}
}
