namespace Sudoku.CodeGenerating.Generators;

/// <summary>
/// Provides a generator that generates the deconstruction methods.
/// </summary>
[Generator]
public sealed partial class DeconstructMethodGenerator : ISourceGenerator
{
	/// <summary>
	/// Indicates the collection that stores the conversion relations from type keywords to their BCL names.
	/// </summary>
	private static readonly IReadOnlyDictionary<string, string> KeywordsToBclNames = new Dictionary<string, string>
	{
		["int"] = "System.Int32",
		["uint"] = "System.UInt32",
		["short"] = "System.Int16",
		["ushort"] = "System.UInt16",
		["long"] = "System.Int64",
		["ulong"] = "System.UInt64",
		["byte"] = "System.Byte",
		["sbyte"] = "System.SByte",
		["bool"] = "System.Boolean",
		["nint"] = "System.IntPtr",
		["nuint"] = "System.UIntPtr",
		["float"] = "System.Single",
		["double"] = "System.Double",
		["decimal"] = "System.Decimal",
		["char"] = "System.Char",
		["string"] = "System.String",
		["object"] = "System.Object"
	};


	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		var receiver = (SyntaxReceiver)context.SyntaxReceiver!;
		var compilation = context.Compilation;
		var attributeSymbol = compilation.GetTypeByMetadataName(typeof(AutoDeconstructAttribute).FullName);

		foreach (var (typeSymbol, attributesData) in
			from type in receiver.Candidates
			let model = compilation.GetSemanticModel(type.SyntaxTree)
			select model.GetDeclaredSymbol(type)! into typeSymbol
			let attributesData =
				from attributeData in typeSymbol.GetAttributes()
				where !attributeData.ConstructorArguments.IsDefaultOrEmpty
				let tempSymbol = attributeData.AttributeClass
				where SymbolEqualityComparer.Default.Equals(tempSymbol, attributeSymbol)
				select attributeData
			where attributesData.Any()
			select (typeSymbol, attributesData))
		{
			typeSymbol.DeconstructInfo(
				false, out string fullTypeName, out string namespaceName, out string genericParametersList,
				out _, out string typeKind, out string readonlyKeyword, out _
			);
			var possibleMembers = (
				from x in GetMembers(typeSymbol, attributeSymbol, false)
				select (Info: x, Param: $"out {x.Type} {x.ParameterName}")
			).ToArray();
			string methods = string.Join(
				"\r\n\r\n\t",
				from attributeData in attributesData
				let memberArgs = attributeData.ConstructorArguments[0].Values
				select (from memberArg in memberArgs select ((string)memberArg.Value!).Trim()) into members
				where members.All(m => possibleMembers.Any(p => p.Info.Name == m))
				let paramInfos = from m in members select possibleMembers.First(p => p.Info.Name == m)
				let deprecatedTypeNames = (
					from paramInfo in paramInfos
					select paramInfo.Info into info
					let tempTypeName = info.Type
					where !KeywordsToBclNames.ContainsKey(tempTypeName)
					let tempSymbol = info.Symbol
					where tempSymbol.CheckAnyTypeArgumentIsMarked<ObsoleteAttribute>(compilation)
					select $"'{tempTypeName}'"
				).ToArray()
				let obsoleteAttributeStr = deprecatedTypeNames.Length switch
				{
					0 => string.Empty,
					1 => $@"
	[global::System.Obsolete(""The method is deprecated because the inner type {deprecatedTypeNames[0]} is deprecated."", false)]",
					> 1 => $@"
	[global::System.Obsolete(""The method is deprecated because the inner types {string.Join(", ", deprecatedTypeNames)} are deprecated."", false)]",
				}
				let paramNames = from paramInfo in paramInfos select paramInfo.Param
				let paramNamesStr = string.Join(", ", paramNames)
				let assignments =
					from m in members
					let paramName = possibleMembers.First(p => p.Info.Name == m).Info.ParameterName
					select $"{paramName} = {m};"
				let assignmentsStr = string.Join("\r\n\t\t", assignments)
				select $@"/// <summary>
	/// Deconstruct the instance into multiple elements.
	/// </summary>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]{obsoleteAttributeStr}
	public {readonlyKeyword}void Deconstruct({paramNamesStr})
	{{
		{assignmentsStr}
	}}"
			);

			context.AddSource(
				typeSymbol.ToFileName(),
				GeneratedFileShortcuts.DeconstructionMethod,
				$@"using System;

#nullable enable

namespace {namespaceName};

partial {typeKind}{typeSymbol.Name}{genericParametersList}
{{
	{methods}
}}
"
			);
		}
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context) => context.FastRegister<SyntaxReceiver>();


	/// <summary>
	/// Try to get all possible fields or properties in the specified type.
	/// </summary>
	/// <param name="typeSymbol">The specified symbol.</param>
	/// <param name="attributeSymbol">The attribute symbol to check.</param>
	/// <param name="handleRecursively">
	/// A <see cref="bool"/> value indicating whether the method will handle the type recursively.
	/// </param>
	/// <returns>The result list that contains all member symbols.</returns>
	private static IReadOnlyList<(string Type, string ParameterName, string Name, INamedTypeSymbol? Symbol, ImmutableArray<AttributeData> Attributes)> GetMembers(
		INamedTypeSymbol typeSymbol,
		INamedTypeSymbol? attributeSymbol,
		bool handleRecursively
	)
	{
		var result = new List<(string, string, string, INamedTypeSymbol?, ImmutableArray<AttributeData>)>(
			(
				from x in typeSymbol.GetMembers().OfType<IFieldSymbol>()
				select (
					x.Type.ToDisplayString(FormatOptions.PropertyTypeFormat),
					x.Name.ToCamelCase(),
					x.Name,
					x.Type as INamedTypeSymbol,
					x.GetAttributes()
				)
			).Concat(
				from x in typeSymbol.GetMembers().OfType<IPropertySymbol>()
				select (
					x.Type.ToDisplayString(FormatOptions.PropertyTypeFormat),
					x.Name.ToCamelCase(),
					x.Name,
					x.Type as INamedTypeSymbol,
					x.GetAttributes()
				)
			)
		);

		if (handleRecursively && typeSymbol.BaseType is { } baseType
			&& baseType.GetAttributes() is var attributesData
			&& attributesData.Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeSymbol)))
		{
			result.AddRange(GetMembers(baseType, attributeSymbol, true));
		}

		return result;
	}
}
