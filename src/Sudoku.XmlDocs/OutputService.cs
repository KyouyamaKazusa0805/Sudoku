using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sudoku.Diagnostics;
using Sudoku.XmlDocs.Extensions;
using Sudoku.XmlDocs.SyntaxInfo;

namespace Sudoku.XmlDocs
{
	/// <summary>
	/// Indicates the output service.
	/// </summary>
	public sealed class OutputService
	{
		/// <summary>
		/// Initializes an <see cref="OutputService"/> with the default instantiation behavior.
		/// </summary>
		public OutputService()
		{
			RootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
			XmlDocDirectoryPath = $@"{RootPath}\docxml";
		}


		/// <summary>
		/// Indicates the root path that stores all projects.
		/// </summary>
		public string RootPath { get; }

		/// <summary>
		/// Indicates the directory path that stores all documentation files about all possible assemblies.
		/// </summary>
		public string XmlDocDirectoryPath { get; }


		/// <summary>
		/// Execute the service, and outputs the documentation files.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The task of the execution.</returns>
		public async Task ExecuteAsync(CancellationToken cancellationToken = default)
		{
#if CONSOLE
			Console.WriteLine("Start execution...");
#endif

			// Try to get all possible files in this whole solution.
			var filePaths = (await new FileCounter(RootPath, "cs", false).CountUpAsync()).FileList;

			// Declares the result file collection to store the result.
			var markdownFilesContent = new List<(string FileName, string Content)>();

			// Store all possible compilations.
			var compilations = new Dictionary<string, Compilation>();
			foreach (string path in filePaths)
			{
				string dirName = Path.GetDirectoryName(path)!;
				string projectName = dirName[dirName.LastIndexOf('\\')..];
				var compilation = CSharpCompilation.Create(projectName);

				if (!compilations.ContainsKey(projectName))
				{
					compilations.Add(projectName, compilation);
				}
			}

			// Iterate on each file via the path.
			foreach (string path in filePaths)
			{
				// Try to get the code.
				string text = await File.ReadAllTextAsync(path, cancellationToken);

				// Try to get the syntax tree.
				var tree = CSharpSyntaxTree.ParseText(text, cancellationToken: cancellationToken);

				// Try to get the semantic model.
				string dirName = Path.GetDirectoryName(path)!;
				string projectName = dirName[dirName.LastIndexOf('\\')..];
				var semanticModel = compilations[projectName].GetSemanticModel(tree);

				// Try to get the syntax node of the root.
				var node = await tree.GetRootAsync(cancellationToken);

				// Try to get the type declarations.
				var typeDeclarationSyntaxes = node.DescendantNodes().OfType<TypeDeclarationSyntax>();

				// Iterate on each syntax node of declarations.
				foreach (var typeDeclarationSyntax in typeDeclarationSyntaxes)
				{
					// Gather all member information.
					var memberInfos = GetMemberSyntaxInfos(typeDeclarationSyntax, semanticModel);
				}
			}
		}


		/// <summary>
		/// Get information from all members, and returns the list of those instances.
		/// </summary>
		/// <param name="typeDeclarationSyntax">The type declaration syntax node.</param>
		/// <param name="semanticModel">The semantic model.</param>
		/// <returns>The list of information instances.</returns>
		private IList<MemberSyntaxInfo> GetMemberSyntaxInfos(
			TypeDeclarationSyntax typeDeclarationSyntax, SemanticModel semanticModel)
		{
			var result = new List<MemberSyntaxInfo>();

			foreach (var memberDeclarationSyntax in
				typeDeclarationSyntax.DescendantNodes().OfType<MemberDeclarationSyntax>())
			{
				switch (memberDeclarationSyntax)
				{
					case FieldDeclarationSyntax
					{
						Modifiers: var modifiers,
						Declaration: { Variables: var variables }
					} fieldDeclaration when modifiers.Contains(CustomAccessibility.Public):
					{
						foreach (var fieldDeclarator in variables)
						{
							string text = fieldDeclarator.Identifier.ValueText;
							result.Add(
								new FieldSyntaxInfo(
									fieldDeclaration,
									semanticModel,
									text,
									CustomAccessibility.Public,
									modifiers.GetCustomModifiers(),
									null,
									null,
									(null, null)
								)
							);
						}

						break;
					}
				}
			}

			return result;
		}
	}
}
