using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Sudoku.CodeGenerating;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	[CodeAnalyzer("SD0411", "SD0412")]
	public sealed partial class RegexAttributeAnalyzer : DiagnosticAnalyzer
	{
		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSymbolAction(AnalyzeSymbol, new[] { SymbolKind.NamedType });
		}


		private static void AnalyzeSymbol(SymbolAnalysisContext context)
		{
			var (symbol, compilation, cancellationToken) = context;

			if (symbol is not INamedTypeSymbol namedType)
			{
				return;
			}

			var attribute = compilation.GetTypeByMetadataName("System.Text.RegularExpressions.RegexAttribute");
			if (attribute is null)
			{
				return;
			}

			Func<ISymbol?, ISymbol?, bool> f = SymbolEqualityComparer.Default.Equals;
			var @string = compilation.GetSpecialType(SpecialType.System_String);
			foreach (var field in namedType.GetMembers().OfType<IFieldSymbol>())
			{
				if (!field.GetAttributes().Any(a => f(a.AttributeClass, attribute)))
				{
					continue;
				}

				checkSD0411(field);
				checkSD0412(field);
			}


			void checkSD0411(IFieldSymbol field)
			{
				if (f(field.Type, @string))
				{
					return;
				}

				context.ReportDiagnostic(
					Diagnostic.Create(
						descriptor: SD0411,
						location: field.DeclaringSyntaxReferences[0].GetSyntax(cancellationToken).GetLocation(),
						messageArgs: null
					)
				);
			}

			void checkSD0412(IFieldSymbol field)
			{
				if (field is not { HasConstantValue: true, ConstantValue: string value })
				{
					return;
				}

				try
				{
					Regex.Match(string.Empty, value, RegexOptions.ExplicitCapture, TimeSpan.FromSeconds(5));
				}
				catch (Exception ex) when (ex is ArgumentException or RegexMatchTimeoutException)
				{
					context.ReportDiagnostic(
						Diagnostic.Create(
							descriptor: SD0412,
							location: field.DeclaringSyntaxReferences[0].GetSyntax(cancellationToken).GetLocation(),
							messageArgs: null
						)
					);
				}
			}
		}
	}
}
