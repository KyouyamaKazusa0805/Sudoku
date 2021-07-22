using System;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Sudoku.CodeGenerating.Extensions;

namespace Sudoku.CodeGenerating
{
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

			const string separator = "\r\n\r\n\t\t";
			const string typeName = "System.Extensions.BitOperationsEx";

			context.AddSource(typeName, null, G_GlobalFile());

			var sb = new StringBuilder();
			var actions = new Action[] { a, b, c, d, e, f };
			Array.ForEach(actions, static action => action());


			void a()
			{
				sb
					.Append(LeadingText)
					.AppendLine(string.Join(separator, from name in GetAllSetsTypes select G_GetAllSets(name)))
					.Append(TrailingText);

				context.AddSource(typeName, "GetAllSets", sb.ToString());

				sb.Clear();
			}

			void b()
			{
				sb
					.Append(LeadingText)
					.AppendLine(string.Join(separator, from name in GetEnumeratorTypes select G_GetEnumerator(name)))
					.Append(TrailingText);

				context.AddSource(typeName, "GetEnumerator", sb.ToString());

				sb.Clear();
			}

			void c()
			{
				sb
					.Append(LeadingText)
					.AppendLine(
						string.Join(
							separator,
							from pair in GetNextSetTypes select G_GetNextSet(pair.TypeName, pair.Size)
						)
					)
					.Append(TrailingText);

				context.AddSource(typeName, "GetNextSet", sb.ToString());

				sb.Clear();
			}

			void d()
			{
				sb
					.Append(LeadingText)
					.AppendLine(
						string.Join(
							separator,
							from pair in ReverseBitsTypes select G_ReverseBits(pair.TypeName, pair.Size)
						)
					)
					.Append(TrailingText);

				context.AddSource(typeName, "ReverseBits", sb.ToString());

				sb.Clear();
			}

			void e()
			{
				sb
					.Append(LeadingText)
					.AppendLine(string.Join(separator, from name in SetAtTypes select G_SetAt(name)))
					.Append(TrailingText);

				context.AddSource(typeName, "SetAt", sb.ToString());

				sb.Clear();
			}

			void f()
			{
				sb
					.Append(LeadingText)
					.AppendLine(string.Join(separator, from name in SkipSetBitTypes select G_SkipSetBit(name)))
					.Append(TrailingText);

				context.AddSource(typeName, "SkipSetBit", sb.ToString());
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
}
