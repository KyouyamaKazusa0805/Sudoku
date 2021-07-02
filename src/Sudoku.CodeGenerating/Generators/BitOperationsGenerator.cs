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

			#region Global file
			context.AddSource("BitOperationsEx.g.cs", GenerateGlobalFile());
			#endregion

			const string separator = "\r\n\r\n\t\t";

			var sb = new StringBuilder();

			#region GetAllSets
			sb
				.Append(LeadingText)
				.AppendLine(
					string.Join(
						separator,
						from name in GetAllSetsTypes select GenerateGetAllSets(name)
					)
				)
				.Append(TrailingText);

			context.AddSource("BitOperationsEx.GetAllSets.g.cs", sb.ToString());
			#endregion

			sb.Clear();

			#region GetEnumerator
			sb
				.Append(LeadingText)
				.AppendLine(
					string.Join(
						separator,
						from name in GetEnumeratorTypes select GenerateGetEnumerator(name)
					)
				)
				.Append(TrailingText);

			context.AddSource("BitOperationsEx.GetEnumerator.g.cs", sb.ToString());
			#endregion

			sb.Clear();

			#region GetNextSet
			sb
				.Append(LeadingText)
				.AppendLine(
					string.Join(
						separator,
						from pair in GetNextSetTypes select GenerateGetNextSet(pair.TypeName, pair.Size)
					)
				)
				.Append(TrailingText);

			context.AddSource("BitOperationsEx.GetNextSet.g.cs", sb.ToString());
			#endregion

			sb.Clear();

			#region ReverseBits
			sb
				.Append(LeadingText)
				.AppendLine(
					string.Join(
						separator,
						from pair in ReverseBitsTypes select GenerateReverseBits(pair.TypeName, pair.Size)
					)
				)
				.Append(TrailingText);

			context.AddSource("BitOperationsEx.ReverseBits.g.cs", sb.ToString());
			#endregion

			sb.Clear();

			#region SetAt
			sb
				.Append(LeadingText)
				.AppendLine(
					string.Join(
						separator,
						from name in SetAtTypes select GenerateSetAt(name)
					)
				)
				.Append(TrailingText);

			context.AddSource("BitOperationsEx.SetAt.g.cs", sb.ToString());
			#endregion

			sb.Clear();

			#region SkipSetBit
			sb
				.Append(LeadingText)
				.AppendLine(
					string.Join(
						separator,
						from name in SkipSetBitTypes select GenerateSkipSetBit(name)
					)
				)
				.Append(TrailingText);

			context.AddSource("BitOperationsEx.SkipSetBit.g.cs", sb.ToString());
			#endregion
		}

		/// <inheritdoc/>
		public void Initialize(GeneratorInitializationContext context)
		{
		}
	}
}
