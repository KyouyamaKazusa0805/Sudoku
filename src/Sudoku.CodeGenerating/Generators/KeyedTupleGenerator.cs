using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Sudoku.CodeGenerating.Extensions;

namespace Sudoku.CodeGenerating
{
	/// <summary>
	/// Define a keyed tuple generator.
	/// </summary>
	[Generator]
	public sealed class KeyedTupleGenerator : ISourceGenerator
	{
		/// <inheritdoc/>
		public void Execute(GeneratorExecutionContext context)
		{
			if (context.IsNotInProject(ProjectNames.SystemExtensions))
			{
				return;
			}

			var sb = new StringBuilder();
			for (int length = 2; length <= 4; length++)
			{
				int[] values = Enumerable.Range(1, length).ToArray();
				string generics = string.Join(", ", from i in values select $"T{i}");
				string commentsForGenericTypeParams = string.Join(
					"\r\n\t",
					from i in values
					select $@"/// <typeparam name=""T{i}"">The type of the property <see cref=""KeyedTuple{{{generics}}}.Item{i}""/>.</typeparam>"
				);
				string commentsForParams = string.Join(
					"\r\n\t",
					from i in values
					let p = i switch
					{
						1 when i / 10 != 1 => "st",
						2 when i / 10 != 1 => "nd",
						3 when i / 10 != 1 => "rd",
						_ => "th"
					}
					select $@"/// <param name=""Item{i}"">The {i}{p} item.</param>"
				);
				string typeParams = string.Join(", ", from i in values select $"T{i}");
				string primaryConstructorParamListWithoutPriorKey = string.Join(", ", from i in values select $"T{i} Item{i}");
				string primaryConstructorParamList = $"{primaryConstructorParamListWithoutPriorKey}, int PriorKey";
				string constructorParamList = string.Join(", ", from i in values select $"T{i} item{i}");
				string constructorParamListWithoutType = string.Join(", ", from i in values select $"item{i}");
				string commentsForConstructorParams = string.Join(
					"\r\n\t\t",
					from i in values select $@"/// <param name=""item{i}"">The item {i}.</param>"
				);
				string indexerValues = string.Join(", ", from i in values select $@"{i} => Item{i}");

				context.AddSource(
					$"KeyedTuple_{length}.cs",
					$@"#pragma warning disable 8509

using System.Runtime.CompilerServices;

#nullable enable

namespace System.Collections.Generic
{{
	/// <summary>
	///	Provides a tuple with a primary element, which means the tuple contains multiple items,
	///	but the only specified item can be output as <see cref=""string""/> text.
	///	</summary>
	{commentsForGenericTypeParams}
	{commentsForParams}
	/// <param name=""PriorKey"">The prior key.</param>
	[CompilerGenerated]
	public sealed record KeyedTuple<{typeParams}>({primaryConstructorParamList}) : ITuple
	{{
		/// <summary>
		/// Initializes an instance with the specified {length} items, and the first one is the prior key.
		/// </summary>
		{commentsForConstructorParams}
		public KeyedTuple({constructorParamList}) : this({constructorParamListWithoutType}, 1)
		{{
		}}

		/// <inheritdoc/>
		int ITuple.Length => {length};

		/// <inheritdoc/>
		object? ITuple.this[int index] => index switch {{ {indexerValues} }};

		/// <inheritdoc/>
		public override string ToString() => ((ITuple)this)[PriorKey]?.ToString() ?? string.Empty;
	}}
}}"
				);
			}
		}

		/// <inheritdoc/>
		public void Initialize(GeneratorInitializationContext context)
		{
		}
	}
}
