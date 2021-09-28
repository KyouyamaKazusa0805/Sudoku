namespace Sudoku.CodeGenerating.Generators;

/// <summary>
/// Indicates the generator that generates the code about extended methods of type <c>BitOperations</c>.
/// </summary>
[Generator]
public sealed partial class BitOperationsGenerator : ISourceGenerator
{
	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		if (context.IsNotInProject(ProjectNames.SystemExtensions))
		{
			return;
		}

		const string separator = "\r\n\r\n\t";
		const string typeName = "System.Numerics.BitOperationsExensions";

		context.AddSource(typeName, null, G_GlobalFile());

		var sb = new StringBuilder();
		foreach (var action in new[] { a, b, c, d, e, f })
		{
			action();
		}

		void a()
		{
			string code = string.Join(separator, from name in GetAllSetsTypes select G_GetAllSets(name));
			context.AddSource(
				typeName,
				GeneratedFileShortcuts.BitOperations_GetAllSets,
				$"{LeadingText}{code}\r\n{TrailingText}"
			);
		}

		void b()
		{
			string code = string.Join(separator, from name in GetEnumeratorTypes select G_GetEnumerator(name));
			context.AddSource(
				typeName,
				GeneratedFileShortcuts.BitOperations_GetEnumerator,
				$"{LeadingText}{code}\r\n{TrailingText}"
			);
		}

		void c()
		{
			string code = string.Join(separator, from pair in GetNextSetTypes select G_GetNextSet(pair.TypeName, pair.Size));
			context.AddSource(
				typeName,
				GeneratedFileShortcuts.BitOperations_GetNextSet,
				$"{LeadingText}{code}\r\n{TrailingText}"
			);
		}

		void d()
		{
			string code = string.Join(separator, from pair in ReverseBitsTypes select G_ReverseBits(pair.TypeName, pair.Size));
			context.AddSource(
				typeName,
				GeneratedFileShortcuts.BitOperations_ReverseBits,
				$"{LeadingText}{code}\r\n{TrailingText}"
			);
		}

		void e()
		{
			string code = string.Join(separator, from name in SetAtTypes select G_SetAt(name));
			context.AddSource(
				typeName,
				GeneratedFileShortcuts.BitOperations_SetAt,
				$"{LeadingText}{code}\r\n{TrailingText}"
			);
		}

		void f()
		{
			string code = string.Join(separator, from name in SkipSetBitTypes select G_SkipSetBit(name));
			context.AddSource(
				typeName,
				GeneratedFileShortcuts.BitOperations_SkipSetBit,
				$"{LeadingText}{code}\r\n{TrailingText}"
			);
		}
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context)
	{
	}


	private partial string G_GlobalFile();
	private partial string G_GetAllSets(string typeName);
	private partial string G_GetEnumerator(string typeName);
	private partial string G_GetNextSet(string typeName, int size);
	private partial string G_ReverseBits(string typeName, int size);
	private partial string G_SetAt(string typeName);
	private partial string G_SkipSetBit(string typeName);
}
