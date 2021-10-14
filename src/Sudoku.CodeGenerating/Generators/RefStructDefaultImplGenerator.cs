namespace Sudoku.CodeGenerating.Generators;

/// <summary>
/// Indicates the generator that generates the default overriden methods in a <see langword="ref struct"/>.
/// </summary>
[Generator]
public sealed partial class RefStructDefaultImplGenerator : ISourceGenerator
{
	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		var receiver = (SyntaxReceiver)context.SyntaxReceiver!;
		var compilation = context.Compilation;

		foreach (var typeGroup in
			from type in receiver.Types
			let model = compilation.GetSemanticModel(type.SyntaxTree)
			select model.GetDeclaredSymbol(type)! into typeSymbol
			let whetherSymbolIsNull = typeSymbol.ContainingType is null
			group typeSymbol by whetherSymbolIsNull
		)
		{
			Action<GeneratorExecutionContext, INamedTypeSymbol, Compilation> f = typeGroup.Key
				? TopLevelStructGenerating
				: NestedStructGenerating;

			foreach (var type in typeGroup)
			{
				f(context, type, compilation);
			}
		}
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context) =>
		context.RegisterForSyntaxNotifications(static () => new SyntaxReceiver());


	private void TopLevelStructGenerating(
		GeneratorExecutionContext context,
		INamedTypeSymbol type,
		Compilation compilation
	)
	{
		type.DeconstructInfo(
			false, out _, out string namespaceName, out string genericParametersList,
			out _, out _, out string readonlyKeyword, out _
		);

		var intSymbol = compilation.GetSpecialType(SpecialType.System_Int32);
		var boolSymbol = compilation.GetSpecialType(SpecialType.System_Boolean);
		var stringSymbol = compilation.GetSpecialType(SpecialType.System_String);
		var objectSymbol = compilation.GetSpecialType(SpecialType.System_Object);

		var methods = type.GetMembers().OfType<IMethodSymbol>().ToArray();
		string equalsMethod = Array.Exists(
			methods,
			symbol =>
				symbol is { IsStatic: false, Name: nameof(Equals), Parameters: { Length: 1 } parameters }
				&& SymbolEqualityComparer.Default.Equals(parameters[0].Type, objectSymbol)
				&& SymbolEqualityComparer.Default.Equals(symbol.ReturnType, boolSymbol)
		)
			? $@"// Can't generate '{nameof(Equals)}' because the method is impl'ed by user."
			: $@"/// <inheritdoc cref=""object.Equals(object?)""/>
	/// <exception cref=""NotSupportedException"">Always throws.</exception>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
	[global::System.Diagnostics.CodeAnalysis.DoesNotReturn]
	[global::System.Obsolete(""You can't use or call this method."", true, DiagnosticId = ""BAN"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	public override {readonlyKeyword}bool Equals(object? other) => throw new NotSupportedException();";

		string getHashCodeMethod = Array.Exists(
			methods,
			symbol =>
				symbol is { IsStatic: false, Name: nameof(GetHashCode), Parameters: { Length: 0 } parameters }
				&& SymbolEqualityComparer.Default.Equals(symbol.ReturnType, intSymbol)
		)
			? $@"// Can't generate '{nameof(GetHashCode)}' because the method is impl'ed by user."
			: $@"/// <inheritdoc cref=""object.GetHashCode""/>
	/// <exception cref=""NotSupportedException"">Always throws.</exception>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
	[global::System.Diagnostics.CodeAnalysis.DoesNotReturn]
	[global::System.Obsolete(""You can't use or call this method."", true, DiagnosticId = ""BAN"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	public override {readonlyKeyword}int GetHashCode() => throw new NotSupportedException();";

		string toStringMethod = Array.Exists(
			methods,
			symbol =>
				symbol is { IsStatic: false, Name: nameof(ToString), Parameters: { Length: 0 } parameters }
				&& SymbolEqualityComparer.Default.Equals(symbol.ReturnType, stringSymbol)
		)
			? $@"// Can't generate '{nameof(ToString)}' because the method is impl'ed by user."
			: $@"/// <inheritdoc cref=""object.ToString""/>
	/// <exception cref=""NotSupportedException"">Always throws.</exception>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
	[global::System.Diagnostics.CodeAnalysis.DoesNotReturn]
	[global::System.Obsolete(""You can't use or call this method."", true, DiagnosticId = ""BAN"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	public override {readonlyKeyword}string? ToString() => throw new NotSupportedException();";

		context.AddSource(
			type.ToFileName(),
			GeneratedFileShortcuts.RefStructDefaultMethod,
			$@"#pragma warning disable CS0809

#nullable enable

namespace {namespaceName};

partial struct {type.Name}{genericParametersList}
{{
#line hidden
	{equalsMethod}

	{getHashCodeMethod}

	{toStringMethod}
#line default
}}
"
		);
	}

	private void NestedStructGenerating(
		GeneratorExecutionContext context,
		INamedTypeSymbol type,
		Compilation compilation
	)
	{
		type.DeconstructInfo(
			false, out _, out string namespaceName, out string genericParametersList,
			out _, out _, out string readonlyKeyword, out _
		);

		// If nested type, the 'genericParametersList' may contain the dot '.' such as
		//
		//     <TKey, TValue>.KeyCollection
		//
		// We should remove the characters before the dot.
		if (!string.IsNullOrEmpty(genericParametersList)
			&& genericParametersList.LastIndexOf('.') is var dot and not -1)
		{
			if (dot + 1 >= genericParametersList.Length)
			{
				return;
			}

			genericParametersList = genericParametersList.Substring(dot + 1);
			if (genericParametersList.IndexOf('<') == -1)
			{
				genericParametersList = string.Empty;
			}
		}

		// Get outer types.
		var outerTypes = new Stack<(INamedTypeSymbol Type, int Indenting)>();
		int outerTypesCount = 0;
		for (var o = type.ContainingType; o is not null; o = o.ContainingType, outerTypesCount++) ;

		string methodIndenting = new('\t', outerTypesCount + 1);
		string typeIndenting = new('\t', outerTypesCount);
		for (var outer = type.ContainingType; outer is not null; outer = outer.ContainingType)
		{
			outerTypes.Push((outer, outerTypesCount--));
		}

		StringBuilder outerTypeDeclarationsStart = new(), outerTypeDeclarationsEnd = new();
		var indentingStack = new Stack<string>();
		foreach (var (outerType, currentIndenting) in outerTypes)
		{
			outerType.DeconstructInfo(
				false, out string outerFullTypeName, out _, out _, out _,
				out string outerTypeKind, out _, out _
			);

			string outerGenericParametersList;
			int lastDot = outerFullTypeName.LastIndexOf('.');
			if (lastDot == -1)
			{
				int lt = outerFullTypeName.IndexOf('<'), gt = outerFullTypeName.IndexOf('>');
				if (lt == -1)
				{
					outerGenericParametersList = string.Empty;
				}
				else if (gt < lt)
				{
					continue;
				}
				else
				{
					outerGenericParametersList = outerFullTypeName.Substring(lt, gt - lt + 1);
				}
			}
			else
			{
				int start = lastDot + 1;
				if (start >= outerFullTypeName.Length)
				{
					continue;
				}

				string temp = outerFullTypeName.Substring(start);
				int lt = temp.IndexOf('<'), gt = temp.IndexOf('>');
				if (lt == -1)
				{
					outerGenericParametersList = string.Empty;
				}
				else if (gt < lt)
				{
					continue;
				}
				else
				{
					outerGenericParametersList = temp.Substring(lt, gt - lt + 1);
				}
			}

			string indenting = new('\t', currentIndenting - 1);

			outerTypeDeclarationsStart
				.AppendLine($"{indenting}partial {outerTypeKind}{outerType.Name}{outerGenericParametersList}")
				.AppendLine($"{indenting}{{");

			indentingStack.Push(indenting);
		}

		foreach (string indenting in indentingStack)
		{
			outerTypeDeclarationsEnd.AppendLine($"{indenting}}}");
		}


		// Remove the last new line.
		outerTypeDeclarationsStart.Remove(outerTypeDeclarationsStart.Length - 2, 2);
		outerTypeDeclarationsEnd.Remove(outerTypeDeclarationsEnd.Length - 2, 2);

		Func<ISymbol, ISymbol, bool> c = SymbolEqualityComparer.Default.Equals;
		var intSymbol = compilation.GetSpecialType(SpecialType.System_Int32);
		var boolSymbol = compilation.GetSpecialType(SpecialType.System_Boolean);
		var stringSymbol = compilation.GetSpecialType(SpecialType.System_String);
		var objectSymbol = compilation.GetSpecialType(SpecialType.System_Object);

		var methods = type.GetMembers().OfType<IMethodSymbol>().ToArray();
		string equalsMethod = Array.Exists(
			methods,
			symbol =>
				symbol is { IsStatic: false, Name: nameof(Equals), Parameters: { Length: 1 } parameters }
				&& c(parameters[0].Type, objectSymbol)
				&& c(symbol.ReturnType, boolSymbol)
		)
			? $"{methodIndenting}// Can't generate '{nameof(Equals)}' because the method is impl'ed by user."
			: $@"{methodIndenting}/// <inheritdoc cref=""object.Equals(object?)""/>
{methodIndenting}/// <exception cref=""NotSupportedException"">Always throws.</exception>
{methodIndenting}[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
{methodIndenting}[global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
{methodIndenting}[global::System.Diagnostics.CodeAnalysis.DoesNotReturn]
{methodIndenting}[global::System.Obsolete(""You can't use or call this method."", true, DiagnosticId = ""BAN"")]
{methodIndenting}[global::System.Runtime.CompilerServices.CompilerGenerated]
{methodIndenting}public override {readonlyKeyword}bool Equals(object? other) => throw new NotSupportedException();";

		string getHashCodeMethod = Array.Exists(
			methods,
			symbol =>
				symbol is { IsStatic: false, Name: nameof(GetHashCode), Parameters: { Length: 0 } parameters }
				&& c(symbol.ReturnType, intSymbol)
		)
			? $"{methodIndenting}// Can't generate '{nameof(GetHashCode)}' because the method is impl'ed by user."
			: $@"{methodIndenting}/// <inheritdoc cref=""object.GetHashCode""/>
{methodIndenting}/// <exception cref=""NotSupportedException"">Always throws.</exception>
{methodIndenting}[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
{methodIndenting}[global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
{methodIndenting}[global::System.Diagnostics.CodeAnalysis.DoesNotReturn]
{methodIndenting}[global::System.Obsolete(""You can't use or call this method."", true, DiagnosticId = ""BAN"")]
{methodIndenting}[global::System.Runtime.CompilerServices.CompilerGenerated]
{methodIndenting}public override {readonlyKeyword}int GetHashCode() => throw new NotSupportedException();";

		string toStringMethod = Array.Exists(
			methods,
			symbol =>
				symbol is { IsStatic: false, Name: nameof(ToString), Parameters: { Length: 0 } parameters }
				&& c(symbol.ReturnType, stringSymbol)
		)
			? $"{methodIndenting}// Can't generate '{nameof(ToString)}' because the method is impl'ed by user."
			: $@"{methodIndenting}/// <inheritdoc cref=""object.ToString""/>
{methodIndenting}/// <exception cref=""NotSupportedException"">Always throws.</exception>
{methodIndenting}[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
{methodIndenting}[global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
{methodIndenting}[global::System.Diagnostics.CodeAnalysis.DoesNotReturn]
{methodIndenting}[global::System.Obsolete(""You can't use or call this method."", true, DiagnosticId = ""BAN"")]
{methodIndenting}[global::System.Runtime.CompilerServices.CompilerGenerated]
{methodIndenting}public override {readonlyKeyword}string? ToString() => throw new NotSupportedException();";

		context.AddSource(
			type.ToFileName(),
			GeneratedFileShortcuts.RefStructDefaultMethod,
			$@"#pragma warning disable CS0809

using System.ComponentModel;

#nullable enable

namespace {namespaceName};

{outerTypeDeclarationsStart}
{typeIndenting}partial struct {type.Name}{genericParametersList}
{typeIndenting}{{
#line hidden
{equalsMethod}

{getHashCodeMethod}

{toStringMethod}
#line default
{typeIndenting}}}
{outerTypeDeclarationsEnd}
"
		);
	}
}
