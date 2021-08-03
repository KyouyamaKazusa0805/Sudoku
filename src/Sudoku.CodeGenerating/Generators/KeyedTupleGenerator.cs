using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Sudoku.CodeGenerating.Extensions;
using static Sudoku.CodeGenerating.Constants;

namespace Sudoku.CodeGenerating.Generators
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
					select $@"/// <typeparam name=""T{i}"">The type of the property <see cref=""KeyedTuple{{{
						generics
					}}}.Item{i}""/>.</typeparam>"
				);
				string commentsForParams = string.Join(
					"\r\n\t",
					from i in values select $@"/// <param name=""Item{i}"">The {i}{i.GetOrderSuffix()} item.</param>"
				);
				string typeParams = string.Join(", ", from i in values select $"T{i}");
				string primaryConstructorParamListWithoutPriorKey = string.Join(
					", ",
					from i in values select $"T{i} Item{i}"
				);
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
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	public sealed record KeyedTuple<{typeParams}>({primaryConstructorParamList})
		: global::System.Runtime.CompilerServices.ITuple
	{{
		/// <summary>
		/// Initializes an instance with the specified {length} items, and the first one is the prior key.
		/// </summary>
		{commentsForConstructorParams}
		[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
		[global::System.Runtime.CompilerServices.CompilerGenerated]
		[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		public KeyedTuple({constructorParamList}) : this({constructorParamListWithoutType}, 1)
		{{
		}}


		/// <inheritdoc/>
		[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
		[global::System.Runtime.CompilerServices.CompilerGenerated]
		int global::System.Runtime.CompilerServices.ITuple.Length
		{{
			[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
			get => {length};
		}}

		/// <inheritdoc/>
		[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
		[global::System.Runtime.CompilerServices.CompilerGenerated]
		object? global::System.Runtime.CompilerServices.ITuple.this[int index]
		{{
			[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
			get => index switch {{ {indexerValues} }};
		}}


		/// <inheritdoc/>
		[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
		[global::System.Runtime.CompilerServices.CompilerGenerated]
		[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		public override string ToString() => ((global::System.Runtime.CompilerServices.ITuple)this)[PriorKey]?.ToString() ?? string.Empty;
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
