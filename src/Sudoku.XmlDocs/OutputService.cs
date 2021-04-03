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
				foreach (var typeDeclaration in typeDeclarationSyntaxes)
				{
					// Gather all member information.
					var memberInfos = GetMemberSyntaxInfos(typeDeclaration);
				}
			}
		}


		/// <summary>
		/// Get information from all members, and returns the list of those instances.
		/// </summary>
		/// <param name="typeDeclaration">The type declaration syntax node.</param>
		/// <returns>The list of information instances.</returns>
		private IList<MemberSyntaxInfo> GetMemberSyntaxInfos(TypeDeclarationSyntax typeDeclaration)
		{
			var result = new List<MemberSyntaxInfo>();

			// Check whether the type is a record. New we should extract its primary constructor.
			if (typeDeclaration is RecordDeclarationSyntax { ParameterList: var paramList } recordDeclaration)
			{
				// The record doesn't contain the primary constructor.
				if (paramList is not { Parameters: { Count: not 0 } parameters })
				{
					goto GatherMembers;
				}

				// The record contains the primary constructor. Now store them.
				//var resultParamList = new List<(string ParamName, string Type, string? Description)>();
				//foreach (var (type, identifier) in parameters)
				//{
				//	string descrption = recordDeclaration.GetParamDescription(identifier.ValueText);
				//	resultParamList.Add((identifier.ValueText, type?.ToString() ?? string.Empty, descrption));
				//}

				//// Add into the result.
				//result.Add(
				//	new PrimaryConstructorSyntaxInfo(
				//		recordDeclaration,
				//		CustomAccessibility.Public,
				//		resultParamList.ToArray(),
				//		recordDeclaration.GetExceptionList(),
				//		recordDeclaration.GetSeeAlsoList()
				//	)
				//);
			}

		GatherMembers:
			// Normal type (class, struct or interface). Now we should check its members.
			foreach (var memberDeclarationSyntax in
				typeDeclaration.DescendantNodes().OfType<MemberDeclarationSyntax>())
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
							result.Add(
								new FieldSyntaxInfo(
									fieldDeclaration,
									fieldDeclarator.Identifier.ValueText,
									CustomAccessibility.Public,
									modifiers.GetCustomModifiers(),
									fieldDeclaration.GetSummary(),
									fieldDeclaration.GetRemarks(),
									fieldDeclaration.GetExample(),
									fieldDeclaration.GetExceptions(),
									fieldDeclaration.GetSeeAlsoList()
								)
							);
						}

						break;
					}
					//case ConstructorDeclarationSyntax
					//{
					//	Modifiers: var modifiers,
					//	ParameterList: { Parameters: var parameterList }
					//} constructorDeclaration when modifiers.Contains(CustomAccessibility.Public):
					//{
					//	result.Add(
					//		new ConstructorSyntaxInfo(
					//			constructorDeclaration,
					//			constructorDeclaration.Identifier.ValueText,
					//			CustomAccessibility.Public,
					//			modifiers.GetCustomModifiers(),
					//			constructorDeclaration.GetSummary(),
					//			constructorDeclaration.GetRemarks(),
					//			constructorDeclaration.GetExample(),
					//			parameterList.ToSyntaxInfoItem(),
					//			constructorDeclaration.GetExceptionList(),
					//			constructorDeclaration.GetSeeAlsoList()
					//		)
					//	);
					//
					//	break;
					//}
				}
			}

			return result;
		}
	}
}
