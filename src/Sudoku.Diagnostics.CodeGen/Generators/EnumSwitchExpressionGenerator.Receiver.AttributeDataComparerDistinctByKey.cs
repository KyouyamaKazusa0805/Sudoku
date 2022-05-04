namespace Sudoku.Diagnostics.CodeGen.Generators;

partial class EnumSwitchExpressionGenerator
{
	partial record class Receiver
	{
		/// <summary>
		/// Defines a comparer that compares for the inner key.
		/// </summary>
		private sealed class AttributeDataComparerDistinctByKey : IEqualityComparer<AttributeData>
		{
			/// <inheritdoc/>
			public bool Equals(AttributeData x, AttributeData y)
			{
				if (!SymbolEqualityComparer.Default.Equals(x.AttributeClass, y.AttributeClass))
				{
					return false;
				}

				string? key = (string?)x.ConstructorArguments[0].Value;
				string? another = (string?)y.ConstructorArguments[0].Value;
				return key == another;
			}

			/// <inheritdoc/>
			public int GetHashCode(AttributeData obj) => SymbolEqualityComparer.Default.GetHashCode(obj.AttributeClass);
		}
	}
}
