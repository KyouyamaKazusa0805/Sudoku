namespace Sudoku.Scripting;

/// <summary>
/// Represents a type that contains a list of methods that supports evaluate methods defined in external files,
/// e.g. stored in local file path.
/// </summary>
public static class Script
{
	/// <summary>
	/// Indicates the default type name.
	/// </summary>
	internal const string DefaultTypeName = "__Script";

	/// <summary>
	/// Indicates the supported <see langword="using"/> directives.
	/// </summary>
	internal const string UsingDirectives = """
		using System;
		using System.Collections;
		using System.Collections.Generic;
		using System.Collections.Immutable;
		using System.Linq;
		using Sudoku.Analytics;
		using Sudoku.Analytics.Categorization;
		using Sudoku.Analytics.Steps;
		using Sudoku.Analytics.StepSearchers;
		using Sudoku.Concepts;
		using Sudoku.Measuring;
		using Sudoku.Measuring.Factors;
		using static System.Numerics.BitOperations;
		using static Sudoku.Concepts.ConclusionType;
		using static Sudoku.SolutionFields;
		""";

	/// <summary>
	/// Indicates the invoking binding flags.
	/// </summary>
	internal const BindingFlags DefaultBindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod;


	/// <summary>
	/// Loads external <see cref="Factor"/> source files, and take them into account,
	/// by inserting factors into <see cref="Step.ExternalFactors"/>.
	/// </summary>
	/// <param name="this">The <see cref="AnalysisResult"/> instance to be updated.</param>
	/// <param name="directoryPath">The directory path.</param>
	/// <seealso cref="Factor"/>
	/// <seealso cref="Step.ExternalFactors"/>
	public static void LoadExternalFactors(AnalysisResult @this, string directoryPath)
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// Parses the text as a valid C# code for a method declaration, and evaluate the method and return a valid result.
	/// </summary>
	/// <param name="script">
	/// The C# script, code. The text code must be a valid method declaration without keywords like <see langword="public"/>
	/// and <see langword="static"/>, with an explicit <see langword="return"/> statement.
	/// </param>
	/// <param name="methodName">Indicates the method name you want to call in this script.</param>
	/// <param name="args">The arguments to be passed.</param>
	/// <param name="cancellationToken">Indicates the cancellation token that can cancel the task.</param>
	/// <returns>A <see cref="SyntaxTree"/> object returned.</returns>
	/// <exception cref="AggregateException">Throws when failed to compile the text.</exception>
	public static async Task<object?> EvaluateAsync([StringSyntax("C#")] string script, string methodName, object?[]? args, CancellationToken cancellationToken = default)
	{
		var compilation = AnalyticsCompilation.CreateCompilation()
			.AddSyntaxTrees(
				CSharpSyntaxTree.ParseText(
					$$"""
					#nullable enable

					{{UsingDirectives}}

					public static unsafe class {{DefaultTypeName}}
					{
						public static {{script}}
					}
					""",
					cancellationToken: cancellationToken
				)
			);
		await using var stream = new MemoryStream();
		var result = compilation.Emit(stream, cancellationToken: cancellationToken);
		if (!result.Success)
		{
			throw new AggregateException(
				from d in result.Diagnostics.ToArray()
				where d is { Severity: DiagnosticSeverity.Error } or { Severity: DiagnosticSeverity.Warning, IsWarningAsError: true }
				select new Exception($"{d.Id}: {d.GetMessage()}")
			);
		}

		stream.Seek(0, SeekOrigin.Begin);
		return Assembly.Load(stream.ToArray())
			.GetType(DefaultTypeName)!
			.InvokeMember(methodName, DefaultBindingFlags, null, null, args)!;
	}
}
