namespace Sudoku.Diagnostics.CodeGen;

/// <summary>
/// Defines a source generator that generates the source code on auto-notifications.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class AutoNotificationGenerator : ISourceGenerator
{
	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		var receiver = (SyntaxContextReceiver)context.SyntaxContextReceiver!;
		if (
			receiver is not
			{
				Diagnostics: { Count: var diagnosticResultCount } diagnostics,
				Gathered: var gathered
			}
		)
		{
			return;
		}

		if (diagnosticResultCount != 0)
		{
			// Disable the source generation if any diagnostic result is found.
			foreach (var diagnostic in diagnostics)
			{
				context.ReportDiagnostic(diagnostic);
			}

			return;
		}

		foreach (var fieldSymbol in gathered)
		{
			string fieldTypeName = fieldSymbol.Type.ToDisplayString();
			string typeName = fieldSymbol.ContainingType.Name;
			string namespaceName = fieldSymbol.ContainingNamespace.ToDisplayString();
			string name = fieldSymbol.Name;
			string targetName = $"{char.ToUpper(name[1])}{name.Substring(2)}";
			context.AddSource(
				$"{typeName}.{name}.g.pan.cs",
				$@"#pragma warning disable CS1591

namespace {namespaceName};

partial class {typeName}
{{
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	public {fieldTypeName} {targetName}
	{{
		get => {name};

		set {{ {name} = value; RaiseNotification(nameof({targetName})); }}
	}}
}}"
			);
		}
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context) =>
		context.RegisterForSyntaxNotifications(() => new SyntaxContextReceiver(context.CancellationToken));


	/// <summary>
	/// Indicates the syntax context receiver.
	/// </summary>
	private sealed class SyntaxContextReceiver : ISyntaxContextReceiver
	{
		/// <summary>
		/// Indicates the SCA0013 descriptor.
		/// </summary>
		[SuppressMessage("MicrosoftCodeAnalysisReleaseTracking", "RS2008:Enable analyzer release tracking", Justification = "<Pending>")]
		private static readonly DiagnosticDescriptor SCA0013 = new(
			nameof(SCA0013),
			title: "For the consideration on encapsulation, the field marked '[PropertyAutoNotify]' cannot be pointer types",
			messageFormat: "For the consideration on encapsulation, the field marked '[PropertyAutoNotify]' cannot be pointer types",
			category: "SourceGen",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true,
			helpLinkUri: null
		);

		/// <summary>
		/// Indicates the SCA0014 descriptor.
		/// </summary>
		[SuppressMessage("MicrosoftCodeAnalysisReleaseTracking", "RS2008:Enable analyzer release tracking", Justification = "<Pending>")]
		private static readonly DiagnosticDescriptor SCA0014 = new(
			nameof(SCA0014),
			title: "The identifier of the field should start with underline and be applied camel-casing style",
			messageFormat: "The identifier of the field should start with underline and be applied camel-casing style",
			category: "SourceGen",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true,
			helpLinkUri: null
		);

		/// <summary>
		/// Indicates the SCA0015 descriptor.
		/// </summary>
		[SuppressMessage("MicrosoftCodeAnalysisReleaseTracking", "RS2008:Enable analyzer release tracking", Justification = "<Pending>")]
		private static readonly DiagnosticDescriptor SCA0015 = new(
			nameof(SCA0015),
			title: "The type should be partial type if any field is marked '[PropertyAutoNotify]'",
			messageFormat: "The type should be partial type if any field is marked '[PropertyAutoNotify]'",
			category: "SourceGen",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true,
			helpLinkUri: null
		);

		/// <summary>
		/// Indicates the SCA0016 descriptor.
		/// </summary>
		[SuppressMessage("MicrosoftCodeAnalysisReleaseTracking", "RS2008:Enable analyzer release tracking", Justification = "<Pending>")]
		private static readonly DiagnosticDescriptor SCA0016 = new(
			nameof(SCA0016),
			title: "The containing type of the field marked '[PropertyAutoNotify]' cannot be generic one",
			messageFormat: "The containing type of the field marked '[PropertyAutoNotify]' cannot be generic one",
			category: "SourceGen",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true,
			helpLinkUri: null
		);

		/// <summary>
		/// Indicates the SCA0017 descriptor.
		/// </summary>
		[SuppressMessage("MicrosoftCodeAnalysisReleaseTracking", "RS2008:Enable analyzer release tracking", Justification = "<Pending>")]
		private static readonly DiagnosticDescriptor SCA0017 = new(
			nameof(SCA0017),
			title: "The containing type of the field marked '[PropertyAutoNotify]' must derive from 'NotificationObject'",
			messageFormat: "The containing type of the field marked '[PropertyAutoNotify]' must derive from 'NotificationObject'",
			category: "SourceGen",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true,
			helpLinkUri: null
		);

		/// <summary>
		/// Indicates the SCA0018 descriptor.
		/// </summary>
		[SuppressMessage("MicrosoftCodeAnalysisReleaseTracking", "RS2008:Enable analyzer release tracking", Justification = "<Pending>")]
		private static readonly DiagnosticDescriptor SCA0018 = new(
			nameof(SCA0018),
			title: "The field cannot be modified 'static' if it is marked '[PropertyAutoNotify]'",
			messageFormat: "The field cannot be modified 'static' if it is marked '[PropertyAutoNotify]'",
			category: "SourceGen",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true,
			helpLinkUri: null
		);

		/// <summary>
		/// Indicates the SCA0019 descriptor.
		/// </summary>
		[SuppressMessage("MicrosoftCodeAnalysisReleaseTracking", "RS2008:Enable analyzer release tracking", Justification = "<Pending>")]
		private static readonly DiagnosticDescriptor SCA0019 = new(
			nameof(SCA0019),
			title: "The field cannot be modified 'readonly' if it is marked '[PropertyAutoNotify]'",
			messageFormat: "The field cannot be modified 'readonly' if it is marked '[PropertyAutoNotify]'",
			category: "SourceGen",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true,
			helpLinkUri: null
		);

		/// <summary>
		/// Indicates the pattern that matches an identifier.
		/// </summary>
		private static readonly Regex IdentifierPattern = new(
			@"_[a-z0-9]\w+",
			RegexOptions.Compiled,
			TimeSpan.FromSeconds(5)
		);


		/// <summary>
		/// Indicates the cancellation token to cancel the operation.
		/// </summary>
		private readonly CancellationToken _cancellationToken;


		/// <summary>
		/// Initializes a <see cref="SyntaxContextReceiver"/> instance via the cancellation token.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
		public SyntaxContextReceiver(CancellationToken cancellationToken) =>
			_cancellationToken = cancellationToken;


		/// <summary>
		/// Indicates the diagnostic results to report.
		/// </summary>
		public ICollection<Diagnostic> Diagnostics { get; } = new List<Diagnostic>();

		/// <summary>
		/// Indicates the target result.
		/// </summary>
		public ICollection<IFieldSymbol> Gathered { get; } = new List<IFieldSymbol>();


		/// <inheritdoc/>
		public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
		{
			if (context is not { Node: var node, SemanticModel: { Compilation: var compilation } semanticModel })
			{
				return;
			}

			var possibleFieldSymbol = semanticModel.GetDeclaredSymbol(node, _cancellationToken);
			if (
				possibleFieldSymbol is not IFieldSymbol
				{
					Type: var fieldType,
					IsStatic: var isStatic,
					IsReadOnly: var isReadOnly,
					Name: var name,
					DeclaringSyntaxReferences: { Length: 1 } syntaxReferences,
					ContainingType:
					{
						IsGenericType: var isGenericType,
						DeclaringSyntaxReferences: { Length: var length } containingTypeSyntaxReferences
					} containingType
				} fieldSymbol
			)
			{
				return;
			}

			var attributesData = fieldSymbol.GetAttributes();
			var attributeSymbol = compilation.GetTypeByMetadataName(typeof(PropertyAutoNotifyAttribute).FullName)!;
			if (attributesData.All(a => !SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeSymbol)))
			{
				return;
			}

			if (fieldType is IPointerTypeSymbol or IFunctionPointerTypeSymbol)
			{
				var syntaxReference = syntaxReferences[0];
				var location = Location.Create(syntaxReference.SyntaxTree, syntaxReference.Span);
				Diagnostics.Add(Diagnostic.Create(SCA0013, location, messageArgs: null));
				return;
			}

			if (isStatic)
			{
				var syntaxReference = syntaxReferences[0];
				var location = Location.Create(syntaxReference.SyntaxTree, syntaxReference.Span);
				Diagnostics.Add(Diagnostic.Create(SCA0018, location, messageArgs: null));
				return;
			}

			if (isReadOnly)
			{
				var syntaxReference = syntaxReferences[0];
				var location = Location.Create(syntaxReference.SyntaxTree, syntaxReference.Span);
				Diagnostics.Add(Diagnostic.Create(SCA0019, location, messageArgs: null));
				return;
			}

			if (isGenericType)
			{
				var containingTypeSyntaxReference = containingTypeSyntaxReferences[0];
				var syntaxNode = (TypeDeclarationSyntax)containingTypeSyntaxReference.GetSyntax(_cancellationToken);
				var identifier = syntaxNode.Identifier;
				Diagnostics.Add(Diagnostic.Create(SCA0016, identifier.GetLocation(), messageArgs: null));
				return;
			}

			if (!IdentifierPattern.IsMatch(name))
			{
				var syntaxReference = syntaxReferences[0];
				var location = Location.Create(syntaxReference.SyntaxTree, syntaxReference.Span);
				Diagnostics.Add(Diagnostic.Create(SCA0014, location, messageArgs: null));
				return;
			}

			if (length == 1)
			{
				var containingTypeSyntaxReference = containingTypeSyntaxReferences[0];
				var syntaxNode = (TypeDeclarationSyntax)containingTypeSyntaxReference.GetSyntax(_cancellationToken);
				var identifier = syntaxNode.Identifier;
				var modifiers = syntaxNode.Modifiers;
				if (!modifiers.Any(SyntaxKind.PartialKeyword))
				{
					Diagnostics.Add(Diagnostic.Create(SCA0015, identifier.GetLocation(), messageArgs: null));
					return;
				}
			}

			bool isDerivedFromNotificationObject = false;
			var notificationObjectSymbol = compilation.GetTypeByMetadataName(typeof(NotificationObject).FullName);
			for (var cur = containingType; cur is not null; cur = cur.BaseType)
			{
				if (SymbolEqualityComparer.Default.Equals(notificationObjectSymbol, cur))
				{
					isDerivedFromNotificationObject = true;
					break;
				}
			}
			if (!isDerivedFromNotificationObject)
			{
				var containingTypeSyntaxReference = containingTypeSyntaxReferences[0];
				var syntaxNode = (TypeDeclarationSyntax)containingTypeSyntaxReference.GetSyntax(_cancellationToken);
				var identifier = syntaxNode.Identifier;
				Diagnostics.Add(Diagnostic.Create(SCA0017, identifier.GetLocation(), messageArgs: null));
				return;
			}

			Gathered.Add(fieldSymbol);
		}
	}
}
