using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sudoku.CodeAnalysis.Extensions;

namespace Sudoku.CodeAnalysis
{
	partial class PropertiesInStepSearcherAnalyzer
	{
		/// <summary>
		/// Encapsulates a target property searcher.
		/// </summary>
		private sealed class TargetPropertySearcher : CSharpSyntaxWalker
		{
			/// <summary>
			/// Indicates the compilation.
			/// </summary>
			private readonly Compilation _compilation;

			/// <summary>
			/// Indicates the semantic model of this syntax tree.
			/// </summary>
			private readonly SemanticModel _semanticModel;


			/// <summary>
			/// Initializes an instance with the specified compilation and the semantic model.
			/// </summary>
			/// <param name="compilation">The compilation.</param>
			/// <param name="semanticModel">The semantic model.</param>
			public TargetPropertySearcher(Compilation compilation, SemanticModel semanticModel)
			{
				_compilation = compilation;
				_semanticModel = semanticModel;
			}


			/// <summary>
			/// Indicates whether the walker found the target property.
			/// </summary>
			/// <remarks>
			/// The result value can be:
			/// <list type="table">
			/// <item>
			/// <term><c><see langword="true"/></c></term>
			/// <description>The target property named '<c>Properties</c>' found.</description>
			/// </item>
			/// <item>
			/// <term><c><see langword="false"/></c></term>
			/// <description>
			/// The target property doesn't exist, but the class being checked
			/// derives from <c>StepSearcher</c>.
			/// </description>
			/// </item>
			/// <item>
			/// <term><c><see langword="null"/></c></term>
			/// <description>The class being checked doesn't derive from <c>StepSearcher</c>.</description>
			/// </item>
			/// </list>
			/// </remarks>
			public bool? HasTargetProperty { get; private set; }

			/// <summary>
			/// Indicatest the class declration syntax node. The value is not <see langword="null"/>
			/// when the property <see cref="HasTargetProperty"/>
			/// is <see langword="true"/> or <see langword="false"/>.
			/// </summary>
			/// <seealso cref="HasTargetProperty"/>
			public ClassDeclarationSyntax? ClassDeclaration { get; private set; }

			/// <summary>
			/// Indicates the result collection.
			/// </summary>
			/// <remarks>
			/// The value can be used if and only if
			/// the property <see cref="HasTargetProperty"/> is <see langword="true"/>.
			/// </remarks>
			/// <seealso cref="HasTargetProperty"/>
			public (PropertyDeclarationSyntax Node, IPropertySymbol Symbol) TargetPropertyInfo { get; private set; }


			/// <inheritdoc/>
			public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
			{
				if (node.Parent is not ClassDeclarationSyntax classNode)
				{
					return;
				}

				switch (_semanticModel.GetDeclaredSymbol(classNode))
				{
					case null:
					case { IsAbstract: true }:
					case { IsAnonymousType: true }:
					case { IsStatic: true }:
					{
						return;
					}
					case var classSymbol when classSymbol.DerivedFrom(_compilation, StepSearcherTypeFullName):
					{
						HasTargetProperty = false;
						ClassDeclaration = classNode;

						var propertySymbols = classSymbol.GetMembers().OfType<IPropertySymbol>();
						if (!propertySymbols.Any())
						{
							return;
						}

						foreach (var propertySymbol in propertySymbols)
						{
							switch (propertySymbol)
							{
								case { IsIndexer: true }:
								case { IsStatic: false }:
								case { Name: not TargetPropertyName }:
								{
									continue;
								}
								default:
								{
									HasTargetProperty = true;
									TargetPropertyInfo = (node, propertySymbol);

									return;
								}
							}
						}

						break;
					}
				}
			}
		}
	}
}
