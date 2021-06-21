using System;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Microsoft.CodeAnalysis.Operations;
using Sudoku.CodeGen;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	[CodeAnalyzer("SS9008")]
	public sealed partial class ReadOnlyPropertyAnalyzer : DiagnosticAnalyzer
	{
		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, new[] { SyntaxKind.PropertyDeclaration });
		}


		private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
		{
			var (semanticModel, _, originalNode, _, cancellationToken) = context;

			if (
				originalNode is not PropertyDeclarationSyntax
				{
					Parent: StructDeclarationSyntax,
					AccessorList: null,
					Initializer: null,
					ExpressionBody: { Expression: var expr },
					Identifier: var identifier
				}
			)
			{
				return;
			}

			switch (expr)
			{
				// => this.MethodInvocation();
				case InvocationExpressionSyntax
				{
					Expression: MemberAccessExpressionSyntax
					{
						RawKind: (int)SyntaxKind.SimpleMemberAccessExpression,
						Expression: ThisExpressionSyntax
					} methodRef
				}
				when isReadOnlyMethodRef(methodRef):
				{
					r();

					break;
				}

				// => constantValue;
				case LiteralExpressionSyntax:
				{
					r();

					break;
				}

				// => default;
				case DefaultExpressionSyntax:
				{
					r();

					break;
				}

				// => this.DataMember;
				case MemberAccessExpressionSyntax
				{
					RawKind: (int)SyntaxKind.SimpleMemberAccessExpression,
					Expression: ThisExpressionSyntax
				} dataMemberRef
				when isReadOnlyDataMemberRef(dataMemberRef):
				{
					r();

					break;
				}

				// => MethodInvocation();
				case InvocationExpressionSyntax { Expression: var methodRef } when isReadOnlyMethodRef(methodRef):
				{
					r();

					break;
				}

				// => DataMember;
				case IdentifierNameSyntax dataMemberRef when isReadOnlyDataMemberRef(dataMemberRef):
				{
					r();

					break;
				}
			}


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			void r() => context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: SS9008,
					location: identifier.GetLocation(),
					messageArgs: null,
					additionalLocations: new[] { originalNode.GetLocation() }
				)
			);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			bool isReadOnlyMethodRef(SyntaxNode methodRef) =>
				semanticModel.GetOperation(methodRef, cancellationToken) is IMethodReferenceOperation
				{
					Method: { IsReadOnly: true }
				};

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			bool isReadOnlyDataMemberRef(SyntaxNode dataMemberRef) =>
				semanticModel.GetOperation(dataMemberRef, cancellationToken)
				is IFieldReferenceOperation
				or IPropertyReferenceOperation { Property: { IsReadOnly: true } };
		}
	}
}
